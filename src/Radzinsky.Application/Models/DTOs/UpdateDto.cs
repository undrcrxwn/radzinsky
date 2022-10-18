using Telegram.Bot.Types.Enums;

namespace Radzinsky.Application.Models.DTOs;

public class UpdateDto
{
    public UpdateType Type;
    public MessageDto? Message;
    public CallbackQueryDto? CallbackQuery;
    
    public long? InteractorUserId =>
        Message?.Sender.Id ??
        CallbackQuery?.Sender.Id;
    
    public long? ChatId =>
        Message?.Chat.Id ??
        CallbackQuery?.Message.Chat.Id;
}