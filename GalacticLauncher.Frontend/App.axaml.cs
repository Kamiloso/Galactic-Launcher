global using Version = GalacticLauncher.Core.Models.Version;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GalacticLauncher.Frontend.Infrastructure.Http;
using GalacticLauncher.Frontend.Repositories;
using GalacticLauncher.Frontend.Services.Cache;
using GalacticLauncher.Frontend.Services.Executables;
using GalacticLauncher.Frontend.Services.Files;
using GalacticLauncher.Frontend.Services.UserData;
using GalacticLauncher.Frontend.Tools.Files;
using GalacticLauncher.Frontend.Tools.Networking;
using GalacticLauncher.Frontend.ViewModels.AdminPanels;
using GalacticLauncher.Frontend.ViewModels.Panels;
using GalacticLauncher.Frontend.ViewModels.ViewServices;
using GalacticLauncher.Frontend.ViewModels.Windows;
using GalacticLauncher.Frontend.Views.MainWindowView;
using GalacticLauncher.Frontend.ViewModels;
using Microsoft.Extensions.DependencyInjection;

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

            services.AddSingleton<LoadingViewModel>();

            // View Services
            services.AddSingleton<INavigator, Navigator>();
            services.AddSingleton<IThemeManager, ThemeManager>();
            services.AddSingleton<INotifications, Notifications>();

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
            services.AddSingleton<IUserDataService, UserDataService>();
            services.AddSingleton<IImageService, ImageService>();
            services.AddSingleton<IGameSelectionService, GameSelectionService>();


            var serviceProvider = services.BuildServiceProvider();

            desktop.MainWindow = serviceProvider.GetRequiredService<MainWindow>();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
