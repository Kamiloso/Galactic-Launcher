using GalacticLauncher.Core.DbRecords;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalacticLauncher.Backend.Database.Configurations;

public class TagInfoConfiguration : IEntityTypeConfiguration<TagInfo>
{
    public void Configure(EntityTypeBuilder<TagInfo> builder)
    {
        builder.ToTable("tags");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        builder.Property(e => e.Name).HasMaxLength(50);
        builder.Property(e => e.Description).HasMaxLength(200);
        
        // M:N relationship: (tags <-> games)
    }
}