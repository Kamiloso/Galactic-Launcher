using System;
using Avalonia.Controls.Notifications;

namespace GalacticLauncher.Frontend.Services
{
    public interface INotificationService
    {
        void ShowError(string title, string message);
        void ShowSuccess(string title, string message);
        void RegisterManager(WindowNotificationManager manager);
    }

    internal class Notifications : INotificationService
    {
        private WindowNotificationManager? _manager;

        public void RegisterManager(WindowNotificationManager manager)
        {
            _manager = manager;
        }

        public void ShowError(string title, string message)
        {
            _manager?.Show(new Notification(title, message, NotificationType.Error, TimeSpan.FromSeconds(4)));
        }

        public void ShowSuccess(string title, string message)
        {
            _manager?.Show(new Notification(title, message, NotificationType.Success, TimeSpan.FromSeconds(4)));
        }
    }
}
