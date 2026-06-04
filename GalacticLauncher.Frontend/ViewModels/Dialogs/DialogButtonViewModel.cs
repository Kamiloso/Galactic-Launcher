using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GalacticLauncher.Frontend.ViewModels.Dialogs;

public enum ButtonAlignment
{
    Left,
    Center,
    Right
}

internal partial class DialogButtonViewModel : ObservableObject
{
    public string Text { get; }
    public bool IsHighlighted { get; }
    public IRelayCommand ClickCommand { get; }

    [ObservableProperty]
    private ButtonAlignment _alignment = ButtonAlignment.Right;

    public DialogButtonViewModel(string text, bool isHighlighted, object? returnValue, Action<object?> closeAction)
    {
        Text = text;
        IsHighlighted = isHighlighted;
        ClickCommand = new RelayCommand(() => closeAction(returnValue));
    }
}