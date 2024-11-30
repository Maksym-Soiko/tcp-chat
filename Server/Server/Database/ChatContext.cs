using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Server.Database.Models;

public class ChatContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<FileRecord> FileRecords { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<BlockedUser> BlockedUsers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        optionsBuilder.UseSqlServer(config.GetConnectionString("Default"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany(u => u.SentMessages)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>()
            .HasOne(m => m.Recipient)
            .WithMany(u => u.ReceivedMessages)
            .HasForeignKey(m => m.RecipientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FileRecord>()
            .HasOne(f => f.Sender)
            .WithMany(u => u.SentFiles)
            .HasForeignKey(f => f.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FileRecord>()
            .HasOne(f => f.Recipient)
            .WithMany(u => u.ReceivedFiles)
            .HasForeignKey(f => f.RecipientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Contact>()
            .HasOne(c => c.User)
            .WithMany(u => u.Contacts)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BlockedUser>()
            .HasOne(b => b.Blocker)
            .WithMany()
            .HasForeignKey(b => b.BlockerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<BlockedUser>()
            .HasOne(b => b.Blocked)
            .WithMany()
            .HasForeignKey(b => b.BlockedId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
