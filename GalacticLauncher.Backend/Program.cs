using GalacticLauncher.Backend.Repositories;
using GalacticLauncher.Backend.Socket;
using GalacticLauncher.Backend.Socket.Endpoints;
using GalacticLauncher.Core;
using Microsoft.EntityFrameworkCore;

namespace GalacticLauncher.Backend;

class Program
{
    // TODO: Move to config
    public static int PrefixIPv4 => 32;
    public static int PrefixIPv6 => 56;
    public static bool UseForwardedFor => false;

    public static void Main(string[] args)
    {
        new RecvBuilder(Utils.Address)

            .SetConfig(PrefixIPv4, PrefixIPv6, UseForwardedFor)

            .WithRateLimit(Utils.FREE, TimeSpan.FromSeconds(1), int.MaxValue)
            .WithRateLimit(Utils.LOW_COST, TimeSpan.FromSeconds(1), 20)
            .WithRateLimit(Utils.MEDIUM_COST, TimeSpan.FromSeconds(3), 3)
            .WithRateLimit(Utils.HIGH_COST, TimeSpan.FromSeconds(3), 3)

            .ConfigureServices(srv =>
            {
                // TODO: ADD Valid live connection for the Database in the appsettings for example (in the connectino string MySql is assumed)
                srv.AddDbContext<Database.AppDbContext>((provider, options) =>
                {
                    var config = provider.GetRequiredService<IConfiguration>();
                    
#if DEBUG
                    var connection = config.GetConnectionString("LocalConnection") ?? throw new InvalidOperationException("LocalConnection string is missing!");
#else
                    var connection = config.GetConnectionString("LiveConnection") ?? throw new InvalidOperationException("LiveConnection string is missing!");
#endif
                    // Version can be changed depending on which one gets chosen (e.g., MySQL 8.0.45)
                    var serverVersion = new MySqlServerVersion(new Version(8, 0, 45));
    
                    options.UseMySql(connection, serverVersion) // For now it is set to MySql as an example
                        .UseSnakeCaseNamingConvention(); // snake_case!
                });

                srv.AddScoped<ILauncherRepository, LauncherRepository>();// Instead of Singleton so that the database doesnt break
                
                // srv.AddSingleton<IEndpoint, AnyGameEndpoint>();
            })

            .RunForever();
    }
}
