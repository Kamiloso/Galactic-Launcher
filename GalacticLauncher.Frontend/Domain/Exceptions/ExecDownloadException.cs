using System;

namespace GalacticLauncher.Frontend.Domain.Exceptions;

internal class ExecDownloadException(string message, Exception? innerException = null)
    : Exception(message, innerException) { }
