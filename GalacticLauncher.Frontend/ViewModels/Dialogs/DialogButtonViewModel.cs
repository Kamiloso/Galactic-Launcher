using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace GalacticLauncher.Frontend.ViewModels.Dialogs;

internal partial class DialogButtonViewModel(string text, bool isHighlighted) : ObservableObject
{
    [ObservableProperty]
    private bool _interactable = true;

    public string Text { get; } = text;
    public bool IsHighlighted { get; } = isHighlighted;

    public event Action? OnClick;

    internal enum ButtonAlignment
    {
        Left,
        Center,
        Right
    }

    [ObservableProperty]
    private ButtonAlignment _alignment = ButtonAlignment.Right;

    [RelayCommand]
    void Click()
    {
        OnClick?.Invoke();
    }

    public void MoveToLeft() =>
        Alignment = ButtonAlignment.Left;

    public void MoveToCenter() =>
        Alignment = ButtonAlignment.Center;

    public void MoveToRight() =>
        Alignment = ButtonAlignment.Right;
}
