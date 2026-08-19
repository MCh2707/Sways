namespace Sways.Models;

public class DirectMessage
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public UserAccount? Sender { get; set; }
    
    public int ReceiverId { get; set; }
    public UserAccount? Receiver { get; set; }
    
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;
}
