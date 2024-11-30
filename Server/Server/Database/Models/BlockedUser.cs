using Microsoft.EntityFrameworkCore;

namespace Server.Database.Models
{
    [EntityTypeConfiguration(typeof(Configurations.BlockedUserConfiguration))]
    public class BlockedUser
    {
        public int Id { get; set; }

        public int BlockerId { get; set; }
        public User Blocker { get; set; }

        public int BlockedId { get; set; }
        public User Blocked { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
