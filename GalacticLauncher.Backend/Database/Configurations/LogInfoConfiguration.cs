using GalacticLauncher.Core.DbRecords;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalacticLauncher.Backend.Database.Configurations;

public class LogInfoConfiguration : IEntityTypeConfiguration<LogInfo>
{
    public void Configure(EntityTypeBuilder<LogInfo> builder)
    {
        builder.ToTable("Actions");
        builder.HasKey(e => e.Id);

        builder.HasOne<UserInfo>()
            .WithMany()
            .HasForeignKey(e => e.IdUser);

        builder.HasOne<ExecInfo>()
            .WithMany()
            .HasForeignKey(e => e.IdExec)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull); 
    }
}