using Telegram.Bot.Types.Enums;

namespace Radzinsky.Application.Models.DTOs;

public class UpdateDto
{
    public UpdateType Type;
    public MessageDto? Message;
    
    public long? InteractorUserId => Message?.Sender.Id;
    public long? ChatId => Message?.Chat.Id;
}