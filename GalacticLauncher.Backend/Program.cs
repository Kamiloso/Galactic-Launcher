using GalacticLauncher.Backend;
using GalacticLauncher.Core;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    EnvironmentName = Utils.IsDevelopment ? "Development" : "Production"
});

var services = builder.Services;

AppConfig config = services.ConfigureAppConfig(builder.Configuration);

services.AddEndpointsApiExplorer();

if (Utils.IsDevelopment)
{
    services.AddSwaggerGen(c =>
    {
        c.SupportNonNullableReferenceTypes();
    });
}

services.ConfigureForwardedFor(config);
services.ConfigureRateLimiters(config);
services.AddGalacticDatabase(config);

services.AddControllers();

// ----- APP SECTION -----

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<Program>>();

if (app.ConnectToDatabase(logger))
{
    app.ConfigureMiddleware(config);
    app.MapControllers();

    app.LogStartup<Program>(config);
    app.Run();
}
