using Telegram.Bot.Types.ReplyMarkups;

namespace Radzinsky.Application.Abstractions;

public interface IButtonFactory
{
    public InlineKeyboardButton CreateCallbackDataButton(string label, string data);
}