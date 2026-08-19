namespace Sways.Models;

public class BlogPost
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public UserAccount? User { get; set; }
    
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? MediaInfo { get; set; }
    public string? MediaFilePath { get; set; }
    public DateTime DatePosted { get; set; } = DateTime.UtcNow;
    public DateTime? LastEdited { get; set; }
    public string Visibility { get; set; } = "Public"; // Public, Saved, Link

    public List<PostLike> Likes { get; set; } = new();
    public List<PostComment> Comments { get; set; } = new();
}
