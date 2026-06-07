global using Version = GalacticLauncher.Core.Models.Version;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using GalacticLauncher.Frontend.Infrastructure.Http;
using GalacticLauncher.Frontend.Repositories;
using GalacticLauncher.Frontend.Services;
using GalacticLauncher.Frontend.Services.Admin;
using GalacticLauncher.Frontend.Services.Cache;
using GalacticLauncher.Frontend.Services.Data;
using GalacticLauncher.Frontend.Services.Executables;
using GalacticLauncher.Frontend.Tools.Files;
using GalacticLauncher.Frontend.Tools.Networking;
using GalacticLauncher.Frontend.ViewModels.AdminPanels;
using GalacticLauncher.Frontend.ViewModels.Panels;
using GalacticLauncher.Frontend.ViewModels.ViewServices;
using GalacticLauncher.Frontend.ViewModels.Windows;
using GalacticLauncher.Frontend.Views.MainWindowView;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

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

            // Avalonia
            services.AddSingleton(desktop);

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
            services.AddSingleton<IAdminPanelSelector, AdminPanelSelector>();
            services.AddSingleton<IGameButtonFactory, GameButtonFactory>();
            services.AddSingleton<IImageFactory, ImageFactory>();
            services.AddSingleton<IThemeManager, ThemeManager>();
            services.AddSingleton<IGamePlayService, GamePlayService>();
            services.AddSingleton<INavigator, Navigator>();
            services.AddSingleton<INotifications, Notifications>();
            services.AddSingleton<ITerminator, Terminator>();
            services.AddSingleton<IDialogs, Dialogs>();
            services.AddSingleton<IErrorHandler, ErrorHandler>();

            // Tools
            services.AddSingleton<IFileDownloader, FileDownloader>(_ => new(HttpProvider.DownloadClient));
            services.AddSingleton<IFileDecompressor, FileDecompressor>();
            services.AddSingleton<IFileHasher, FileHasher>();
            services.AddSingleton<IJsonFiles, JsonFiles>();
            services.AddSingleton<IHttpPoster, HttpPoster>(_ => new(HttpProvider.ApiClient));
            services.AddSingleton<IBackendTalker, BackendTalker>();
            services.AddSingleton<ITelemetryCollector, TelemetryCollector>();

            // Repositories
            services.AddSingleton<ICacheRepository, CacheRepository>();
            services.AddSingleton<IDataRepository, DataRepository>();
            services.AddSingleton<IMemoryRepository, MemoryRepository>();

            // Services
            services.AddSingleton<IExecManager, ExecManager>();
            services.AddSingleton<IExecPathSystem, ExecPathSystem>();
            services.AddSingleton<IExecRunner, ExecRunner>();
            services.AddSingleton<IExecCleaner, ExecCleaner>();
            services.AddSingleton<ICacheRefresher, CacheRefresher>();
            services.AddSingleton<ICacheProvider, CacheProvider>();
            services.AddSingleton<IImageProvider, ImageProvider>();
            services.AddSingleton<IGameListManager, GameListManager>();
            services.AddSingleton<ILastGameManager, LastGameManager>();
            services.AddSingleton<IPreferenceManager, PreferenceManager>();
            services.AddSingleton<IAuthService, AuthService>();

            // Initialize App
            InitializeApp(services, services.BuildServiceProvider());
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void InitializeApp(
        IServiceCollection services, ServiceProvider serviceProvider)
    {
        InstantiateSingletons(services, serviceProvider);

        // Error handling
        var errorHandler = serviceProvider.GetRequiredService<IErrorHandler>();
        var notifications = serviceProvider.GetRequiredService<INotifications>();

        errorHandler.OnInfo += notifications.ShowInfo;
        errorHandler.OnWarning += notifications.ShowWarning;
        errorHandler.OnError += notifications.ShowError;
        errorHandler.OnSuccess += notifications.ShowSuccess;

        // Cache Initialization
        var cacheRefresher = serviceProvider.GetRequiredService<ICacheRefresher>();

        _ = cacheRefresher.RefreshRootAsync();

        // Window Initialize
        var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
        var desktop = serviceProvider.GetRequiredService<IClassicDesktopStyleApplicationLifetime>();

        desktop.MainWindow = mainWindow;
    }

    private static void InstantiateSingletons(
        IServiceCollection services, ServiceProvider serviceProvider)
    {
        List<ServiceDescriptor> singletonDescriptors = [..
            services.Where(d =>
                d.Lifetime == ServiceLifetime.Singleton &&
                !d.ServiceType.ContainsGenericParameters)
            ];

        foreach (var descriptor in singletonDescriptors)
        {
            _ = serviceProvider.GetService(descriptor.ServiceType);
        }
    }
}
