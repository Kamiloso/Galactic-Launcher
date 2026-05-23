using System;
using Avalonia.Controls.Notifications;

namespace GalacticLauncher.Frontend.ViewModels.ViewServices
{
    public interface INotifications
    {
        void RegisterManager(WindowNotificationManager manager);
        void ShowInfo(string title, string message);
        void ShowWarning(string title, string message);
        void ShowError(string title, string message);
        void ShowSuccess(string title, string message);
    }

    internal class Notifications : INotifications
    {
        // primary constructors - see

        // TODO: Magda
        // Make this independent from Avalonia, since
        // it is a View Service, and should not be tightly coupled to the UI framework.

        private WindowNotificationManager? _manager;

        public void RegisterManager(WindowNotificationManager manager)
        {
            _manager = manager;
        }

        public void ShowInfo(string title, string message) => Show(NotificationType.Information, title, message);
        public void ShowWarning(string title, string message) => Show(NotificationType.Warning, title, message);
        public void ShowError(string title, string message) => Show(NotificationType.Error, title, message);
        public void ShowSuccess(string title, string message) => Show(NotificationType.Success, title, message);

        private void Show(NotificationType type, string title, string message)
        {
            _manager?.Show(
                new Notification(
                    title, message, type,
                    TimeSpan.FromSeconds(4)));
        }
    }
}
