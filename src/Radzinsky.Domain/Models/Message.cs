namespace Radzinsky.Domain.Models;

public class Message
{
    public int Id { get; set; }
    public string Text { get; set; }
    public string NormalizedText { get; set; }
    public User Sender { get; set; }
    public Chat Chat { get; set; }
    public Message? ReplyTarget { get; set; }
    public bool IsReplyToMe { get; set; }
    public bool IsPrivate { get; set; }
}