using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GalacticLauncher.Frontend.ViewModels.Dialogs;

internal partial class ConfirmationDialogViewModel(string title, string message) : DialogViewModel<bool>
{
    [ObservableProperty]
    private string _title = title;

    [ObservableProperty]
    private string _message = message;

    [RelayCommand]
    private void Confirm() => Close(true);

    [RelayCommand]
    private void Cancel() => Close(false);
}