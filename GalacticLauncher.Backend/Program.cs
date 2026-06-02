global using Version = GalacticLauncher.Core.Models.Version;
using Dapper;
using GalacticLauncher.Core;
using GalacticLauncher.Backend.Services;
using GalacticLauncher.Backend.Infrastructure.DbScopes;
using GalacticLauncher.Backend.Infrastructure.TypeHandlers;
using GalacticLauncher.Backend.Infrastructure.Startup;
using GalacticLauncher.Backend;
using GalacticLauncher.Backend.Repositories.Writers;
using GalacticLauncher.Backend.Repositories.Readers;
using GalacticLauncher.Backend.Repositories;

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

// Startup
services.ConfigureSwagger();
services.ConfigureForwardedFor(config);
services.ConfigureRateLimiters(config);
services.AddDatabase(config);

// Infrastructure
services.AddSingleton<IAppScopeFactory, AppScopeFactory>();

// Repositories
services.AddScoped<IGameReader, GameReader>();
services.AddScoped<IImageReader, ImageReader>();
services.AddScoped<IVersionReader, VersionReader>();
services.AddScoped<ITagReader, TagReader>();

services.AddScoped<IGameDataWriter, GameDataWriter>();
services.AddScoped<IGameWriter, GameWriter>();
services.AddScoped<ITagWriter, TagWriter>();

services.AddScoped<IHistoryRepository, HistoryRepository>();

// Services
services.AddSingleton<IDataAccessService, DataAccessService>();
services.AddSingleton<IDataUpdateService, DataUpdateService>();
services.AddSingleton<IAdminService, AdminService>();
services.AddSingleton<IHistoryService, HistoryService>();

// Controllers
services.AddControllers();

// ----- APP SECTION -----

var app = builder.Build();

app.ConfigureMiddleware(config);
app.MapControllers();

app.LogStartup<Program>(config);
app.Run();
