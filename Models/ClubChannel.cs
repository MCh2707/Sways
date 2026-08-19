namespace Sways.Models;

public class ClubChannel
{
    public int Id { get; set; }
    public int ClubId { get; set; }
    public Club? Club { get; set; }

    public string Name { get; set; } = "general"; // e.g. "general", "rules", "media"
    public string Description { get; set; } = string.Empty;
    public string WritePermission { get; set; } = "everyone"; // "everyone", "moderator", "admin"

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
