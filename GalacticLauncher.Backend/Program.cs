using GalacticLauncher.Backend.Socket;
using GalacticLauncher.Backend.Socket.Endpoints;

namespace GalacticLauncher.Backend;

class Program
{
    public static void Main(string[] args)
    {
        _ = new RecvBuilder()
            .AddRateLimiters(new Dictionary<string, Policy?>
            {
                { Policy.FREE,
                    new Policy(
                        Period: TimeSpan.FromSeconds(1),
                        Limit: int.MaxValue)
                },
                { Policy.LOW_COST,
                    new Policy(
                        Period: TimeSpan.FromSeconds(1),
                        Limit: 20)
                },
                { Policy.MEDIUM_COST,
                    new Policy(
                        Period: TimeSpan.FromSeconds(3),
                        Limit: 3)
                },
                { Policy.HIGH_COST,
                    new Policy(
                        Period: TimeSpan.FromSeconds(15),
                        Limit: 3)
                },
            })
            .InjectDependencies(srv =>
            {
                // Register singletons and other services here
                // Then use them inside IEndpoint Delegate
                // For example:
                // ------------------------------------------
                // srv.AddSingleton<IEndpoint, AnyGameEndpoint>();
            })
            .BuildWebApp()
            .RegisterEndpoints(app =>
            {
                IEndpoint.MapEndpoint<AnyGameEndpoint>(app);
                IEndpoint.MapEndpoint<GameEcho>(app);
            })
            .Run();
    }
}
