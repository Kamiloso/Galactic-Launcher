using Microsoft.OpenApi.Models;

namespace GalacticLauncher.Backend.Socket;

internal interface IEndpoint
{
    string Name { get; }
    string HttpMethod { get; }
    string? RateLimitingPolicy { get; }
    string Summary => $"Activates {Name} endpoint.";
    string Description => $"No more information.";

    Delegate HandleRequest { get; }

    public static void MapEndpoint<T>(IEndpointRouteBuilder app) where T : IEndpoint, new()
    {
        T handler = new();

        string fullName = $"/{handler.Name}";

        var chain = app.MapMethods(fullName, [handler.HttpMethod], handler.HandleRequest)
            .WithName(fullName)
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = handler.Summary,
                Description = handler.Description
            });

        string? policy = handler.RateLimitingPolicy;
        if (policy != null)
        {
            chain.RequireRateLimiting(policy);
        }
    }
}