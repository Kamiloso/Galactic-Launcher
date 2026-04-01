using GalacticLauncher.Backend;
using GalacticLauncher.Backend.Database;
using GalacticLauncher.Core;
using Microsoft.EntityFrameworkCore;

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

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        if (Utils.IsDebug)
        {
            dbContext.Database.Migrate();
        }
        else
        {
            if (dbContext.Database.CanConnect())
            {
                logger.LogInformation("Connection with the database established.");
            }
            else
            {
                logger.LogWarning("Connection with the database could not be established!");
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogCritical(ex, "Critical error while trying to establish a connection with the database!");
        throw;
    }
}

app.ConfigureMiddleware(config);
app.MapControllers();

app.LogStartup<Program>(config);
app.Run();