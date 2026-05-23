using System;

namespace GalacticLauncher.Frontend.ViewModels.ViewServices;

public interface INavigator
{
    event Action<Type, object[]>? OnNavigate;
    event Action<Type, object[]>? OnGamesPanelNavigate;
    event Action<Type, object[]>? OnAdminPanelNavigate;

    void NavigateTo<T>(params object[] args);
    void GamesPanelNavigateTo<T>(params object[] args);
    void AdminPanelNavigateTo<T>(params object[] args);
}

internal class Navigator : INavigator
{
    public event Action<Type, object[]>? OnNavigate;
    public event Action<Type, object[]>? OnGamesPanelNavigate;
    public event Action<Type, object[]>? OnAdminPanelNavigate;

    public void NavigateTo<T>(params object[] args)
    {
        OnNavigate?.Invoke(typeof(T), args);
    }

    public void GamesPanelNavigateTo<T>(params object[] args)
    {
        OnGamesPanelNavigate?.Invoke(typeof(T), args);
    }

    public void AdminPanelNavigateTo<T>(params object[] args)
    {
        OnAdminPanelNavigate?.Invoke(typeof(T), args);
    }
}
