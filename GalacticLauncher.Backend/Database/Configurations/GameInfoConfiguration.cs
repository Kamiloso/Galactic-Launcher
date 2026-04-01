using GalacticLauncher.Core.DbRecords;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalacticLauncher.Backend.Database.Configurations;

public class GameInfoConfiguration : IEntityTypeConfiguration<GameInfo>
{
    public void Configure(EntityTypeBuilder<GameInfo> builder)
    {
        builder.ToTable("games");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        
        builder.Property(e => e.Name).HasMaxLength(50);
        builder.Property(e => e.Description).HasMaxLength(2000);
        // 1:N relationships: (game -> versions, game -> images)
        // M:N relationship: (game <-> tags)
    }
}