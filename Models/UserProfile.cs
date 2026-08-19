namespace Sways.Models;

public class UserProfile
{
    public int Id { get; set; }
    public string Name { get; set; } = "name";
    public string Username { get; set; } = "username";
    public string Gender { get; set; } = "she/her";
    public string AboutMe { get; set; } = "This is a simple about section for the user.";
    
    // Social Links
    public string? TwitterLink { get; set; } = "#";
    public string? InstagramLink { get; set; } = "#";
    public string? DiscordLink { get; set; } = "#";
    public string? MailLink { get; set; } = "#";
    public string? SpotifyLink { get; set; } = "#";
    public string? TikTokLink { get; set; } = "#";
}
