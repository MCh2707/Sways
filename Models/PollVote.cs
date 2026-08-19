namespace Sways.Models;

public class PollVote
{
    public int Id { get; set; }
    public int PollPostId { get; set; }
    public PollPost? PollPost { get; set; }
    public int PollOptionId { get; set; }
    public PollOption? PollOption { get; set; }
    public int UserId { get; set; }
    public UserAccount? User { get; set; }
    public DateTime VotedAt { get; set; } = DateTime.UtcNow;
}
