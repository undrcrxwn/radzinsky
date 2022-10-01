using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models.Checkpoints;
using Radzinsky.Application.Models.DTOs;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Radzinsky.Application.Models.Contexts;

public class MessageContext
{
    public MessageDto Message = null!;
    public Checkpoint? Checkpoint;
    public ITelegramBotClient Bot;
        
    protected readonly IInteractionService Interaction;

    public MessageContext(ITelegramBotClient bot, IInteractionService interaction)
    {
        Bot = bot;
        Interaction = interaction;
    }
    
    public void ResetCheckpoint()
    {
        Interaction.ResetCheckpoint(Message.Sender.Id);
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
            await Interaction.SetPreviousReplyMessageIdAsync(
                handlerTypeName, message.Chat.Id, message.MessageId);

        return message.MessageId;
    }
    
    public async Task DeleteMessageAsync(int messageId) =>
        await Bot.DeleteMessageAsync(Message.Chat.Id, messageId);
}