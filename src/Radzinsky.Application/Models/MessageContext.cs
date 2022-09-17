using Radzinsky.Application.Abstractions;
using Radzinsky.Domain.Models;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Radzinsky.Application.Models;

public class MessageContext
{
    public Message Message;
    public Checkpoint? Checkpoint;
    public ITelegramBotClient Bot;
        
    protected readonly IInteractionService _interaction;

    public MessageContext(ITelegramBotClient bot, IInteractionService interaction)
    {
        Bot = bot;
        _interaction = interaction;
    }
    
    public void ResetCheckpoint()
    {
        _interaction.ResetCheckpoint(Message.Sender.Id);
        Checkpoint = null;
    }
    
    public virtual async Task<int> ReplyAsync(
        string text,
        ParseMode? parseMode = null,
        bool? disableWebPagePreview = null,
        string? handlerTypeName = null)
    {
        var message = await Bot.SendTextMessageAsync(Message.Chat.Id, text, parseMode,
            disableWebPagePreview: disableWebPagePreview ?? true);

        if (handlerTypeName is not null)
            await _interaction.SetPreviousReplyMessageIdAsync(
                handlerTypeName, message.Chat.Id, message.MessageId);

        return message.MessageId;
    }
    
    public async Task DeleteMessageAsync(int messageId) =>
        await Bot.DeleteMessageAsync(Message.Chat.Id, messageId);
}