using Microsoft.EntityFrameworkCore;
using Sways.Models;

namespace Sways.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<UserAccount> UserAccounts { get; set; } = default!;
    public DbSet<BlogPost> BlogPosts { get; set; } = default!;
    public DbSet<PollPost> PollPosts { get; set; } = default!;
    public DbSet<PollOption> PollOptions { get; set; } = default!;
    public DbSet<PollVote> PollVotes { get; set; } = default!;
    public DbSet<PostLike> PostLikes { get; set; } = default!;
    public DbSet<PostComment> PostComments { get; set; } = default!;
    public DbSet<UserFollower> UserFollowers { get; set; } = default!;
    public DbSet<Club> Clubs { get; set; } = default!;
    public DbSet<ClubMember> ClubMembers { get; set; } = default!;
    public DbSet<ClubChannel> ClubChannels { get; set; } = default!;
    public DbSet<ClubMessage> ClubMessages { get; set; } = default!;
    public DbSet<DirectMessage> DirectMessages { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserFollower>()
            .HasOne(uf => uf.Follower)
            .WithMany()
            .HasForeignKey(uf => uf.FollowerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserFollower>()
            .HasOne(uf => uf.Following)
            .WithMany()
            .HasForeignKey(uf => uf.FollowingId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DirectMessage>()
            .HasOne(dm => dm.Sender)
            .WithMany()
            .HasForeignKey(dm => dm.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DirectMessage>()
            .HasOne(dm => dm.Receiver)
            .WithMany()
            .HasForeignKey(dm => dm.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Club>()
            .HasOne(c => c.CreatedByUser)
            .WithMany()
            .HasForeignKey(c => c.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Club>()
            .HasIndex(c => c.InviteCode)
            .IsUnique();

        modelBuilder.Entity<UserAccount>().HasData(
            new UserAccount
            {
                Id = 1,
                Name = "Demo User",
                Username = "username",
                Email = "demo@sways.com",
                PasswordHash = "password123", 
                Gender = "she/her",
                AboutMe = "This is a simple about section for the user.",
                TwitterLink = "#",
                InstagramLink = "#",
                DiscordLink = "#",
                MailLink = "#",
                SpotifyLink = "#",
                TikTokLink = "#",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
