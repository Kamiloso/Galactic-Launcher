using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Domain.Exceptions;
using GalacticLauncher.Frontend.Domain.Models;
using GalacticLauncher.Frontend.Domain.Models.Extensions;
using GalacticLauncher.Frontend.Infrastructure;
using GalacticLauncher.Frontend.Services.Data;
using GalacticLauncher.Frontend.Services.Executables;
using GalacticLauncher.Frontend.ViewModels.ViewServices;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GalacticLauncher.Frontend.ViewModels.Panels;

internal partial class GameViewModel : ObservableObject, INavigationAware
{
    public ObservableCollection<Version> AvailableVersions { get; } = [];

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
    private string _gameDisplayDebugJson = string.Empty;

    [ObservableProperty]
    private string _selectedVersionDebugJson = string.Empty;

    [ObservableProperty]
    private ViewModeEnum _viewMode = ViewModeEnum.Nothing;

    public enum ViewModeEnum
    {
        Nothing = 0,
        Downloading = 1,
        ReadyToPlay = 2,
    }

    public string DownloadButtonText =>
        ViewMode == ViewModeEnum.Downloading ? "Downloading..." : "Download";

    public bool IsDownloadEnabled => ViewMode == ViewModeEnum.Nothing;
    public bool IsCancelEnabled => ViewMode == ViewModeEnum.Downloading;
    public bool IsPlayEnabled => ViewMode == ViewModeEnum.ReadyToPlay;
    public bool IsDeleteEnabled => ViewMode == ViewModeEnum.ReadyToPlay;
    public bool IsProgressVisible => ViewMode == ViewModeEnum.Downloading;

    private bool _init = false;
    private long _id = 0;

    private Task? _downloading;
    private CancellationTokenSource? _downloadingCancel;

    private readonly ICacheProvider _cacheProvider;
    private readonly ICacheRefresher _cacheRefresher;
    private readonly ILastGameManager _lastGameManager;
    private readonly IExecManager _execManager;
    private readonly ITerminator _terminator;

    public GameViewModel(
        ICacheProvider cacheProvider,
        ICacheRefresher cacheRefresher,
        ILastGameManager lastGameManager,
        IExecManager execManager,
        ITerminator terminator)
    {
        _cacheProvider = cacheProvider;
        _cacheRefresher = cacheRefresher;
        _lastGameManager = lastGameManager;
        _execManager = execManager;
        _terminator = terminator;

        _cacheRefresher.OnRefreshAll +=
            () => { if (_init) _ = _cacheRefresher.RefreshGame(_id); };
        _cacheRefresher.OnRefreshGame +=
            id => { if (id == _id) UpdateView(); };
    }

    public void OnActivate(object[] args)
    {
        _init = true;
        _id = (long)args[0];

        ResetSelections();
        UpdateView();

        _ = _cacheRefresher.RefreshGame(_id);
    }

    private void ResetSelections()
    {
        SelectedVersion = null;
        ViewMode = ViewModeEnum.Nothing;
        DownloadProgress = 0;

        _downloadingCancel?.Cancel();
        _downloadingCancel = null;
        _downloading = null;
    }

    private void UpdateView()
    {
        Game? game = _cacheProvider.GetGameOf(_id);

        Title = game?.Name ?? "Unknown";
        Description = game?.Description ?? "";
        IconUrl = game?.IconUrl;

        GameDisplayDebugJson = JsonSerializer.Serialize(game);

        Version[] versions = _cacheProvider.GetVersionsOf(_id);

        AvailableVersions.Clear();

        foreach (var version in versions
            .OrderByDescending(v => v.ReleaseDate))
        {
            AvailableVersions.Add(version);
        }

        SelectedVersion = SelectedVersion == null
            ? AvailableVersions.FirstOrDefault(v => v.IsPrimary)
            : AvailableVersions.FirstOrDefault(v => v.Id == SelectedVersion.Id);
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
            DownloadProgress = Math.Clamp(value, 0, 1));

        var cts = new CancellationTokenSource();

        _downloadingCancel = cts;
        _downloading = _execManager.Download(execInfo, progress, cts.Token);

        ViewMode = ViewModeEnum.Downloading;

        try
        {
            await _downloading;

            ViewMode = ViewModeEnum.ReadyToPlay;
        }
        catch (OperationCanceledException)
        {
            ViewMode = ViewModeEnum.Nothing;
        }
        finally
        {
            _downloadingCancel?.Cancel();
            _downloadingCancel = null;
            _downloading = null;

            UpdateViewModeFromSelectedVersion();
        }
    }

    [RelayCommand]
    private void CancelDownload()
    {
        ExecInfo? execInfo = MakeCurrentExecInfo();
        if (execInfo == null) return;

        _downloadingCancel?.Cancel();
        _downloadingCancel = null;
        _downloading = null;

        ViewMode = ViewModeEnum.Nothing;
    }

    [RelayCommand]
    private void DeleteSelectedVersion()
    {
        ExecInfo? execInfo = MakeCurrentExecInfo();
        if (execInfo == null) return;

        // TODO: Add confirmation dialog

        if (_execManager.Exists(execInfo))
            _execManager.Delete(execInfo);

        ViewMode = ViewModeEnum.Nothing;
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
            DebugBox.Show(ex.Message, "Error");
        }
    }

    private ExecInfo? MakeCurrentExecInfo()
    {
        if (SelectedVersion == null) return null;

        GameData? gameData = _cacheProvider.GetGameDataOf(_id);
        if (gameData == null) return null;

        return gameData
            .ToInfo()
            .UpgradeToExecInfo(SelectedVersion);
    }

    private void UpdateViewModeFromSelectedVersion()
    {
        if (ViewMode == ViewModeEnum.Downloading)
            return;

        ExecInfo? execInfo = MakeCurrentExecInfo();
        ViewMode = execInfo != null && _execManager.Exists(execInfo)
            ? ViewModeEnum.ReadyToPlay
            : ViewModeEnum.Nothing;
    }

    partial void OnSelectedVersionChanged(Version? value)
    {
        SelectedVersionDebugJson = value == null
            ? string.Empty
            : JsonSerializer.Serialize(value);

        UpdateViewModeFromSelectedVersion();
    }

    partial void OnViewModeChanged(ViewModeEnum value)
    {
        OnPropertyChanged(nameof(DownloadButtonText));
        OnPropertyChanged(nameof(IsDownloadEnabled));
        OnPropertyChanged(nameof(IsCancelEnabled));
        OnPropertyChanged(nameof(IsPlayEnabled));
        OnPropertyChanged(nameof(IsDeleteEnabled));
        OnPropertyChanged(nameof(IsProgressVisible));
    }
}
