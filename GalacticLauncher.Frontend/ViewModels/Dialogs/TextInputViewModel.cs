using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace GalacticLauncher.Frontend.ViewModels.Dialogs;

internal partial class TextInputViewModel : ObservableObject
{
    [ObservableProperty]
    private string _text = string.Empty;
    public string Watermark { get; }
    public string Label { get; }

    public TextInputViewModel(string watermark = "", string label = "")
    {
        Watermark = watermark;
        Label = label;
    }
}