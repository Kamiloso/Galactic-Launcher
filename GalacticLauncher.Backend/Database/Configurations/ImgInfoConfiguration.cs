using GalacticLauncher.Core.DbRecords;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalacticLauncher.Backend.Database.Configurations;

public class ImgInfoConfiguration : IEntityTypeConfiguration<ImgInfo>
{
    public void Configure(EntityTypeBuilder<ImgInfo> builder)
    {
        builder.ToTable("images");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(e => e.IdGame).HasColumnName("game_id");
        
        // 1:N relation: (game -> images)
        builder.HasOne<GameInfo>()
            .WithMany()
            .HasForeignKey(e => e.IdGame)
            .OnDelete(DeleteBehavior.Cascade);
    }
}