using GalacticLauncher.Core.DbRecords;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalacticLauncher.Backend.Database.Configurations;

public class VersionInfoConfiguration : IEntityTypeConfiguration<VersionInfo>
{
    public void Configure(EntityTypeBuilder<VersionInfo> builder)
    {
        builder.ToTable("versions");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        
        builder.Property(e => e.IdGame).HasColumnName("game_id");
        builder.Property(e => e.VersionText).HasMaxLength(50);
        builder.Property(e => e.Description).HasMaxLength(2000);
        
        // 1:N relation: (game -> versions)
        builder.HasOne<GameInfo>()
            .WithMany()
            .HasForeignKey(e => e.IdGame)
            .OnDelete(DeleteBehavior.Cascade); // if the game is deleted all its version are deleted as well
    }
}