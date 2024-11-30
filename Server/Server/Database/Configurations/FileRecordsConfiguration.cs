using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Server.Database.Models;

namespace Server.Database.Configurations
{
    public class FileRecordsConfiguration : IEntityTypeConfiguration<FileRecord>
    {
        public void Configure(EntityTypeBuilder<FileRecord> builder)
        {
            builder
                .ToTable("file_records");

            builder
                .HasKey(f => f.Id);

            builder
                .Property(f => f.Id)
                .HasColumnName("id");

            builder
                .Property(f => f.FileName)
                .HasColumnName("file_name")
                .HasMaxLength(300)
                .IsRequired(true);

            builder
                .Property(f => f.FileContent)
                .HasColumnName("file_content")
                .IsRequired(false);

            builder
                .Property(f => f.Timestamp)
                .HasColumnName("timestamp")
                .IsRequired(true);
        }
    }

}