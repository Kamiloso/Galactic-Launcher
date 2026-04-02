using GalacticLauncher.Core.DbRecords;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalacticLauncher.Backend.Database.Configurations;

public class GameTagInfoConfiguration : IEntityTypeConfiguration<GameTagInfo>
{
    public void Configure(EntityTypeBuilder<GameTagInfo> builder)
    {
        builder.ToTable("games_tags");
        
        builder.HasKey(gt => new { gt.IdGame, gt.IdTag }); // GameID + TagID is the Primary Key

        builder.Property(gt => gt.IdGame).HasColumnName("game_id");
        builder.Property(gt => gt.IdTag).HasColumnName("tag_id");

        builder.HasOne<GameInfo>()
            .WithMany()
            .HasForeignKey(gt => gt.IdGame)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<TagInfo>()
            .WithMany()
            .HasForeignKey(gt => gt.IdTag)
            .OnDelete(DeleteBehavior.Cascade);
    }
}