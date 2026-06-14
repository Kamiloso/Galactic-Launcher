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

public partial class GameViewModel : ObservableObject, INavigationAware
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(InLibrary))]
    [NotifyPropertyChangedFor(nameof(InFavorite))]
    private long _id;

    [ObservableProperty]
    private string _title = "";

    [ObservableProperty]
    private string _description = "";

    [ObservableProperty]
    private string _author = "";

    [ObservableProperty]
    private bool _isInstalledSectionExpanded;

    [ObservableProperty]
    private bool _isAvailableSectionExpanded;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(InstalledVersions))]
    private bool _filterInstalledSnapshot;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AvailableVersions))]
    private bool _filterAvailableSnapshot;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ExecInfo))]
    [NotifyPropertyChangedFor(nameof(IsNoInstanceState))]
    [NotifyPropertyChangedFor(nameof(IsReadyToPlayState))]
    private Version? _selectedVersion;

    [ObservableProperty]
    private ImageViewModel? _banner;

    public ObservableCollection<Version> InstalledVersions { get; } = [];
    public ObservableCollection<Version> AvailableVersions { get; } = [];

    public ObservableCollection<ImageViewModel> Screenshots { get; } = [];
    public ObservableCollection<Tag> Tags { get; } = [];

    public ExecInfo? ExecInfo => SelectedVersion is { } ? _game.ToExecInfo(SelectedVersion) : null;

    public bool IsNoInstanceState => ExecInfo is { } && !_execManager.Exists(ExecInfo);
    public bool IsReadyToPlayState => ExecInfo is { } && _execManager.Exists(ExecInfo);

    public bool InLibrary => _gameListManager.InLibrary(Id);
    public bool InFavorite => _gameListManager.InFavorite(Id);

    private Game _game;
    private GameData? _gameData;
    private List<Version> _versions = [];
    private List<Tag> _tags = [];
        
    private readonly ICacheProvider _cacheProvider;
    private readonly ICacheRefresher _cacheRefresher;
    private readonly IExecManager _execManager;
    private readonly IPreferenceManager _preferenceManager;
    private readonly IGamePlayService _gamePlayService;
    private readonly IGameListManager _gameListManager;
    private readonly IImageFactory _imageFactory;
    private readonly ILastGameManager _lastGameManager;

    public GameViewModel(
        ICacheProvider cacheProvider,
        ICacheRefresher cacheRefresher,
        IExecManager execManager,
        IPreferenceManager preferenceManager,
        IGamePlayService gamePlayService,
        IGameListManager gameListManager,
        IImageFactory imageFactory,
        ILastGameManager lastGameManager)
    {
        _cacheProvider = cacheProvider;
        _cacheRefresher = cacheRefresher;
        _execManager = execManager;
        _preferenceManager = preferenceManager;
        _gamePlayService = gamePlayService;
        _gameListManager = gameListManager;
        _imageFactory = imageFactory;
        _lastGameManager = lastGameManager;

        _cacheRefresher.OnInitialize +=
            () => { if (Id != 0) FireGameDataQuery(); };

        _cacheRefresher.OnRefreshGameData +=
            id => { if (Id != 0 && Id == id) UpdateView(); };

        _gameListManager.OnListsChanged += () =>
        {
            OnPropertyChanged(nameof(InLibrary));
            OnPropertyChanged(nameof(InFavorite));
        };

        _game = Game.GetFallback(Id);
    }

    private void FireGameDataQuery()
    {
        _ = _cacheRefresher.RefreshGameDataAsync(Id);
    }

    public void OnActivate(object[] args)
    {
        Id = (long)args[0];

        InitializePreferences();
        FireGameDataQuery();

        UpdateView();
    }

    private void UpdateView()
    {
        PrepareObjects();

        RefreshBasicInfo();
        RefreshListsAndSelection();
        RefreshBanner();
        RefreshScreenshots();
        RefreshTags();
    }

    private void PrepareObjects()
    {
        _game = _cacheProvider.GetGameOf(Id) ?? Game.GetFallback(Id);
        _gameData = _cacheProvider.GetGameDataOf(Id);

        _versions = [.. _cacheProvider.GetVersionsOf(Id)];
        _tags = [.. _cacheProvider.GetTagsOf(Id)];
    }

    private void RefreshBasicInfo()
    {
        Title = _game.Name;
        Description = _game.Description;
        Author = _game.Author;
    }

    private void RefreshListsAndSelection()
    {
        long? oldVersionId = _preferenceManager.GetSelectedVersion(Id);

        UpdateObservableVersions(InstalledVersions);
        UpdateObservableVersions(AvailableVersions);

        SelectedVersion = oldVersionId is null
            ? _versions.FirstOrDefault(v => v.IsPrimary)
            : _versions.FirstOrDefault(v => v.Id == oldVersionId);

        SelectedVersion ??= _versions.FirstOrDefault();
    }

    private void UpdateObservableVersions(ObservableCollection<Version> observableVersions)
    {
        bool showInstalled = ReferenceEquals(InstalledVersions, observableVersions);
        bool showAvailable = ReferenceEquals(AvailableVersions, observableVersions);

        observableVersions.Clear();

        foreach (Version version in _versions)
        {
            ExecInfo execInfo = _game.ToExecInfo(version);
            bool exists = _execManager.Exists(execInfo);

            if ((exists && showInstalled) || (!exists && showAvailable))
            {
                observableVersions.Add(version);
            }
        }
    }

    private void RefreshBanner()
    {
        Banner = null;

        if (_gameData is null) return;

        List<string> bannerUrls = [.. _gameData.Images
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

        if (_gameData is null) return;

        List<string> screenshotUrls = [.. _gameData.Images
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

        foreach (Tag tag in _tags)
        {
            Tags.Add(tag);
        }
    }

    [RelayCommand]
    private async Task DownloadSelectedVersion()
    {
        bool success = IsNoInstanceState &&
            await _gamePlayService.Download(ExecInfo!);

        if (success)
        {
            _gameListManager.AddToLibrary(Id);

            RefreshListsAndSelection();
        }
    }

    [RelayCommand]
    private async Task DeleteSelectedVersion()
    {
        bool success = IsReadyToPlayState &&
            await _gamePlayService.Delete(ExecInfo!);

        if (success)
        {
            RefreshListsAndSelection();
        }
    }

    [RelayCommand]
    private async Task PlaySelectedVersion()
    {
        bool success = IsReadyToPlayState &&
            await _gamePlayService.PlayAndTerminate(ExecInfo!);

        if (success)
        {
            _lastGameManager.SetLastGame(Id);
        }
    }

    [RelayCommand]
    private void ToggleLibrary()
    {
        Action<long> action = InLibrary
            ? _gameListManager.RemoveFromLibrary
            : _gameListManager.AddToLibrary;

        action(Id);
    }

    [RelayCommand]
    private void ToggleFavorite()
    {
        Action<long> action = InFavorite
            ? _gameListManager.RemoveFromFavorite
            : _gameListManager.AddToFavorite;

        action(Id);
    }

    #region Preferences

    private const string INS_SNAPSHOT = "ins-snapshot";
    private const string AVB_SNAPSHOT = "avb-snapshot";

    private const string INS_EXPANDED = "ins-expanded";
    private const string AVB_EXPANDED = "avb-expanded";

    private void InitializePreferences()
    {
        FilterInstalledSnapshot = _preferenceManager.GetGameBool(Id, INS_SNAPSHOT, true);
        FilterAvailableSnapshot = _preferenceManager.GetGameBool(Id, AVB_SNAPSHOT, false);

        IsInstalledSectionExpanded = _preferenceManager.GetGameBool(Id, INS_EXPANDED, true);
        IsAvailableSectionExpanded = _preferenceManager.GetGameBool(Id, AVB_EXPANDED, true);
    }

    partial void OnFilterInstalledSnapshotChanged(bool value)
    {
        _preferenceManager.SetGameBool(Id, INS_SNAPSHOT, value);

        RefreshListsAndSelection();
    }

    partial void OnFilterAvailableSnapshotChanged(bool value)
    {
        _preferenceManager.SetGameBool(Id, AVB_SNAPSHOT, value);

        RefreshListsAndSelection();
    }

    partial void OnIsInstalledSectionExpandedChanged(bool value)
    {
        _preferenceManager.SetGameBool(Id, INS_EXPANDED, value);
    }

    partial void OnIsAvailableSectionExpandedChanged(bool value)
    {
        _preferenceManager.SetGameBool(Id, AVB_EXPANDED, value);
    }

    partial void OnSelectedVersionChanged(Version? value)
    {
        _preferenceManager.SetSelectedVersion(Id, value?.Id);
    }

    #endregion
}
