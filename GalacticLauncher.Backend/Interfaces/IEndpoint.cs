using Microsoft.AspNetCore.Routing;

namespace GalacticLauncher.Backend.Interfaces
{
    public interface IEndpoint
    {
        void MapEndpoint(IEndpointRouteBuilder app);
    }
}