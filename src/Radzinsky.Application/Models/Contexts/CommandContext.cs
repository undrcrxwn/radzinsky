using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models.Checkpoints;
using Radzinsky.Application.Models.Resources;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Radzinsky.Application.Models.Contexts;

public class CommandContext : MessageContext
{
    public string CommandTypeName = null!;
    public CommandResources? Resources;
    public string Payload = null!;

    public CommandContext(ITelegramBotClient bot, IInteractionService interaction)
        : base(bot, interaction) { }

    public override async Task<int> ReplyAsync(
        string text,
        ParseMode? parseMode = null,
        bool? disableWebPagePreview = null,
        string? handlerTypeName = null) =>
        await base.ReplyAsync(text, parseMode, disableWebPagePreview,
            handlerTypeName ?? CommandTypeName);

    public Checkpoint SetCommandCheckpoint(string name)
    {
        Checkpoint = Interaction.IssueCheckpoint(name, CommandTypeName, Message.Sender.Id);
        return Checkpoint;
    }

    public async Task DeletePreviousReplyAsync()
    {
        var messageId = await Interaction.TryGetPreviousReplyMessageIdAsync(
            CommandTypeName, Message.Chat.Id);
        
        if (messageId is not null)
            await DeleteMessageAsync(messageId.Value);
    }
}