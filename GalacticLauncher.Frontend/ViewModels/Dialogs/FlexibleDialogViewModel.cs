using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GalacticLauncher.Frontend.ViewModels.Dialogs;

internal partial class FlexibleDialogViewModel(string title, string message) : DialogViewModel<object?>
{
    [ObservableProperty]
    private string _title = title;

    [ObservableProperty]
    private string _message = message;
    
    public ObservableCollection<TextInputViewModel> Inputs { get; } = [];
    public ObservableCollection<DialogButtonViewModel> Buttons { get; } = [];
    
    public TextInputViewModel AddInput(string watermark, string label, bool isPassword = false)
    {
        var tvm = new TextInputViewModel(watermark, label, isPassword);

        Inputs.Add(tvm);
        return tvm;
    }

    public DialogButtonViewModel AddButton(string text, object? returnValue, bool isHighlighted = false)
    {
        if (Buttons.Count >= 3)
            throw new InvalidOperationException("Max buttons exceeded.");

        DialogButtonViewModel dvm = new(text, isHighlighted);
        dvm.OnClick += () => Close(returnValue);

        Buttons.Add(dvm);

        if (Buttons.Count == 1)
        {
            Buttons[0].MoveToRight();
        }
        
        if (Buttons.Count == 2)
        {
            Buttons[0].MoveToLeft();
            Buttons[1].MoveToRight();
        }
        
        if (Buttons.Count == 3)
        {
            Buttons[0].MoveToLeft();
            Buttons[1].MoveToCenter();
            Buttons[2].MoveToRight();
        }

        return dvm;
    }
}