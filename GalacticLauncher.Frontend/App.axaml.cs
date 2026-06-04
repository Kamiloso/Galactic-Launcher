global using Version = GalacticLauncher.Core.Models.Version;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GalacticLauncher.Frontend.Infrastructure.Http;
using GalacticLauncher.Frontend.Repositories;
using GalacticLauncher.Frontend.Services.Admin;
using GalacticLauncher.Frontend.Services.Data;
using GalacticLauncher.Frontend.Services.Executables;
using GalacticLauncher.Frontend.Services.Handlers;
using GalacticLauncher.Frontend.Services.Images;
using GalacticLauncher.Frontend.Tools.Files;
using GalacticLauncher.Frontend.Tools.Networking;
using GalacticLauncher.Frontend.ViewModels;
using GalacticLauncher.Frontend.ViewModels.AdminPanels;
using GalacticLauncher.Frontend.ViewModels.Panels;
using GalacticLauncher.Frontend.ViewModels.ViewServices;
using GalacticLauncher.Frontend.ViewModels.Windows;
using GalacticLauncher.Frontend.Views.MainWindowView;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            services.AddSingleton<ILibraryGameButtonFactory, LibraryGameButtonFactory>();
            services.AddSingleton<IThemeManager, ThemeManager>();
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

            // Repositories
            services.AddSingleton<ICacheRepository, CacheRepository>();
            services.AddSingleton<IDataRepository, DataRepository>();

            // Services
            services.AddSingleton<IExecManager, ExecManager>();
            services.AddSingleton<IExecPathSystem, ExecPathSystem>();
            services.AddSingleton<IExecRunner, ExecRunner>();
            services.AddSingleton<ICacheRefresher, CacheRefresher>();
            services.AddSingleton<ICacheProvider, CacheProvider>();
            services.AddSingleton<IImageProvider, ImageProvider>();
            services.AddSingleton<IGameListManager, GameListManager>();
            services.AddSingleton<ILastGameManager, LastGameManager>();
            services.AddSingleton<IAuthService, AuthService>();

            // Initialize App
            var serviceProvider = services.BuildServiceProvider();

            InstantiateSingletons(serviceProvider, services);

            var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
            var cacheRefresher = serviceProvider.GetRequiredService<ICacheRefresher>();

            _ = cacheRefresher.RefreshRootAsync();

            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    public static void InstantiateSingletons(IServiceProvider provider, IServiceCollection services)
    {
        List<ServiceDescriptor> singletonDescriptors = [..
            services.Where(d =>
                d.Lifetime == ServiceLifetime.Singleton &&
                !d.ServiceType.ContainsGenericParameters)
            ];

        foreach (var descriptor in singletonDescriptors)
        {
            _ = provider.GetService(descriptor.ServiceType);
        }
    }
}
