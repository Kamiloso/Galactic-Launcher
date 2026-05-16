using Dapper;
using GalacticLauncher.Backend;
using GalacticLauncher.Backend.Startup;
using GalacticLauncher.Backend.Patches;
using GalacticLauncher.Backend.Repositories;
using GalacticLauncher.Core;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    EnvironmentName = Utils.IsDevelopment ? "Development" : "Production"
});

DefaultTypeMap.MatchNamesWithUnderscores = true; // snake_case!

SqlMapper.AddTypeHandler(new EnumAsStringHandler<VersionType>());
SqlMapper.AddTypeHandler(new EnumAsStringHandler<Platform>());
SqlMapper.AddTypeHandler(new EnumAsStringHandler<AlertLevel>());
SqlMapper.AddTypeHandler(new EnumAsStringHandler<ImageType>());

var services = builder.Services;

AppConfig config = services.ConfigureAppConfig(builder.Configuration);

if (Utils.IsDevelopment)
{
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(c => c.SupportNonNullableReferenceTypes());
}

// Startup
services.ConfigureForwardedFor(config);
services.ConfigureRateLimiters(config);
services.AddDatabase(config);

// Repositories
services.AddScoped<IGameRepository, GameRepository>();
services.AddScoped<IImageRepository, ImageRepository>();
services.AddScoped<IVersionRepository, VersionRepository>();
services.AddScoped<ITagRepository, TagRepository>();

// Services

services.AddControllers();

// ----- APP SECTION -----

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<Program>>();

app.ConfigureMiddleware(config);
app.MapControllers();

app.LogStartup<Program>(config);
app.Run();
