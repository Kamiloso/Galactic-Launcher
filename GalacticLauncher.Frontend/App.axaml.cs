global using Version = GalacticLauncher.Core.Models.Version;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GalacticLauncher.Frontend.Infrastructure.Client;
using GalacticLauncher.Frontend.Services.Executables;
using GalacticLauncher.Frontend.ViewModels.AdminPanels;
using GalacticLauncher.Frontend.ViewModels.Panels;
using GalacticLauncher.Frontend.ViewModels.Windows;
using GalacticLauncher.Frontend.Views.MainWindowView;
using Microsoft.Extensions.DependencyInjection;
using GalacticLauncher.Frontend.Infrastructure.Files;
using GalacticLauncher.Frontend.ViewModels.ViewServices;
using GalacticLauncher.Frontend.Services.Cache;
using GalacticLauncher.Frontend.DataAccess.Networking;

namespace GalacticLauncher.Frontend;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var services = new ServiceCollection();

            // Roots
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainWindowViewModel>();

            // View Models
            services.AddSingleton<HomeViewModel>();
            services.AddSingleton<GameViewModel>();
            services.AddSingleton<LibraryViewModel>();
            services.AddSingleton<AdminViewModel>();

            services.AddSingleton<AdGamesViewModel>();
            services.AddSingleton<AdTagsViewModel>();
            services.AddSingleton<AdUsersViewModel>();

            // View Services
            services.AddSingleton<INavigator, Navigator>();
            services.AddSingleton<IThemeManager, ThemeManager>();
            services.AddSingleton<INotifications, Notifications>();

            // Infrastructure Services
            services.AddSingleton<IHttpPoster, HttpPoster>(_ => new(HttpProvider.ApiClient));
            services.AddSingleton<IBackendTalker, BackendTalker>();
            services.AddSingleton<IFileDownloader, FileDownloader>(_ => new(HttpProvider.DownloadClient));
            services.AddSingleton<IFileDecompressor, FileDecompressor>();
            services.AddSingleton<IFileHasher, FileHasher>();
            services.AddSingleton<IJsonFiles, JsonFiles>();

            // Model Services (for View Models)
            services.AddSingleton<IExecManager, ExecManager>();
            services.AddSingleton<IExecPathSystem, ExecPathSystem>();
            services.AddSingleton<ICacheRefresher, CacheRefresher>();

            var serviceProvider = services.BuildServiceProvider();

            desktop.MainWindow = serviceProvider.GetRequiredService<MainWindow>();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
