namespace GalacticLauncher.Backend.Domain.Exceptions;

public class ClientFaultException(string message, int statusCode) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
    public string FaultInfo => $"{Message} (HTTP {StatusCode})";

    public static ClientFaultException BadRequest400(string message) =>
        new(message, StatusCodes.Status400BadRequest);

    public static ClientFaultException Unauthorized401(string message) =>
        new(message, StatusCodes.Status401Unauthorized);

    public static ClientFaultException Forbidden403(string message) =>
        new(message, StatusCodes.Status403Forbidden);

    public static ClientFaultException NotFound404(string message) =>
        new(message, StatusCodes.Status404NotFound);

    public static ClientFaultException TooManyRequests429(string message) =>
        new(message, StatusCodes.Status429TooManyRequests);
}
