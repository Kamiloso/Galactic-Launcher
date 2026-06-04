using System;
using System.Threading.Tasks;
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
        string title, string message);

    Func<Task> ShowLoadingDialogAsync( // Returns a function to close the dialog
        string title, string message, int minimumTimeMs = 1000);

    Task ShowLoadingDialogAsync( // Returns the result of the task, while showing a loading dialog
        string title, string message, Task task, int minimumTimeMs = 1000);

    Task<T> ShowLoadingDialogAsync<T>( // Returns the result of the task, while showing a loading dialog
        string title, string message, Task<T> task, int minimumTimeMs = 1000);
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
        string title, string message)
    {
        FlexibleDialogViewModel dialog = new(title, message);

        TextInputViewModel ifUser = dialog.AddInput("Username", "Username");
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
        string title, string message, int minimumTimeMs = 1000)
    {
        LoadingDialogViewModel dialog = new(title, message,
            minimumTimeMs: minimumTimeMs);

        _ = ShowDialogAsync(dialog);

        return dialog.Finish;
    }

    public async Task ShowLoadingDialogAsync(
        string title, string message, Task task, int minimumTimeMs = 1000)
    {
        await ShowLoadingDialogAsync(title, message,
            task: task.ContinueWith(_ => 0),
            minimumTimeMs: minimumTimeMs);
    }

    public async Task<T> ShowLoadingDialogAsync<T>(
        string title, string message, Task<T> task, int minimumTimeMs = 1000)
    {
        Func<Task> finish = ShowLoadingDialogAsync(title, message,
            minimumTimeMs: minimumTimeMs);

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