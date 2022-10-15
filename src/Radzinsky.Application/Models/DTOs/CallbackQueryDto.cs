namespace Radzinsky.Application.Models.DTOs;

public class CallbackQueryDto
{
    public string CallbackHandlerTypeNameHash { get; set; } = null!;
    public string Data { get; set; } = null!;
}