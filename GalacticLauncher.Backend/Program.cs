using GalacticLauncher.Backend.Database;
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
                srv.AddGalacticDatabase();
            })

            .RunForever();
    }
}
