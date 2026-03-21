namespace GalacticLauncher.Backend.Endpoints
{
    public static class EndpointExtensions
    {
        public static IEndpointRouteBuilder MapAllEndpoints(this IEndpointRouteBuilder app)
        {
            new AnyGameEndpoint().MapEndpoint(app);

            // TODO: Dodac kolejne Endpoint dla roznych funkcji np.:
            // new LoginEndpoint().MapEndpoint(app);
            // new DownloadEndpoint().MapEndpoint(app);

            return app;
        }
    }
}