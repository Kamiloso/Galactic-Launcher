using System;
using Avalonia.Controls.Notifications;

namespace GalacticLauncher.Frontend.ViewModels.ViewServices;

public interface INotifications
{
    void ShowInfo(string title, string message);
    void ShowWarning(string title, string message);
    void ShowError(string title, string message);
    void ShowSuccess(string title, string message);
}

internal class Notifications(
    WindowNotificationManager notifications) : INotifications
{
    public void ShowInfo(string title, string message) => Show(NotificationType.Information, title, message);
    public void ShowWarning(string title, string message) => Show(NotificationType.Warning, title, message);
    public void ShowError(string title, string message) => Show(NotificationType.Error, title, message);
    public void ShowSuccess(string title, string message) => Show(NotificationType.Success, title, message);

    private void Show(NotificationType type, string title, string message)
    {
        Notification notification = new(
            title, message, type,
            TimeSpan.FromSeconds(4));

        notifications?.Show(notification);
    }
}
