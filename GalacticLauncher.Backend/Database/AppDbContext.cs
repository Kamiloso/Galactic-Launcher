using GalacticLauncher.Core.DbRecords;
using Microsoft.EntityFrameworkCore;

namespace GalacticLauncher.Backend.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<GameInfo> Games { get; set; }
    public DbSet<ImgInfo> Images { get; set; }
    public DbSet<VersionInfo> Versions { get; set; }
    public DbSet<ExecInfo> Execs { get; set; }
    public DbSet<UserInfo> Users { get; set; }
    public DbSet<LogInfo> Actions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
}