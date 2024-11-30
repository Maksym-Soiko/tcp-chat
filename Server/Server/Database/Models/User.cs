using Microsoft.EntityFrameworkCore;

namespace Server.Database.Models
{
    [EntityTypeConfiguration(typeof(Configurations.UserConfiguration))]
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;

        public ICollection<Message>? SentMessages { get; set; }
        public ICollection<Message>? ReceivedMessages { get; set; }
        public ICollection<FileRecord>? SentFiles { get; set; }
        public ICollection<FileRecord>? ReceivedFiles { get; set; }
        public ICollection<Contact>? Contacts { get; set; }
    }
}
