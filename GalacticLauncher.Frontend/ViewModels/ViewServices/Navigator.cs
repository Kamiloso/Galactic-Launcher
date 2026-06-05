using System;

namespace GalacticLauncher.Frontend.ViewModels.ViewServices;

public interface INavigator
{
    Type PageType { get; }
    Type AdminPanelPageType { get; }

    event Action<Type, object[]>? OnNavigate;
    event Action<Type, object[]>? OnAdminPanelNavigate;

    void NavigateTo<T>(params object[] args);
    void AdminPanelNavigateTo<T>(params object[] args);
}

internal class Navigator : INavigator
{
    public Type PageType { get; private set; } = typeof(object);
    public Type AdminPanelPageType { get; private set; } = typeof(object);

    public event Action<Type, object[]>? OnNavigate;
    public event Action<Type, object[]>? OnAdminPanelNavigate;

    public void NavigateTo<T>(params object[] args)
    {
        PageType = typeof(T);
        OnNavigate?.Invoke(typeof(T), args);
    }

    public void AdminPanelNavigateTo<T>(params object[] args)
    {
        AdminPanelPageType = typeof(T);
        OnAdminPanelNavigate?.Invoke(typeof(T), args);
    }
}
