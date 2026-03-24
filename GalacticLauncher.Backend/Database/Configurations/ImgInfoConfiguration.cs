using GalacticLauncher.Core.DbRecords;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalacticLauncher.Backend.Database.Configurations;

public class ImgInfoConfiguration : IEntityTypeConfiguration<ImgInfo>
{
    public void Configure(EntityTypeBuilder<ImgInfo> builder)
    {
        builder.ToTable("Images");
        builder.HasKey(x => x.Id);
        
        builder.HasOne<GameInfo>()
            .WithMany()
            .HasForeignKey(x => x.IdGame)
            .OnDelete(DeleteBehavior.Cascade);
    }
}