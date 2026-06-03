global using Version = GalacticLauncher.Core.Models.Version;
using Dapper;
using GalacticLauncher.Core;
using GalacticLauncher.Backend.Services;
using GalacticLauncher.Backend.Infrastructure.DbScopes;
using GalacticLauncher.Backend.Infrastructure.TypeHandlers;
using GalacticLauncher.Backend.Infrastructure.Startup;
using GalacticLauncher.Backend;
using GalacticLauncher.Backend.Repositories;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    EnvironmentName = Utils.IsDevelopment ? "Development" : "Production"
});

DefaultTypeMap.MatchNamesWithUnderscores = true; // snake_case!

SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());

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
services.AddScoped<IGameRepository, GameRepository>();
services.AddScoped<IImageRpository, ImageRepository>();
services.AddScoped<IVersionRepository, VersionRepository>();
services.AddScoped<ITagRepository, TagRepository>();
services.AddScoped<IHistoryRepository, HistoryRepository>();
services.AddScoped<IGameTreeWriter, GameTreeWriter>();

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
