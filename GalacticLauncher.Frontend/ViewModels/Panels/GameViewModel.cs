using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Domain.Exceptions;
using GalacticLauncher.Frontend.Domain.Models;
using GalacticLauncher.Frontend.Domain.Models.Extensions;
using GalacticLauncher.Frontend.Infrastructure;
using GalacticLauncher.Frontend.Services.Data;
using GalacticLauncher.Frontend.Services.Executables;
using GalacticLauncher.Frontend.Tools.Classes;
using GalacticLauncher.Frontend.ViewModels.ViewServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using GalacticLauncher.Frontend.ViewModels.Dialogs;

namespace GalacticLauncher.Frontend.ViewModels.Panels;

internal partial class GameViewModel : ObservableObject, INavigationAware
{
    [ObservableProperty]
    private string _title = "";

    [ObservableProperty]
    private string _description = "";

    [ObservableProperty]
    private string? _iconUrl;

    [ObservableProperty]
    private Version? _selectedVersion;

    [ObservableProperty]
    private double _downloadProgress;

    [ObservableProperty]
    private string _gameDisplayDebugJson = "";

    [ObservableProperty]
    private string _selectedVersionDebugJson = "";

    public ObservableCollection<Version> AvailableVersions { get; } = [];

    public enum ViewModeEnum
    {
        Locked = 0,
        NoInstance = 1,
        Downloading = 2,
        ReadyToPlay = 3,
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNoInstanceState))]
    [NotifyPropertyChangedFor(nameof(IsDownloadingState))]
    [NotifyPropertyChangedFor(nameof(IsReadyToPlayState))]
    [NotifyPropertyChangedFor(nameof(DownloadButtonText))]
    private ViewModeEnum _viewMode = ViewModeEnum.Locked;

    public bool IsNoInstanceState => ViewMode == ViewModeEnum.NoInstance;
    public bool IsDownloadingState => ViewMode == ViewModeEnum.Downloading;
    public bool IsReadyToPlayState => ViewMode == ViewModeEnum.ReadyToPlay;
    public string DownloadButtonText => IsDownloadingState ? "Downloading..." : "Download";

    private bool _init = false;
    private long _id = 0;

    private readonly TaskObserver _downloading = new();

    private readonly ICacheProvider _cacheProvider;
    private readonly ICacheRefresher _cacheRefresher;
    private readonly ILastGameManager _lastGameManager;
    private readonly IExecManager _execManager;
    private readonly ITerminator _terminator;
    private readonly IDialog _dialog;
    private readonly INotifications _notifications;

    public GameViewModel(
        ICacheProvider cacheProvider,
        ICacheRefresher cacheRefresher,
        ILastGameManager lastGameManager,
        IExecManager execManager,
        ITerminator terminator,
        IDialog dialog,
        INotifications notifications)
    {
        _cacheProvider = cacheProvider;
        _cacheRefresher = cacheRefresher;
        _lastGameManager = lastGameManager;
        _execManager = execManager;
        _terminator = terminator;
        _dialog = dialog;
        _notifications = notifications;

        _cacheRefresher.OnInitialize +=
            () => { if (_init) RunGameDataRefresh(); };

        _cacheRefresher.OnRefreshGameData +=
            id => { if (_init && _id == id) UpdateView(); };
    }

    public void OnActivate(object[] args)
    {
        _init = true;
        _id = (long)args[0];

        RunGameDataRefresh();

        ResetSelections();
        UpdateView();
    }

    private void RunGameDataRefresh()
    {
        _ = _cacheRefresher.RefreshGameDataAsync(_id);
    }

    private void ResetSelections()
    {
        SelectedVersion = null;
        DownloadProgress = 0;

        _downloading.Terminate();

        SetAdequateViewMode();
    }

    private void UpdateView()
    {
        Game? game = _cacheProvider.GetGameOf(_id);

        Title = game?.Name ?? "Unknown";
        Description = game?.Description ?? "";
        IconUrl = game?.IconUrl;

        GameDisplayDebugJson = JsonSerializer.Serialize(game);

        Version? selectedVersion = SelectedVersion;

        List<Version> versions = [.. _cacheProvider.GetVersionsOf(_id)];

        AvailableVersions.Clear();

        foreach (var version in versions)
        {
            AvailableVersions.Add(version);
        }

        SelectedVersion = selectedVersion == null
            ? AvailableVersions.FirstOrDefault(v => v.IsPrimary)
            : AvailableVersions.FirstOrDefault(v => v.Id == selectedVersion.Id);
    }

    [RelayCommand]
    private async Task DownloadSelectedVersion()
    {
        ExecInfo? execInfo = MakeCurrentExecInfo();
        if (execInfo == null) return;

        if (_execManager.Exists(execInfo)) return;
        if (_execManager.IsDownloading(execInfo)) return;

        DownloadProgress = 0;
        Progress<double> progress = new(value =>
        {
            DownloadProgress = Math.Clamp(value, 0, 1);
        });

        try
        {
            Task task = _downloading.Start(cancellationToken =>
                _execManager.DownloadAsync(execInfo, progress, cancellationToken));

            SetAdequateViewMode();

            await task;
            
            _notifications.ShowSuccess("Download Complete", $"{Title} is ready to play.");
        }
        catch (OperationCanceledException) { }
        catch (DownloadException )
        {
            _notifications.ShowError("Download Error", $"{Title} failed to download.");
        }
        finally
        {
            _downloading.Terminate();

            SetAdequateViewMode();
        }
    }

    [RelayCommand]
    private void CancelDownload()
    {
        ExecInfo? execInfo = MakeCurrentExecInfo();
        if (execInfo == null) return;

        _downloading.Terminate();

        SetAdequateViewMode();
        
        _notifications.ShowInfo("Download Cancelled", $"Download for {Title} was stopped.");
    }

    [RelayCommand]
    private async Task DeleteSelectedVersion()
    {
        ExecInfo? execInfo = MakeCurrentExecInfo();
        if (execInfo == null) return;

        var dialog = new ConfirmationDialogViewModel(
            "Delete Game",
            "Are you sure you want to delete this game?"
        );

        bool isConfirmed = await _dialog.ShowDialogAsync(dialog);

        if (isConfirmed)
        {
            if (_execManager.Exists(execInfo))
            {
                _execManager.Delete(execInfo);
                _notifications.ShowInfo("Game Deleted", $"{Title} has been deleted.");
            }
            
            ViewMode = ViewModeEnum.NoInstance;
        }
    }

    [RelayCommand]
    private void PlaySelectedVersion()
    {
        ExecInfo? execInfo = MakeCurrentExecInfo();
        if (execInfo == null) return;

        if (!_execManager.Exists(execInfo)) return;

        try
        {
            _notifications.ShowSuccess("Launching", $"Starting {Title}...");
            
            _execManager.Play(execInfo);

            long? lastGameId = _cacheProvider.GetGameOf(_id)?.Id;
            _lastGameManager.SetLastGame(lastGameId);

            _terminator.Terminate();
        }
        catch (ExecutableRunException ex)
        {
            _notifications.ShowError("Run Error", ex.Message);
        }
    }

    partial void OnSelectedVersionChanged(Version? value)
    {
        SelectedVersionDebugJson = value == null
            ? string.Empty
            : JsonSerializer.Serialize(value);

        SetAdequateViewMode();
    }

    private void SetAdequateViewMode()
    {
        if (SelectedVersion is null)
        {
            ViewMode = ViewModeEnum.Locked;
            return;
        }

        if (_downloading.IsRunning)
        {
            ViewMode = ViewModeEnum.Downloading;
            return;
        }

        ViewMode = MakeCurrentExecInfo() is { } execInfo && _execManager.Exists(execInfo)
            ? ViewModeEnum.ReadyToPlay
            : ViewModeEnum.NoInstance;
    }

    private ExecInfo? MakeCurrentExecInfo()
    {
        if (SelectedVersion == null) return null;

        GameData? gameData = _cacheProvider.GetGameDataOf(_id);
        if (gameData == null) return null;

        return gameData.ToExecInfo(SelectedVersion);
    }
}
