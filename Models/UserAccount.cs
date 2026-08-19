namespace Sways.Models;

public class UserAccount
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Gender { get; set; } = "she/her";
    public string AboutMe { get; set; } = "This is a simple about section for the user.";
    public string Bio { get; set; } = string.Empty;
    public DateTime? BirthDate { get; set; }
    public string? ProfilePicUrl { get; set; }
    public string? BannerUrl { get; set; }

    // Additional info & Location
    public string Country { get; set; } = "Georgia";
    public string City { get; set; } = "Tbilisi";
    public string Languages { get; set; } = "Georgian";

    // Education
    public string EducationLevel { get; set; } = "school"; // school, college, university, other
    public string SchoolName { get; set; } = string.Empty;
    public string SchoolAddress { get; set; } = string.Empty;
    public string SchoolClass { get; set; } = string.Empty;
    public bool ShowSchoolPublicly { get; set; } = true;

    public string UniversityName { get; set; } = string.Empty;
    public string UniversityAddress { get; set; } = string.Empty;
    public string UniversityEmail { get; set; } = string.Empty;

    public string CollegeName { get; set; } = string.Empty;
    public string CollegeAddress { get; set; } = string.Empty;
    public string CollegeEmail { get; set; } = string.Empty;

    // Workplace
    public string WorkplaceCompany { get; set; } = string.Empty;
    public string WorkplacePosition { get; set; } = string.Empty;

    // Social Links
    public string? TwitterLink { get; set; } = "#";
    public string? InstagramLink { get; set; } = "#";
    public string? DiscordLink { get; set; } = "#";
    public string? MailLink { get; set; } = "#";
    public string? SpotifyLink { get; set; } = "#";
    public string? TikTokLink { get; set; } = "#";
    public string? FacebookLink { get; set; } = "#";

    // Achievements & Equipped Badges (comma-separated badge IDs, e.g. "first-article,young-journalist")
    public string EquippedBadges { get; set; } = "first-article,young-journalist";

    // Privacy Controls
    public bool IsPublicProfile { get; set; } = true;
    public bool IsSchoolOnly { get; set; } = false;
    public bool HideEmail { get; set; } = false;
    public bool HideLocation { get; set; } = false;

    // Theme Preferences (Persisted in DB)
    public string ThemeMode { get; set; } = "open"; // "open" (Light Mode) or "dark"
    public string AccentColor { get; set; } = "blue"; // "blue", "pink", "green", "purple", "orange"

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

