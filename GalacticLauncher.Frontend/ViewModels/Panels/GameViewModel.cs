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

internal partial class GameViewModel
    : ObservableObject, INavigationAware
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelTitle))]
    private string _title = "";

    [ObservableProperty]
    private string _description = "";

    [ObservableProperty]
    private string _author = "";

    [ObservableProperty]
    private bool _anyScreenshots = false;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(InstalledVersions))]
    private bool _filterInstalledSnapshot;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredAvailableVersions))]
    private bool _filterAvailableSnapshot;

    [ObservableProperty]
    private bool _isInLibrary;

    [ObservableProperty]
    private bool _isFavorite;

    [ObservableProperty]
    private bool _isInstalledSectionExpanded;

    [ObservableProperty]
    private bool _isAvailableSectionExpanded;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SelTitle))]
    private Version? _selectedVersion;

    [ObservableProperty]
    private ImageViewModel? _banner;

    public ObservableCollection<Version> InstalledVersions { get; } = [];
    public ObservableCollection<Version> FilteredAvailableVersions { get; } = [];
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
    public string SelTitle => $"{Title} {SelectedVersion?.Caption}";

    private bool _init = false;
    private long _id = 0;

    private readonly List<Version> _allVersionsRaw = [];

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
            () => { if (_init) RunGameDataRefresh(); };

        _cacheRefresher.OnRefreshGameData +=
            id => { if (_init && _id == id) UpdateView(); };
    }

    public void OnActivate(object[] args)
    {
        _init = true;
        _id = (long)args[0];

        RunGameDataRefresh();
        InitializePreferences();
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

        ViewMode = GetAdequateViewMode();
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

        List<Version> versions = [..
            _cacheProvider.GetVersionsOf(_id)];

		_allVersionsRaw.Clear();
        _allVersionsRaw.AddRange(versions);
		
		ApplyFiltersAndCategories();

        var combinedLists = InstalledVersions
            .Concat(FilteredAvailableVersions)
            .ToList();

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
            ImageViewModel? screenshotVm = _imageFactory.CreateAndStartLoadingImage(url);
            if (screenshotVm != null)
            {
                Screenshots.Add(screenshotVm);
            }
        }

        Tags.Clear();

        List<Tag> gameTags = [.. _cacheProvider.GetTagsByGameId(_id)];

        foreach (Tag tag in gameTags)
        {
            Tags.Add(tag);
        }

        UpdateLists();
    }

    private void ApplyFiltersAndCategories()
    {
        GameData? gameData = _cacheProvider.GetGameDataOf(_id);
        if (gameData is null) return;

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
                VersionType.Snapshot => FilterInstalledSnapshot,
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
                VersionType.Snapshot => FilterAvailableSnapshot,
                _ => true
            });

        foreach (var version in availableFiltered)
        {
            FilteredAvailableVersions.Add(version);
        }
    }

    partial void OnSelectedVersionChanged(Version? value)
    {
        _preferenceManager.SetSelectedVersion(_id, SelectedVersion?.Id);

        ViewMode = GetAdequateViewMode();
    }

    [RelayCommand]
    private void ToggleLibrary()
    {
        Action<long> action = IsInLibrary
            ? _gameListManager.RemoveFromLibrary
            : _gameListManager.AddToLibrary;

        action(_id);

        UpdateLists();
    }

    [RelayCommand]
    private void ToggleFavorite()
    {
        Action<long> action = IsFavorite
            ? _gameListManager.RemoveFromFavorite
            : _gameListManager.AddToFavorite;

        action(_id);

        UpdateLists();
    }

    private void UpdateLists()
    {
        IsInLibrary = _gameListManager.GetLibraryGames().Contains(_id);
        IsFavorite = _gameListManager.GetFavoriteGames().Contains(_id);
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
        }

        ViewMode = GetAdequateViewMode();
        ApplyFiltersAndCategories();
    }

    [RelayCommand]
    private async Task DeleteSelectedVersion()
    {
        ExecInfo? execInfo = MakeCurrentExecInfo();
        if (execInfo is null) return;

        bool success = await _gamePlayService.Delete(execInfo);
        if (success)
        {
            SelectedVersion ??= _allVersionsRaw
                .FirstOrDefault(v => v.IsPrimary);

            ViewMode = GetAdequateViewMode();
            ApplyFiltersAndCategories();
        }
    }

    [RelayCommand]
    private async Task PlaySelectedVersion()
    {
        ExecInfo? execInfo = MakeCurrentExecInfo();
        if (execInfo is null) return;

        await _gamePlayService.PlayAndTerminate(execInfo);
    }

    private ViewModeEnum GetAdequateViewMode()
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
}
