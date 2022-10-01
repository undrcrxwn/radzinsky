using Radzinsky.Domain.Models.Entities;

namespace Radzinsky.Application.Models.DTOs;

public class MessageDto
{
    public int Id { get; set; }
    public string Text { get; set; }
    public string NormalizedText { get; set; }
    public UserDto Sender { get; set; }
    public ChatDto Chat { get; set; }
    public MessageDto? ReplyTarget { get; set; }
    public bool IsReplyToMe { get; set; }
    public bool IsPrivate { get; set; }
    public bool StartsWithMyName { get; set; }
}