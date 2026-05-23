using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GalacticLauncher.Frontend.ViewModels.ViewServices;

namespace GalacticLauncher.Frontend.ViewModels.Panels;

internal partial class HomeViewModel(INavigator navigator) : ObservableObject
{
    [RelayCommand]
    public void ShowGame(string id_)
    {
        long id = long.Parse(id_);
        navigator.NavigateTo<GameViewModel>(id);
    }
}
