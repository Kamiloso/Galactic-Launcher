using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GalacticLauncher.Frontend.ViewModels.Dialogs;

internal partial class FlexibleDialogViewModel : DialogViewModel<object?>
{
    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private string _message;
    
    
    public ObservableCollection<TextInputViewModel> Inputs { get; } = [];
    public ObservableCollection<DialogButtonViewModel> Buttons { get; } = [];

    public FlexibleDialogViewModel(string title, string message)
    {
        Title = title;
        Message = message;
    }
    
    public void AddInput(string watermark = "", string label = "")
    {
        Inputs.Add(new TextInputViewModel(watermark, label));
    }

    public void AddButton(string text, object? returnValue, bool isHighlighted = false)
    {
        if (Buttons.Count >= 3) return; 

        Buttons.Add(new DialogButtonViewModel(text, isHighlighted, returnValue, Close));

        if (Buttons.Count == 1)
        {
            Buttons[0].Alignment = ButtonAlignment.Right;
        }
        else if (Buttons.Count == 2)
        {
            Buttons[0].Alignment = ButtonAlignment.Left;
            Buttons[1].Alignment = ButtonAlignment.Right;
        }
        else if (Buttons.Count == 3)
        {
            Buttons[0].Alignment = ButtonAlignment.Left;
            Buttons[1].Alignment = ButtonAlignment.Center;
            Buttons[2].Alignment = ButtonAlignment.Right;
        }
    }
}