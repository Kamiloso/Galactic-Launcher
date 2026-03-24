using GalacticLauncher.Core.DbRecords;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalacticLauncher.Backend.Database.Configurations;

public class UserInfoConfiguration : IEntityTypeConfiguration<UserInfo>
{
    public void Configure(EntityTypeBuilder<UserInfo> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(e => e.Id);
    }
}