using GalacticLauncher.Backend;
using GalacticLauncher.Core;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    EnvironmentName = Utils.IsDebug ? "Development" : "Production"
});

var services = builder.Services;

AppConfig config = services.ConfigureAppConfig(builder.Configuration);

services.AddEndpointsApiExplorer();

if (Utils.IsDebug)
{
    services.AddSwaggerGen();
}

services.ConfigureForwardedFor(config);
services.ConfigureRateLimiters(config);
services.AddGalacticDatabase(config);

services.AddControllers();

// ----- APP SECTION -----

var app = builder.Build();

app.ConfigureMiddleware(config);
app.MapControllers();

app.LogStartup<Program>(config);
app.Run();