using Microsoft.EntityFrameworkCore;

namespace Server.Database.Models
{
    [EntityTypeConfiguration(typeof(Configurations.FileRecordsConfiguration))]
    public class FileRecord
    {
        public int Id { get; set; }
        public string FileName { get; set; } = null!;
        public byte[]? FileContent { get; set; }
        public DateTime Timestamp { get; set; }

        public int SenderId { get; set; }
        public User Sender { get; set; } = null!;

        public int RecipientId { get; set; }
        public User Recipient { get; set; } = null!;
    }

}
