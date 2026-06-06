using System;
using System.Threading.Tasks;
using GalacticLauncher.Frontend.Domain.Models;
using GalacticLauncher.Frontend.ViewModels.Dialogs;

namespace GalacticLauncher.Frontend.ViewModels.ViewServices;

internal interface IDialogs
{
    event Action<object?>? OnDialogChanged;

    Task ShowOkDialogAsync(
        string title, string message, string textOk = "OK");

    Task<bool> ShowConfirmationDialogAsync(
        string title, string message, string textYes = "Yes", string textNo = "No");

    Task<(string Username, string Password)?> ShowLoginDialogAsync(
        string title, string message, string username = "", string password = "");

    Func<Task> ShowLoadingDialogAsync( // Returns a function to close the dialog
        string title, string message, int minimumTimeMs = 0);

    Task ShowLoadingDialogAsync( // Returns the result of the task, while showing a loading dialog
        string title, string message, Task task, int fakeLoadingTime = 0);

    Task<T> ShowLoadingDialogAsync<T>( // Returns the result of the task, while showing a loading dialog
        string title, string message, Task<T> task, int fakeLoadingTime = 0);

    Func<Task> ShowProgressDialogAsync( // Returns a function to close the dialog
        string title, string message, Progress<DownloadProgressData> progress, Action onCancel);

    Task ShowProgressDialogAsync( // Returns the result of the task, while showing a progress dialog
        string title, string message, Task task, Progress<DownloadProgressData> progress, Action onCancel);

    Task<T> ShowProgressDialogAsync<T>( // Returns the result of the task, while showing a progress dialog
        string title, string message, Task<T> task, Progress<DownloadProgressData> progress, Action onCancel);
}

internal class Dialogs : IDialogs
{
    public event Action<object?>? OnDialogChanged;

    public async Task ShowOkDialogAsync(
        string title, string message, string textOk = "OK")
    {
        FlexibleDialogViewModel dialog = new(title, message);

        dialog.AddButton(textOk, true, isHighlighted: true);

        await ShowDialogAsync(dialog);
    }

    public async Task<bool> ShowConfirmationDialogAsync(
        string title, string message, string textYes = "Yes", string textNo = "No")
    {
        FlexibleDialogViewModel dialog = new(title, message);

        dialog.AddButton(textYes, true, isHighlighted: true);
        dialog.AddButton(textNo, false);

        return (bool)(await ShowDialogAsync(dialog) ?? false);
    }

    public async Task<(string Username, string Password)?> ShowLoginDialogAsync(
        string title, string message, string username = "", string password = "")
    {
        FlexibleDialogViewModel dialog = new(title, message);

        TextInputViewModel ifUser = dialog.AddInput("Username", "Username", fillText: username);
        TextInputViewModel ifPass = dialog.AddInput("Password", "Password", isPassword: true);

        dialog.AddButton("Login", true, isHighlighted: true);
        dialog.AddButton("Cancel", false);

        bool allowed = (bool)(await ShowDialogAsync(dialog) ?? false);
        if (!allowed) return null;

        return (
            Username: ifUser.Text,
            Password: ifPass.Text
            );
    }

    public Func<Task> ShowLoadingDialogAsync(
        string title, string message, int fakeLoadingTime = 0)
    {
        LoadingDialogViewModel dialog = new(title, message,
            fakeLoadingTime: fakeLoadingTime);

        _ = ShowDialogAsync(dialog);

        return dialog.Finish;
    }

    public async Task ShowLoadingDialogAsync(
        string title, string message, Task task, int fakeLoadingtime = 0)
    {
        await ShowLoadingDialogAsync(title, message,
            task: task.ContinueWith(_ => 0),
            fakeLoadingTime: fakeLoadingtime);
    }

    public async Task<T> ShowLoadingDialogAsync<T>(
        string title, string message, Task<T> task, int fakeLoadingTime = 0)
    {
        Func<Task> finish = ShowLoadingDialogAsync(title, message,
            fakeLoadingTime: fakeLoadingTime);

        T result = await task;

        await finish();

        return result;
    }
    
    public Func<Task> ShowProgressDialogAsync(
        string title, string message, Progress<DownloadProgressData> progress, Action onCancel)
    {
        ProgressDialogViewModel dialog = new(title, message, onCancel);

        progress.ProgressChanged += (_, e) =>
        {
            dialog.ProgressValue = e.Percentage;
            
            double currentMb = e.DownloadedBytes / 1048576.0;
            if (e.TotalBytes.HasValue)
            {
                dialog.IsIndeterminate = false;
                dialog.ProgressValue = e.Percentage;
                
                double totalMb = e.TotalBytes.Value / 1048576.0;
                dialog.ProgressText = $"{currentMb:F2} MB / {totalMb:F2} MB";
            }
            else
            {
                dialog.IsIndeterminate = true;
                dialog.ProgressText = $"{currentMb:F2} MB Downloaded";
            }
        };

        _ = ShowDialogAsync(dialog);

        return dialog.Finish;
    }

    public async Task ShowProgressDialogAsync(
        string title, string message, Task task, Progress<DownloadProgressData> progress, Action onCancel)
    {
        Func<Task> finish = ShowProgressDialogAsync(title, message, progress, onCancel);
        
        try
        {
            await task;
        }
        finally
        {
            await finish(); 
        }
    }

    public async Task<T> ShowProgressDialogAsync<T>(
        string title, string message, Task<T> task, Progress<DownloadProgressData> progress, Action onCancel)
    {
        Func<Task> finish = ShowProgressDialogAsync(title, message, progress, onCancel);
        
        T result = await task;

        await finish();

        return result;
    }

    private async Task<TResult> ShowDialogAsync<TResult>(DialogViewModel<TResult> dialogVm)
    {
        OnDialogChanged?.Invoke(dialogVm);

        try
        {
            return await dialogVm.GetResultInternal();
        }
        finally
        {
            OnDialogChanged?.Invoke(null);
        }
    }
}