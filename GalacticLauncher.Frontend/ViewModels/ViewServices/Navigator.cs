using System;

namespace GalacticLauncher.Frontend.ViewModels.ViewServices;

public interface INavigator
{
    event Action<Type>? OnNavigate;
    event Action<Type>? OnAdminPanelNavigate;
    void NavigateTo<T>();
    void AdminPanelNavigateTo<T>();
}

internal class Navigator : INavigator
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
