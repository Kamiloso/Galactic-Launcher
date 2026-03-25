using GalacticLauncher.Core.DbRecords;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalacticLauncher.Backend.Database.Configurations;

public class GameInfoConfiguration : IEntityTypeConfiguration<GameInfo>
{
    public void Configure(EntityTypeBuilder<GameInfo> builder)
    {
        builder.ToTable("Games");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(20);
    }
}