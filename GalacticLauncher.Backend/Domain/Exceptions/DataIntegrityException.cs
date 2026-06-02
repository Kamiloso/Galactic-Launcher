namespace GalacticLauncher.Backend.Domain.Exceptions;

public class DataIntegrityException(string message, Exception? innerException = null)
    : Exception(message, innerException)
{
}
