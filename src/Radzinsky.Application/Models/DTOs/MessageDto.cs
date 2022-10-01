namespace Radzinsky.Application.Models.DTOs;

public class MessageDto
{
    public int Id { get; set; }
    public string Text { get; set; } = null!;
    public string NormalizedText { get; set; } = null!;
    public UserDto Sender { get; set; } = null!;
    public ChatDto Chat { get; set; } = null!;
    public MessageDto? ReplyTarget { get; set; } = null!;
    public bool IsReplyToMe { get; set; }
    public bool IsPrivate { get; set; }
    public bool StartsWithMyName { get; set; }
}