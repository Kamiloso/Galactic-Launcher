using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Domain.Exceptions;
using GalacticLauncher.Frontend.Domain.Models;
using GalacticLauncher.Frontend.Domain.Models.Extensions;
using GalacticLauncher.Frontend.Infrastructure;
using GalacticLauncher.Frontend.Services;
using GalacticLauncher.Frontend.Services.Cache;
using GalacticLauncher.Frontend.Services.Data;
using GalacticLauncher.Frontend.Services.Executables;
using GalacticLauncher.Frontend.Tools.Classes;
using GalacticLauncher.Frontend.ViewModels.ViewServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace GalacticLauncher.Frontend.ViewModels.Panels;

internal partial class GameViewModel : ObservableObject, INavigationAware
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelTitle))]
    private string _title = "";

    [ObservableProperty]
    private string _description = "";

    [ObservableProperty]
    private string? _iconUrl;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelTitle))]
    private Version? _selectedVersion;

    [ObservableProperty]
    private double _downloadProgress;

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
    public string DownloadButtonText => IsDownloadingState ? "DOWNLOADING..." : "DOWNLOAD";
    public string SelTitle => $"{Title} {SelectedVersion?.Caption}";

    private bool _init = false;
    private long _id = 0;

    private readonly TaskObserver _downloading = new();

    private readonly ICacheProvider _cacheProvider;
    private readonly ICacheRefresher _cacheRefresher;
    private readonly ILastGameManager _lastGameManager;
    private readonly IExecManager _execManager;
    private readonly IPreferenceManager _preferenceManager;
    private readonly ITerminator _terminator;
    private readonly IDialogs _dialogs;
    private readonly INotifications _notifications;

    public GameViewModel(
        ICacheProvider cacheProvider,
        ICacheRefresher cacheRefresher,
        ILastGameManager lastGameManager,
        IExecManager execManager,
        IPreferenceManager preferenceManager,
        ITerminator terminator,
        IDialogs dialog,
        INotifications notifications)
    {
        _cacheProvider = cacheProvider;
        _cacheRefresher = cacheRefresher;
        _lastGameManager = lastGameManager;
        _execManager = execManager;
        _preferenceManager = preferenceManager;
        _terminator = terminator;
        _dialogs = dialog;
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

        long? selVersionId = _preferenceManager.GetSelectedVersion(_id);

        List<Version> versions = [.. _cacheProvider.GetVersionsOf(_id)];

        AvailableVersions.Clear();

        foreach (var version in versions)
        {
            AvailableVersions.Add(version);
        }

        SelectedVersion = selVersionId == null
            ? AvailableVersions.FirstOrDefault(v => v.IsPrimary)
            : AvailableVersions.FirstOrDefault(v => v.Id == selVersionId);
    }

    [RelayCommand]
    private async Task DownloadSelectedVersion()
    {
        ExecInfo? execInfo = MakeCurrentExecInfo();
        if (execInfo == null) return;

        if (_execManager.Exists(execInfo)) return;
        if (_execManager.IsDownloading(execInfo)) return;

        DownloadProgress = 0;
        _notifications.ShowInfo(
            $"Download Started",
            $"Downloading {SelTitle}...");

        Progress<DownloadProgressData> progress = new();

        Task downloadTask = _downloading.Start(cancellationToken =>
            _execManager.DownloadAsync(execInfo, progress, cancellationToken));

        SetAdequateViewMode();

        try
        {
            await _dialogs.ShowDownloadProgressDialogAsync(
                $"Downloading...",
                $"Downloading {SelTitle}...",
                downloadTask, _downloading.Terminate, progress);

            _notifications.ShowSuccess(
                $"Download Complete",
                $"{SelTitle} is ready to play.");
        }
        catch (OperationCanceledException)
        {
            _notifications.ShowInfo(
                $"Download Cancelled",
                $"Download for {SelTitle} was cancelled.");
        }
        catch (DownloadException)
        {
            _notifications.ShowError(
                $"Download Error",
                $"Download for {SelTitle} has failed.");
        }
        finally
        {
            SetAdequateViewMode();
        }
    }

    [RelayCommand]
    private async Task DeleteSelectedVersion()
    {
        ExecInfo? execInfo = MakeCurrentExecInfo();
        if (execInfo == null) return;

        bool isConfirmed = await _dialogs.ShowConfirmationDialogAsync(
            $"Delete Version",
            $"Are you sure you want to delete {SelTitle}?",
            textYes: "Delete", textNo: "Cancel");

        if (isConfirmed)
        {
            if (_execManager.Exists(execInfo))
            {
                _execManager.Delete(execInfo);

                _notifications.ShowSuccess(
                    "Version Deleted",
                    $"{SelTitle} has been deleted.");
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
            _execManager.Play(execInfo);

            long? lastGameId = _cacheProvider.GetGameOf(_id)?.Id;
            _lastGameManager.SetLastGame(lastGameId);

            _terminator.Terminate();
        }
        catch (ExecutableRunException ex)
        {
            _notifications.ShowError(
                "Run Error",
                ex.Message);
        }
    }

    partial void OnSelectedVersionChanged(Version? value)
    {
        SetAdequateViewMode();

        _preferenceManager.SetSelectedVersion(_id, SelectedVersion?.Id);
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
