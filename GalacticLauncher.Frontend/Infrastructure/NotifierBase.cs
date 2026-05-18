using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GalacticLauncher.Frontend.Infrastructure;

internal abstract class NotifierBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
