using GalacticLauncher.Core.DbRecords;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalacticLauncher.Backend.Database.Configurations;

public class LogInfoConfiguration : IEntityTypeConfiguration<LogInfo>
{
    public void Configure(EntityTypeBuilder<LogInfo> builder)
    {
        builder.ToTable("actions");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();

        builder.Property(e => e.IdUser).HasColumnName("user_id");
        builder.Property(e => e.IdExec).HasColumnName("exec_id");

        builder.HasOne<UserInfo>()
            .WithMany()
            .HasForeignKey(e => e.IdUser)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<ExecInfo>()
            .WithMany()
            .HasForeignKey(e => e.IdExec)
            .OnDelete(DeleteBehavior.SetNull);
    }
}