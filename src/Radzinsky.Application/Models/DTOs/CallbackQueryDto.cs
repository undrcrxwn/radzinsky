namespace Radzinsky.Application.Models.DTOs;

public class CallbackQueryDto
{
    public string Id { get; set; }
    public UserDto Sender { get; set; } = null!;
    public MessageDto Message { get; set; } = null!;
    public string CallbackHandlerTypeNameHash { get; set; } = null!;
    public string Data { get; set; } = null!;
}