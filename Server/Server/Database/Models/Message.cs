using Microsoft.EntityFrameworkCore;

namespace Server.Database.Models
{
    [EntityTypeConfiguration(typeof(Configurations.MessageConfiguration))]
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; } = null!;
        public DateTime Timestamp { get; set; }

        public int SenderId { get; set; }
        public User Sender { get; set; } = null!;

        public int RecipientId { get; set; }
        public User Recipient { get; set; } = null!;
    }

}
