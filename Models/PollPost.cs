namespace Sways.Models;

public class PollPost
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public UserAccount? User { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime DatePosted { get; set; } = DateTime.UtcNow;
    public bool IsSingleChoice { get; set; } = true;
    
    public List<PollOption> Options { get; set; } = new();
    public List<PollVote> Votes { get; set; } = new();
    public List<PostLike> Likes { get; set; } = new();
    public List<PostComment> Comments { get; set; } = new();
}
