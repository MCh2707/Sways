namespace Sways.Models;

public class ClubMember
{
    public int Id { get; set; }
    public int ClubId { get; set; }
    public Club? Club { get; set; }
    
    public int UserId { get; set; }
    public UserAccount? User { get; set; }
    
    public string Role { get; set; } = "member"; // "admin", "moderator", "member"
    public bool IsBlocked { get; set; } = false;

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}
