using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace GalacticLauncher.Frontend.Infrastructure.System;

internal sealed class SystemMutex : IDisposable
{
    private const string SALT = "GL-Kamiloso-3d80a4";

    private readonly Mutex _mutex;
    private readonly bool _acquired;

    private bool _disposed;

    private class MutexException() : Exception("No message");

    private SystemMutex(string name, int miliseconds)
    {
        string dvkey = Obfuscate($"{name}::{SALT}")[..20];
        string mutexName = $"Global\\{dvkey}";

        _mutex = new Mutex(false, mutexName);

        try
        {
            _acquired = _mutex.WaitOne(miliseconds);

            if (!_acquired)
            {
                throw new();
            }
        }
        catch (AbandonedMutexException) { _acquired = true; }
        catch (Exception)
        {
            Dispose();

            throw new MutexException();
        }
    }

    public static bool TryAcquire(string name, int miliseconds,
        out SystemMutex mutex)
    {
        try
        {
            mutex = new SystemMutex(name, miliseconds);
            return true;
        }
        catch (MutexException)
        {
            mutex = null!;
            return false;
        }
    }

    private static string Obfuscate(string input)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(input);
        byte[] hashed = SHA256.HashData(bytes);

        return new string([.. Convert.ToBase64String(hashed)
            .Select(c => char.IsLetterOrDigit(c) ? c : '0')]);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        try
        {
            if (_acquired)
            {
                _mutex.ReleaseMutex();
            }
        }
        catch
        {
            // Something is very wrong with the application.
            // Just exit to avoid undefined state.
            Environment.Exit(1);
        }

        _mutex.Dispose();
    }
}
