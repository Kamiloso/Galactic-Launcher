global using Version = GalacticLauncher.Core.Models.Version;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GalacticLauncher.Core;
using GalacticLauncher.Frontend.Infrastructure.Client;
using GalacticLauncher.Frontend.Services;
using GalacticLauncher.Frontend.Services.Executables;
using GalacticLauncher.Frontend.Services.Files;
using GalacticLauncher.Frontend.Services.Networking;
using GalacticLauncher.Frontend.ViewModels.AdminPanels;
using GalacticLauncher.Frontend.ViewModels.Panels;
using GalacticLauncher.Frontend.ViewModels.Windows;
using GalacticLauncher.Frontend.Views.MainWindowView;
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

            // View Services
            services.AddSingleton<Navigator>();
            services.AddSingleton<ThemeManager>();

            // View Models
            services.AddSingleton<HomeViewModel>();
            services.AddSingleton<GameViewModel>();
            services.AddSingleton<LibraryViewModel>();
            services.AddSingleton<AdminViewModel>();

            services.AddSingleton<AdGamesViewModel>();
            services.AddSingleton<AdTagsViewModel>();
            services.AddSingleton<AdUsersViewModel>();

            // Internal Model Services
            services.AddSingleton<IHttpPoster, HttpPoster>(_ => new(HttpProvider.ApiClient));
            services.AddSingleton<IFileDownloader, FileDownloader>(_ => new(HttpProvider.DownloadClient));
            services.AddSingleton<IExecPathSystem, ExecPathSystem>();
            services.AddSingleton<IFileDecompressor, FileDecompressor>();
            services.AddSingleton<IFileHasher, FileHasher>();

            // Model Services (for View Models)
            services.AddSingleton<IBackendTalker, BackendTalker>();
            services.AddSingleton<IExecManager, ExecManager>();

            var serviceProvider = services.BuildServiceProvider();

            desktop.MainWindow = serviceProvider.GetRequiredService<MainWindow>();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
