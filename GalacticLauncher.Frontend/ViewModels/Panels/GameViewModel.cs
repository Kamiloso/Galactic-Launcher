using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GalacticLauncher.Frontend.Domain.Models;
using GalacticLauncher.Frontend.Infrastructure;
using GalacticLauncher.Frontend.Services.Cache;
using GalacticLauncher.Frontend.Services.Executables;
using System.Collections.ObjectModel;
using System.Linq;

namespace GalacticLauncher.Frontend.ViewModels.Panels;

internal partial class GameViewModel : ObservableObject, INavigationAware
{
    [ObservableProperty]
    private string _title = "";

    [ObservableProperty]
    private string _description = "";

    [ObservableProperty]
    private string? _iconUrl = null;

    public ObservableCollection<VersionDisplay> AvailableVersions { get; } = [];

    [ObservableProperty]
    private VersionDisplay? _selectedVersion;

    private bool _init = false;
    private long _id = 0;

    private readonly ICacheProvider _cacheProvider;
    private readonly ICacheRefresher _cacheRefresher;
    private readonly IExecManager _execManager;

    public GameViewModel(
        ICacheProvider cacheProvider,
        ICacheRefresher cacheRefresher,
        IExecManager execManager)
    {
        _cacheProvider = cacheProvider;
        _cacheRefresher = cacheRefresher;
        _execManager = execManager;

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
    }

    private void UpdateView()
    {
        GameDisplay game = _cacheProvider.GetDisplayOf(_id);
        VersionDisplay[] versions = _cacheProvider.GetVersionDisplaysOf(_id);

        Title = game.Title;
        Description = game.Description;
        IconUrl = game.IconUrl;

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
    private void PlaySelectedVersion()
    {
        DebugBox.Show($"Play: game = {Title}, version = {SelectedVersion?.Caption}");
    }
}
