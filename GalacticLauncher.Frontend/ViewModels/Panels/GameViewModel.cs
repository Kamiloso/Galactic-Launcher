using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GalacticLauncher.Core;
using GalacticLauncher.Core.Models;
using GalacticLauncher.Frontend.Domain.Models;
using GalacticLauncher.Frontend.Domain.Models.Extensions;
using GalacticLauncher.Frontend.Infrastructure;
using GalacticLauncher.Frontend.Services.Cache;
using GalacticLauncher.Frontend.Services.Data;
using GalacticLauncher.Frontend.Services.Executables;
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
    [ObservableProperty]
    private string _title = "";

    [ObservableProperty]
    private string _description = "";

    [ObservableProperty]
    private string _author = "";

    [ObservableProperty]
    private bool _isInLibrary;

    [ObservableProperty]
    private bool _isFavorite;

    [ObservableProperty]
    private bool _isInstalledSectionExpanded;

    [ObservableProperty]
    private bool _isAvailableSectionExpanded;

    [ObservableProperty]
    private Version? _selectedVersion;

    [ObservableProperty]
    private ImageViewModel? _banner;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(InstalledVersions))]
    private bool _filterInstalledSnapshot;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AvailableVersions))]
    private bool _filterAvailableSnapshot;

    public ObservableCollection<Version> InstalledVersions { get; } = [];
    public ObservableCollection<Version> AvailableVersions { get; } = [];

    public ObservableCollection<ImageViewModel> Screenshots { get; } = [];
    public ObservableCollection<Tag> Tags { get; } = [];

    public enum ViewModeEnum
    {
        Locked = 0,
        NoInstance = 1,
        Downloading = 2,
        ReadyToPlay = 3,
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNoInstanceState))]
    [NotifyPropertyChangedFor(nameof(IsReadyToPlayState))]
    private ViewModeEnum _viewMode = ViewModeEnum.Locked;

    public bool IsNoInstanceState => ViewMode == ViewModeEnum.NoInstance;
    public bool IsReadyToPlayState => ViewMode == ViewModeEnum.ReadyToPlay;

    private bool _init = false;
    private long _id = 0;

    private List<Version> _allVersions = [];

    private readonly ICacheProvider _cacheProvider;
    private readonly ICacheRefresher _cacheRefresher;
    private readonly IExecManager _execManager;
    private readonly IPreferenceManager _preferenceManager;
    private readonly IGamePlayService _gamePlayService;
    private readonly IGameListManager _gameListManager;
    private readonly IImageFactory _imageFactory;

    public GameViewModel(
        ICacheProvider cacheProvider,
        ICacheRefresher cacheRefresher,
        IExecManager execManager,
        IPreferenceManager preferenceManager,
        IGamePlayService gamePlayService,
        IGameListManager gameListManager,
        IImageFactory imageFactory)
    {
        _cacheProvider = cacheProvider;
        _cacheRefresher = cacheRefresher;
        _execManager = execManager;
        _preferenceManager = preferenceManager;
        _gamePlayService = gamePlayService;
        _gameListManager = gameListManager;
        _imageFactory = imageFactory;

        _cacheRefresher.OnInitialize +=
            () => { if (_init) _ = _cacheRefresher.RefreshGameDataAsync(_id); };

        _cacheRefresher.OnRefreshGameData +=
            id => { if (_init && _id == id) UpdateView(); };
    }

    public void OnActivate(object[] args)
    {
        _init = true;
        _id = (long)args[0];

        _cacheRefresher.RefreshGameDataAsync(_id);

        InitializePreferences();
        UpdateView();
    }

    private void UpdateView()
    {
        RefreshBasicInfo();
        RefreshListsAndSelection();
        RefreshBanner();
        RefreshScreenshots();
        RefreshTags();

        RefreshGameLists();
    }

    private void RefreshBasicInfo()
    {
        Game? game = _cacheProvider.GetGameOf(_id);

        Title = game?.Name ?? "Unknown";
        Description = game?.Description ?? "";
        Author = game?.Author ?? "";
    }

    private void RefreshListsAndSelection()
    {
        long? oldVersionId = _preferenceManager.GetSelectedVersion(_id);

        _allVersions = [.. _cacheProvider.GetVersionsOf(_id)];

        UpdateObservableVersions(InstalledVersions);
        UpdateObservableVersions(AvailableVersions);

        SelectedVersion = oldVersionId is null
            ? _allVersions.FirstOrDefault(v => v.IsPrimary)
            : _allVersions.FirstOrDefault(v => v.Id == oldVersionId);

        SelectedVersion ??= _allVersions.FirstOrDefault();

        ViewMode = AdequateViewMode();
    }

    private void RefreshBanner()
    {
        Banner = null;

        GameData? gameData = _cacheProvider.GetGameDataOf(_id);
        if (gameData is null) return;

        List<string> bannerUrls = [.. gameData.Images
            .Where(i => i.Type == ImageType.Banner)
            .Select(i => i.DownloadUrl)];

        ImageViewModel bannerViewModel =
            _imageFactory.CreateAndStartLoadingImage(
                bannerUrls.FirstOrDefault());

        Banner = bannerViewModel;
    }

    private void RefreshScreenshots()
    {
        Screenshots.Clear();

        GameData? gameData = _cacheProvider.GetGameDataOf(_id);
        if (gameData is null) return;

        List<string> screenshotUrls = [.. gameData.Images
            .Where(i => i.Type == ImageType.Screenshot)
            .OrderBy(i => i.SortIndex)
            .Select(i => i.DownloadUrl) ?? []];

        List<ImageViewModel> screenshotViewModels = [.. screenshotUrls
            .Select(url => _imageFactory.CreateAndStartLoadingImage(url))
            .Where(i => i != null)];

        foreach (var scvm in screenshotViewModels)
        {
            Screenshots.Add(scvm);
        }
    }

    private void RefreshTags()
    {
        Tags.Clear();

        _cacheProvider.GetTagsByGameId(_id)
            .ToList()
            .ForEach(Tags.Add);
    }

    private void UpdateObservableVersions(ObservableCollection<Version> observableVersions)
    {
        bool showInstalled = ReferenceEquals(InstalledVersions, observableVersions);
        bool showAvailable = ReferenceEquals(AvailableVersions, observableVersions);

        observableVersions.Clear();

        Game? game = _cacheProvider.GetGameOf(_id);

        foreach (Version version in _allVersions)
        {
            ExecInfo? execInfo = game?.ToExecInfo(version);
            if (execInfo is null) continue;

            bool exists = _execManager.Exists(execInfo);

            if ((exists && showInstalled) || (!exists && showAvailable))
            {
                observableVersions.Add(version);
            }
        }
    }

    [RelayCommand]
    private async Task DownloadSelectedVersion()
    {
        ExecInfo? execInfo = MakeCurrentExecInfo();
        if (execInfo == null) return;

        bool success = await _gamePlayService.Download(execInfo);
        if (success)
        {
            _gameListManager.AddToLibrary(_id);

            RefreshListsAndSelection();
            RefreshGameLists();
        }
    }

    [RelayCommand]
    private async Task DeleteSelectedVersion()
    {
        ExecInfo? execInfo = MakeCurrentExecInfo();
        if (execInfo is null) return;

        bool success = await _gamePlayService.Delete(execInfo);
        if (success)
        {
            RefreshListsAndSelection();
        }
    }

    [RelayCommand]
    private async Task PlaySelectedVersion()
    {
        ExecInfo? execInfo = MakeCurrentExecInfo();
        if (execInfo is null) return;

        await _gamePlayService.PlayAndTerminate(execInfo);
    }

    partial void OnSelectedVersionChanged(Version? value)
    {
        _preferenceManager.SetSelectedVersion(_id, SelectedVersion?.Id);

        ViewMode = AdequateViewMode();
    }

    private ViewModeEnum AdequateViewMode()
    {
        if (SelectedVersion is null)
            return ViewModeEnum.Locked;

        if (MakeCurrentExecInfo() is { } execInfo && _execManager.Exists(execInfo))
            return ViewModeEnum.ReadyToPlay;

        return ViewModeEnum.NoInstance;
    }

    private ExecInfo? MakeCurrentExecInfo()
    {
        if (SelectedVersion is null)
            return null;

        return _cacheProvider
            .GetGameDataOf(_id)?
            .ToExecInfo(SelectedVersion);
    }

    // -----

    [RelayCommand]
    private void ToggleLibrary()
    {
        Action<long> action = IsInLibrary
            ? _gameListManager.RemoveFromLibrary
            : _gameListManager.AddToLibrary;

        action(_id);

        RefreshGameLists();
    }

    [RelayCommand]
    private void ToggleFavorite()
    {
        Action<long> action = IsFavorite
            ? _gameListManager.RemoveFromFavorite
            : _gameListManager.AddToFavorite;

        action(_id);

        RefreshGameLists();
    }

    private void RefreshGameLists()
    {
        IsInLibrary = _gameListManager.GetLibraryGames().Contains(_id);
        IsFavorite = _gameListManager.GetFavoriteGames().Contains(_id);
    }
}
