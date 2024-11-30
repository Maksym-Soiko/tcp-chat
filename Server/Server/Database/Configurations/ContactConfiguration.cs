using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Server.Database.Models;

namespace Server.Database.Configurations
{
    public class ContactConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> builder)
        {
            builder
                .ToTable("contacts");

            builder
                .HasKey(c => c.Id);

            builder
                .Property(c => c.Id)
                .HasColumnName("id");

            builder
                .Property(c => c.ContactName)
                .HasColumnName("contact_name")
                .HasMaxLength(100)
                .IsRequired(true);

            builder
                .Property(c => c.Timestamp)
                .HasColumnName("timestamp")
                .IsRequired(true);
        }
    }
}
