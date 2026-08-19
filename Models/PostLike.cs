namespace Sways.Models;

public class PostLike
{
    public int Id { get; set; }
    public int? BlogPostId { get; set; }
    public BlogPost? BlogPost { get; set; }
    public int? PollPostId { get; set; }
    public PollPost? PollPost { get; set; }
    public int UserId { get; set; }
    public UserAccount? User { get; set; }
    public DateTime LikedAt { get; set; } = DateTime.UtcNow;
}
