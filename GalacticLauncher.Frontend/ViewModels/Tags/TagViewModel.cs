using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GalacticLauncher.Core.Models;

namespace GalacticLauncher.Frontend.ViewModels.Tags;

internal partial class TagViewModel(
    Tag tag,
    Action<TagViewModel> onToggle) : ObservableObject
{
    [ObservableProperty]
    private string _name = tag.Name;

    [ObservableProperty]
    private string _description = tag.Description;

    [ObservableProperty]
    private bool _isActive;

    public long Id { get; } = tag.Id;

    [RelayCommand]
    public void ToggleTag()
    {
        onToggle.Invoke(this);
    }
}
