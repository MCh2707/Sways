namespace Sways.Models;

public class ClubMessage
{
    public int Id { get; set; }
    public int ClubId { get; set; }
    public Club? Club { get; set; }

    public int ChannelId { get; set; }
    public ClubChannel? Channel { get; set; }

    public int UserId { get; set; }
    public UserAccount? User { get; set; }

    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? LinkUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
