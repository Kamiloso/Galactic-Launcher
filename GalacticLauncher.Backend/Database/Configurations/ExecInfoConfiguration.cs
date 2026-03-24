using GalacticLauncher.Core.DbRecords;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalacticLauncher.Backend.Database.Configurations;

public class ExecInfoConfiguration : IEntityTypeConfiguration<ExecInfo>
{
    public void Configure(EntityTypeBuilder<ExecInfo> builder)
    {
        builder.ToTable("Execs");
        builder.HasKey(e => e.Id);

        builder.HasOne<VersionInfo>()
            .WithMany()
            .HasForeignKey(e => e.IdVersion)
            .OnDelete(DeleteBehavior.Cascade);
    }
}