using GalacticLauncher.Frontend.Domain.Exceptions;
using GalacticLauncher.Frontend.Domain.Models;
using GalacticLauncher.Frontend.Infrastructure.System;
using GalacticLauncher.Frontend.Tools.Networking;
using GalacticLauncher.Frontend.ViewModels.ViewServices;
using System;
using System.Threading.Tasks;

namespace GalacticLauncher.Frontend.Services.Executables;

public interface IGamePlayService
{
    Task<bool> Download(ExecInfo execInfo);
    Task<bool> PlayAndTerminate(ExecInfo execInfo);
    Task<bool> Delete(ExecInfo execInfo);
}

internal class GamePlayService(
    IExecManager execManager,
    ITelemetryCollector telemetryCollector,
    IDialogs dialogs,
    INotifications notifications,
    ITerminator terminator) : IGamePlayService
{
    private readonly TaskObserver _downloading = new();

    public async Task<bool> Download(ExecInfo execInfo)
    {
        if (_downloading.IsRunning)
            return false;

        if (execManager.Exists(execInfo))
            return true;

        long id = execInfo.GameId;
        string title = execInfo.GetFullName();

        notifications.ShowInfo(
            $"Download Started",
            $"Downloading {title}...");

        Progress<DownloadProgressData> progress = new();

        Task downloadTask = _downloading.Start(cancellationToken =>
            execManager.DownloadAsync(execInfo, progress, cancellationToken));

        try
        {
            await dialogs.ShowDownloadProgressDialogAsync(
                $"Downloading...",
                $"Downloading {title}...",
                downloadTask, _downloading.Terminate, progress);

            notifications.ShowSuccess(
                $"Download Complete",
                $"{title} is ready to play.");

            return true;
        }
        catch (OperationCanceledException)
        {
            notifications.ShowInfo(
                $"Download Cancelled",
                $"Download for {title} was cancelled.");

            return false;
        }
        catch (DownloadException)
        {
            notifications.ShowError(
                $"Download Error",
                $"Download for {title} has failed.");

            return false;
        }
    }

    public async Task<bool> PlayAndTerminate(ExecInfo execInfo)
    {
        if (_downloading.IsRunning)
            return false;

        if (!execManager.Exists(execInfo))
            return false;

        long id = execInfo.GameId;
        string title = execInfo.GetFullName();

        try
        {
            execManager.Play(execInfo);

            // Fake loading:

            // 1. Improves responsiveness.
            // 2. Allows telemetry to be sent to the server before app termination.

            // Waiting for 1000 ms should be enough
            // to send data under normal circumstances.

            _ = telemetryCollector.TrackGameLaunch(execInfo);

            await dialogs.ShowLoadingDialogAsync(
                $"Loading...",
                $"Starting {title}...",
                Task.CompletedTask,
                fakeLoadingTime: 1000);

            terminator.Terminate();

            return true;
        }
        catch (ExecutableRunException ex)
        {
            notifications.ShowError(
                "Run Error",
                ex.Message);

            return false;
        }
    }

    public async Task<bool> Delete(ExecInfo execInfo)
    {
        if (_downloading.IsRunning)
            return false;

        string title = execInfo.GetFullName();

        bool isConfirmed = await dialogs.ShowConfirmationDialogAsync(
            $"Delete Version",
            $"Are you sure you want to delete {title}?",
            textYes: "Delete", textNo: "Cancel");

        if (isConfirmed)
        {
            if (execManager.Exists(execInfo))
                execManager.Delete(execInfo);

            notifications.ShowSuccess(
                "Version Deleted",
                $"{title} has been deleted.");
        }

        return isConfirmed;
    }
}
