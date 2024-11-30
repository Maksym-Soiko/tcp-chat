using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Server.Database.Models;

namespace Server.Database.Configurations
{
    public class BlockedUserConfiguration : IEntityTypeConfiguration<BlockedUser>
    {
        public void Configure(EntityTypeBuilder<BlockedUser> builder)
        {
            builder
                .ToTable("blocked_users");

            builder
                .HasKey(b => b.Id);

            builder
                .Property(b => b.Id)
                .HasColumnName("id");

            builder
                .Property(b => b.Timestamp)
                .HasColumnName("timestamp")
                .IsRequired(true);
        }
    }
}
