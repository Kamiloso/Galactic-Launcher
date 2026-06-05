using CommunityToolkit.Mvvm.ComponentModel;

namespace GalacticLauncher.Frontend.ViewModels.Dialogs;

internal partial class TextInputViewModel(
    string watermark, string label) : ObservableObject
{
    [ObservableProperty]
    private string _text = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PasswordChar))]
    private bool _isPassword = false;

    public char PasswordChar => IsPassword ? '*' : '\0';

    public string Watermark { get; } = watermark;
    public string Label { get; } = label;
}
