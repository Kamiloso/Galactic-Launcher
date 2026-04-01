using GalacticLauncher.Core.DbRecords;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GalacticLauncher.Backend.Database.Configurations;

public class ExecInfoConfiguration : IEntityTypeConfiguration<ExecInfo>
{
    public void Configure(EntityTypeBuilder<ExecInfo> builder)
    {
        builder.ToTable("execs");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedOnAdd();
        
        builder.Property(e => e.IdVersion).HasColumnName("version_id");
        builder.Property(e => e.FileHashSHA256).HasMaxLength(64).IsFixedLength().UseCollation("ascii_general_ci"); // Fixed length of 64 characters

        // 1:N relationships: (game -> versions, versions -> exe s)
        builder.HasOne(e => e.Version)
            .WithMany(v => v.Executables)
            .HasForeignKey(e => e.IdVersion)
            .OnDelete(DeleteBehavior.Cascade); // if the version is deleted its executables are removed from as well
    }
}