using GalacticLauncher.Core.DbRecords;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalacticLauncher.Backend.Database.Configurations;

public class UserInfoConfiguration : IEntityTypeConfiguration<UserInfo>
{
    public void Configure(EntityTypeBuilder<UserInfo> builder)
    {
        builder.ToTable("users");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.GoogleKey).IsUnique();
        
        builder.Property(e => e.Email).HasMaxLength(200);
        builder.Property(e => e.GoogleKey).HasMaxLength(200);
        builder.Property(e => e.Name).HasMaxLength(50);
    }
}