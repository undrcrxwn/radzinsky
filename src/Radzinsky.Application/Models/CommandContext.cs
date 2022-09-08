using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Radzinsky.Application.Models;

public class CommandContext
{
    public ITelegramBotClient Bot;
    public Message Message;
    public string Payload;
    public CommandResources Resources;

    public async Task ReplyAsync(string text, ParseMode? parseMode = null, bool? disableWebPagePreview = null) =>
        await Bot.SendTextMessageAsync(Message.Chat.Id, text, parseMode, disableWebPagePreview: disableWebPagePreview);
}