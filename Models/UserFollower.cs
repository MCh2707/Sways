namespace Sways.Models;

public class UserFollower
{
    public int Id { get; set; }
    public int FollowerId { get; set; }
    public UserAccount? Follower { get; set; }
    
    public int FollowingId { get; set; }
    public UserAccount? Following { get; set; }
    
    public DateTime FollowedAt { get; set; } = DateTime.UtcNow;
}
