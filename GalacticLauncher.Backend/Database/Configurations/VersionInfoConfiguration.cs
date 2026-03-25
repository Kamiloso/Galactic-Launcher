using GalacticLauncher.Core.DbRecords;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalacticLauncher.Backend.Database.Configurations;

public class VersionInfoConfiguration : IEntityTypeConfiguration<VersionInfo>
{
    public void Configure(EntityTypeBuilder<VersionInfo> builder)
    {
        builder.ToTable("Versions");
        builder.HasKey(e => e.Id);

        builder.HasOne<GameInfo>()
            .WithMany()
            .HasForeignKey(e => e.IdGame)
            .OnDelete(DeleteBehavior.Cascade);
    }
}