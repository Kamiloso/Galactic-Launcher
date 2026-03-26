using GalacticLauncher.Backend.Database;
using GalacticLauncher.Backend.Socket;

namespace GalacticLauncher.Backend;

class Program
{
    public static void Main(string[] args)
    {
        new RecvBuilder()
            .ConfigureServices(srv =>
            {
                srv.AddGalacticDatabase();
            })
            .RunForever(args);
    }
}
