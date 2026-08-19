namespace Sways.Models;

public class PollOption
{
    public int Id { get; set; }
    public int PollPostId { get; set; }
    public PollPost? PollPost { get; set; }
    public string Text { get; set; } = string.Empty;
}
