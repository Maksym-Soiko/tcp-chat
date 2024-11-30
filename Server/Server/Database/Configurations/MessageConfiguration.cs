using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Server.Database.Models;

namespace Server.Database.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder
                .ToTable("messages");

            builder
                .HasKey(m => m.Id);

            builder
                .Property(m => m.Id)
                .HasColumnName("id");

            builder
                .Property(m => m.Text)
                .HasColumnName("text")
                .HasMaxLength(1500)
                .IsRequired(true);

            builder
                .Property(m => m.Timestamp)
                .HasColumnName("timestamp")
                .IsRequired(true);
        }
    }

}