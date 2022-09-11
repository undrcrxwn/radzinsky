using Radzinsky.Application.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Radzinsky.Application.Models;

public class CommandContext
{
    public ITelegramBotClient Bot;
    public Radzinsky.Domain.Models.User User;
    public Checkpoint? Checkpoint;
    public Message Message;
    public Message? TargetMessage;
    public bool IsReplyToMe;
    public bool IsPrivateMessage;
    public string Payload;
    public CommandResources? Resources;

    private readonly IInteractionService _interaction;

    public CommandContext(ITelegramBotClient bot, IInteractionService interaction)
    {
        Bot = bot;
        _interaction = interaction;
    }

    public Checkpoint SetMentionCheckpoint(string name)
    {
        Checkpoint = _interaction.IssueMentionCheckpoint(name, Message.From.Id);
        return Checkpoint;
    }

    public Checkpoint SetCommandCheckpoint(string name)
    {
        Checkpoint = _interaction.IssueCommandCheckpoint(name, Resources.CommandTypeName, Message.From.Id);
        return Checkpoint;
    }

    public void ResetCheckpoint()
    {
        _interaction.ResetCheckpoint(Message.From.Id);
        Checkpoint = null;
    }

    public async Task<Message> ReplyAsync(
        string text,
        ParseMode? parseMode = null,
        bool? disableWebPagePreview = null,
        bool preventTracking = false)
    {
        var message = await Bot.SendTextMessageAsync(Message.Chat.Id, text, parseMode,
            disableWebPagePreview: disableWebPagePreview);

        if (!preventTracking)
            await _interaction.SetPreviousReplyMessageIdAsync(
                Resources.CommandTypeName, message.Chat.Id, message.MessageId);

        return message;
    }

    public async Task DeletePreviousReplyAsync()
    {
        var messageId = await _interaction.TryGetPreviousReplyMessageIdAsync(
            Resources.CommandTypeName, Message.Chat.Id);
        if (messageId is not null)
            await DeleteMessageAsync(messageId.Value);
    }

    public async Task DeleteMessageAsync(int messageId) =>
        await Bot.DeleteMessageAsync(Message.Chat.Id, messageId);
}