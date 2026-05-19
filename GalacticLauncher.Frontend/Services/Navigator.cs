using System;

namespace GalacticLauncher.Frontend.Services;

internal class Navigator
{
    public event Action<Type>? OnNavigate;
    public event Action<Type>? OnAdminPanelNavigate;

    public void NavigateTo<T>()
    {
        OnNavigate?.Invoke(typeof(T));
    }

    public void AdminPanelNavigateTo<T>()
    {
        OnAdminPanelNavigate?.Invoke(typeof(T));
    }
}
