using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GalacticLauncher.Core;
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
using GalacticLauncher.Frontend.ViewModels.ImageLoad;
using GalacticLauncher.Frontend.ViewModels.ViewServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace GalacticLauncher.Frontend.ViewModels.Panels;

internal partial class GameViewModel : ObservableObject, INavigationAware
{
    protected const string EMPTY_STATUS = "";
    protected const string GAME_NOT_FOUND = "NO GAME";
    protected const string LOADING_IMAGE = "LOADING IMAGE...";
    protected const string IMAGE_NOT_FOUND = "IMAGE NOT FOUND";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelTitle))]
    private string _title = "";

    [ObservableProperty]
    private string _description = "";

    [ObservableProperty]
    private string _author = "";

    [ObservableProperty]
    private ImageViewModel _banner;

    public ObservableCollection<ImageViewModel> Screenshots { get; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelTitle))]
    private Version? _selectedVersion;

    [ObservableProperty]
    private double _downloadProgress;

    [ObservableProperty]
    private bool _anyScreenshots = false;

    private readonly List<Version> _allVersionsRaw = [];
    public ObservableCollection<Version> InstalledVersions { get; } = [];
    public ObservableCollection<Version> FilteredAvailableVersions { get; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(InstalledVersions))]
    private bool _filterInstalledRelease = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(InstalledVersions))]
    private bool _filterInstalledSnapshot = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(InstalledVersions))]
    private bool _filterInstalledBeta = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(InstalledVersions))]
    private bool _filterInstalledAlpha = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredAvailableVersions))]
    private bool _filterAvailableRelease = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredAvailableVersions))]
    private bool _filterAvailableSnapshot = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredAvailableVersions))]
    private bool _filterAvailableBeta = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredAvailableVersions))]
    private bool _filterAvailableAlpha = true;

    public ObservableCollection<Tag> Tags { get; } = [];

    [ObservableProperty]
    private bool _isInLibrary;

    [ObservableProperty]
    private bool _isFavorite;

    [ObservableProperty]
    private bool _isInstalledSectionExpanded = false;

    [ObservableProperty]
    private bool _isAvailableSectionExpanded = false;

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
    private readonly IGameListManager _gameListManager;
    private readonly IImageFactory _imageFactory;

    public GameViewModel(
        ICacheProvider cacheProvider,
        ICacheRefresher cacheRefresher,
        ILastGameManager lastGameManager,
        IExecManager execManager,
        IPreferenceManager preferenceManager,
        IGameListManager gameListManager,
        ITerminator terminator,
        IDialogs dialog,
        INotifications notifications,
        IImageProvider imageProvider,
        IImageFactory imageFactory
        )
    {
        _cacheProvider = cacheProvider;
        _cacheRefresher = cacheRefresher;
        _lastGameManager = lastGameManager;
        _execManager = execManager;
        _preferenceManager = preferenceManager;
        _terminator = terminator;
        _gameListManager = gameListManager;
        _dialogs = dialog;
        _notifications = notifications;
        _imageFactory = imageFactory;

        _cacheRefresher.OnInitialize +=
            () => { if (_init) RunGameDataRefresh(); };

        _cacheRefresher.OnRefreshGameData +=
            id => { if (_init && _id == id) UpdateView(); };

        Banner = _imageFactory.CreateAndStartLoadingImage("www.example.com/nothing");
    }

    public void OnActivate(object[] args)
    {
        _init = true;
        _id = (long)args[0];

        RunGameDataRefresh();

        ResetSelections();
        UpdateView();

        FilterInstalledRelease = _preferenceManager.GetFilterState(_id, "InstRelease", true);
        FilterInstalledSnapshot = _preferenceManager.GetFilterState(_id, "InstSnapshot", true);
        FilterInstalledBeta = _preferenceManager.GetFilterState(_id, "InstBeta", true);
        FilterInstalledAlpha = _preferenceManager.GetFilterState(_id, "InstAlpha", true);

        FilterAvailableRelease = _preferenceManager.GetFilterState(_id, "AvRelease", true);
        FilterAvailableSnapshot = _preferenceManager.GetFilterState(_id, "AvSnapshot", true);
        FilterAvailableBeta = _preferenceManager.GetFilterState(_id, "AvBeta", true);
        FilterAvailableAlpha = _preferenceManager.GetFilterState(_id, "AvAlpha", true);
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
        GameData? gameData = _cacheProvider.GetGameDataOf(_id);

        Title = game?.Name ?? "Unknown";
        Description = game?.Description ?? "";
        Author = game?.Author ?? "";
		
		Screenshots.Clear();
		
		long? selVersionId = _preferenceManager.GetSelectedVersion(_id);

        List<Version> versions = [.. _cacheProvider.GetVersionsOf(_id)];
		_allVersionsRaw.Clear();
        _allVersionsRaw.AddRange(versions);
		
		ApplyFiltersAndCategories(gameData);
		
		AvailableVersions.Clear();

        var combinedLists = InstalledVersions.Concat(FilteredAvailableVersions).ToList();
        SelectedVersion = selVersionId == null
            ? combinedLists.FirstOrDefault(v => v.IsPrimary) ?? combinedLists.FirstOrDefault()
            : combinedLists.FirstOrDefault(v => v.Id == selVersionId);

        var screenshotsUrls = gameData?.Images?
            .Where(img => img.Type == ImageType.Screenshot)
            .OrderBy(img => img.SortIndex)
            .Select(img => img.DownloadUrl) ?? [];

        AnyScreenshots = screenshotsUrls.Any();

        string? bannerUrl = gameData?.Images?
            .FirstOrDefault(img => img.Type == ImageType.Banner)?
            .DownloadUrl;

        Banner = _imageFactory.CreateAndStartLoadingImage(bannerUrl);

        foreach (string url in screenshotsUrls)
        {
            ImageViewModel? screenshotVM = _imageFactory.CreateAndStartLoadingImage(url);
            if (screenshotVM != null)
            {
                Screenshots.Add(screenshotVM);
            }
        }

        Tags.Clear();

        List<Tag> gameTags = [..
            _cacheProvider.GetTagsByGameId(_id)];

        foreach (Tag tag in gameTags)
        {
            Tags.Add(tag);
        }

        IsInLibrary = _gameListManager.GetLibraryGames().Contains(_id);
        IsFavorite = _gameListManager.GetFavoriteGames().Contains(_id);
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

			_gameListManager.AddToLibrary(_id);

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
            RefreshFilteredLists();
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
            RefreshFilteredLists();
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

    [RelayCommand]
    private void ToggleLibrary()
    {
        if (IsInLibrary)
        {
            _gameListManager.RemoveFromLibrary(_id);
            IsInLibrary = false;
            IsFavorite = false;
        }
        else
        {
            _gameListManager.AddToLibrary(_id);
            IsInLibrary = true;
        }
    }

    [RelayCommand]
    private void ToggleFavorite()
    {
        if (IsFavorite)
        {
            _gameListManager.RemoveFromFavorite(_id);
            IsFavorite = false;
        }
        else
        {
            _gameListManager.AddToFavorite(_id);
            IsFavorite = true;
            IsInLibrary = true;
        }
    }

    private void RefreshFilteredLists()
    {
        GameData? gameData = _cacheProvider.GetGameDataOf(_id);
        ApplyFiltersAndCategories(gameData);
    }

    private void ApplyFiltersAndCategories(GameData? gameData)
    {
        if (gameData == null) return;

        InstalledVersions.Clear();
        FilteredAvailableVersions.Clear();

        var categorizedVersions = _allVersionsRaw.Select(v => new
        {
            Version = v,
            IsInstalled = gameData.ToExecInfo(v) is { } execInfo && _execManager.Exists(execInfo)
        }).ToList();

        var installedFiltered = categorizedVersions
            .Where(x => x.IsInstalled)
            .Select(x => x.Version)
            .Where(v => v.Type switch
            {
                VersionType.Release => FilterInstalledRelease,
                VersionType.Snapshot => FilterInstalledSnapshot,
                VersionType.Beta => FilterInstalledBeta,
                _ => true
            });

        foreach (var version in installedFiltered)
        {
            InstalledVersions.Add(version);
        }

        var availableFiltered = categorizedVersions
            .Select(x => x.Version)
            .Where(v => v.Type switch
            {
                VersionType.Release => FilterAvailableRelease,
                VersionType.Snapshot => FilterAvailableSnapshot,
                VersionType.Beta => FilterAvailableBeta,
                _ => true
            });

        foreach (var version in availableFiltered)
        {
            FilteredAvailableVersions.Add(version);
        }
    }

    partial void OnFilterInstalledReleaseChanged(bool value)
    {
        _preferenceManager.SetFilterState(_id, "InstRelease", value);
        RefreshFilteredLists();
    }

    partial void OnFilterInstalledSnapshotChanged(bool value)
    {
        _preferenceManager.SetFilterState(_id, "InstSnapshot", value);
        RefreshFilteredLists();
    }

    partial void OnFilterInstalledBetaChanged(bool value)
    {
        _preferenceManager.SetFilterState(_id, "InstBeta", value);
        RefreshFilteredLists();
    }

    partial void OnFilterInstalledAlphaChanged(bool value)
    {
        _preferenceManager.SetFilterState(_id, "InstAlpha", value);
        RefreshFilteredLists();
    }

    partial void OnFilterAvailableReleaseChanged(bool value)
    {
        _preferenceManager.SetFilterState(_id, "AvRelease", value);
        RefreshFilteredLists();
    }

    partial void OnFilterAvailableSnapshotChanged(bool value)
    {
        _preferenceManager.SetFilterState(_id, "AvSnapshot", value);
        RefreshFilteredLists();
    }

    partial void OnFilterAvailableBetaChanged(bool value)
    {
        _preferenceManager.SetFilterState(_id, "AvBeta", value);
        RefreshFilteredLists();
    }

    partial void OnFilterAvailableAlphaChanged(bool value)
    {
        _preferenceManager.SetFilterState(_id, "AvAlpha", value);
        RefreshFilteredLists();
    }
}
