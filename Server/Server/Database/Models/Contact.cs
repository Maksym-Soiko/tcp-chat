using Microsoft.EntityFrameworkCore;

namespace Server.Database.Models
{
    [EntityTypeConfiguration(typeof(Configurations.ContactConfiguration))]
    public class Contact
    {
        public int Id { get; set; }
        
        public string? ContactName { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public int UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
