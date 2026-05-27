using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GalacticLauncher.Core.Models;

namespace GalacticLauncher.Frontend.ViewModels.Tags
{
    internal partial class TagViewModel : ObservableObject
    {
        public long Id { get; }

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _description;

        [ObservableProperty]
        private bool _isActive;

        private readonly Action<TagViewModel> _onToggle;

        public TagViewModel(Tag tag, Action<TagViewModel> onToggle)
        {
            Id = tag.Id;
            Name = tag.Name;
            Description = tag.Description;
            IsActive = false;
            _onToggle = onToggle;
        }

        [RelayCommand]
        public void ToggleTag() => _onToggle(this);
    }
}
