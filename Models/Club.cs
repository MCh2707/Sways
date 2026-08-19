namespace Sways.Models;

public class Club
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string InviteCode { get; set; } = Guid.NewGuid().ToString("N")[..8];
    public string? IconUrl { get; set; }
    public string? BannerUrl { get; set; }
    public string Category { get; set; } = "Engineering";
    
    public int CreatedByUserId { get; set; }
    public UserAccount? CreatedByUser { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<ClubMember> Members { get; set; } = new();
    public List<ClubChannel> Channels { get; set; } = new();
}
