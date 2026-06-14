using System;
using System.Threading;
using System.Threading.Tasks;
using GalacticLauncher.Frontend.Domain.Models;
using GalacticLauncher.Frontend.ViewModels.Dialogs;

namespace GalacticLauncher.Frontend.ViewModels.ViewServices;

public interface IDialogs
{
    event Action<object?>? OnDialogChanged;

    Task ShowOkDialogAsync(
        string title, string message, string textOk = "OK");

    Task<bool> ShowConfirmationDialogAsync(
        string title, string message, string textYes = "Yes", string textNo = "No");

    Task<(string Username, string Password)?> ShowLoginDialogAsync(
        string title, string message, string username = "", string password = "");

    Func<Task> ShowLoadingDialog(
        string title, string message, int fakeLoadingTime = 0);

    Task ShowLoadingDialogAsync( // awaits the given task
        string title, string message, Task task, int fakeLoadingTime = 0);

    Task<T> ShowLoadingDialogAsync<T>( // awaits the given task
        string title, string message, Task<T> task, int fakeLoadingTime = 0);

    Task ShowDownloadProgressDialogAsync(
        string title, string message, Task downloadTask, Action terminate,
        Progress<DownloadProgressData> progress);
}

public class Dialogs : IDialogs
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

    public Func<Task> ShowLoadingDialog(
        string title, string message, int fakeLoadingTime = 0)
    {
        LoadingDialogViewModel dialog = new(title, message,
            fakeLoadingTime: fakeLoadingTime);

        _ = ShowDialogAsync(dialog);

        return dialog.Finish;
    }

    public async Task ShowLoadingDialogAsync(
        string title, string message, Task task, int fakeLoadingTime = 0)
    {
        Func<Task> close = ShowLoadingDialog(title, message,
            fakeLoadingTime: fakeLoadingTime);

        try
        {
            await task;
        }
        finally
        {
            await close();
        }
    }

    public async Task<T> ShowLoadingDialogAsync<T>(
        string title, string message, Task<T> task, int fakeLoadingtime = 0)
    {
        await ShowLoadingDialogAsync(title, message, task,
            fakeLoadingTime: fakeLoadingtime);

        return await task;
    }

    public async Task ShowDownloadProgressDialogAsync(
        string title, string message, Task downloadTask, Action terminate,
        Progress<DownloadProgressData> progress)
    {
        ProgressDialogViewModel dialog = new(title, message, progress);
        dialog.OnCancel += terminate;

        _ = ShowDialogAsync(dialog);

        try
        {
            await downloadTask;
        }
        finally
        {
            await dialog.Finish();
        }
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