using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Server.Database.Models;

namespace Server.Database.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder
                .ToTable("users");

            builder
                .HasKey(u => u.Id);

            builder
                .Property(u => u.Id)
                .HasColumnName("id");

            builder
                .Property(u => u.Username)
                .HasColumnName("username")
                .HasMaxLength(100)
                .IsRequired(true);

            builder
                .Property(u => u.Password)
                .HasColumnName("password")
                .HasMaxLength(255)
                .IsRequired(true);

            builder
                .HasIndex(u => u.Username)
                .IsUnique();
        }
    }

}
