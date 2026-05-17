global using Version = GalacticLauncher.Core.Models.Version;
using Dapper;
using GalacticLauncher.Backend.Repositories;
using GalacticLauncher.Core;
using GalacticLauncher.Backend.Services;
using GalacticLauncher.Backend;
using GalacticLauncher.Backend.Infrastructure.DbScopes;
using GalacticLauncher.Backend.Infrastructure.TypeHandlers;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    EnvironmentName = Utils.IsDevelopment ? "Development" : "Production"
});

DefaultTypeMap.MatchNamesWithUnderscores = true; // snake_case!

SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());

SqlMapper.AddTypeHandler(new EnumTypeHandler<VersionType>());
SqlMapper.AddTypeHandler(new EnumTypeHandler<Platform>());
SqlMapper.AddTypeHandler(new EnumTypeHandler<AlertLevel>());
SqlMapper.AddTypeHandler(new EnumTypeHandler<ImageType>());

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
services.AddSingleton<IAppScopeFactory, AppScopeFactory>();
services.AddSingleton<IDataAccessService, DataAccessService>();

// Controllers
services.AddControllers();

// ----- APP SECTION -----

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<Program>>();

app.ConfigureMiddleware(config);
app.MapControllers();

app.LogStartup<Program>(config);
app.Run();
