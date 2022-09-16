using Microsoft.Extensions.DependencyInjection;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Commands;
using Radzinsky.Application.Models;
using Radzinsky.Persistence;
using Serilog;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Radzinsky.Application.Services;

public class UpdateHandler : IUpdateHandler
{
    private readonly ICommandsService _commands;
    private readonly ILinguisticParser _parser;
    private readonly IInteractionService _interaction;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ApplicationDbContext _dbContext;
    private readonly CommandContext _context;

    public UpdateHandler(
        ICommandsService commands,
        ILinguisticParser parser,
        IInteractionService interaction,
        IServiceScopeFactory scopeFactory,
        ApplicationDbContext dbContext,
        CommandContext context)
    {
        _commands = commands;
        _parser = parser;
        _interaction = interaction;
        _scopeFactory = scopeFactory;
        _dbContext = dbContext;
        _context = context;
    }

    public async Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        // Handle text messages only
        if (update.Message?.Text is null)
            return;

#if DEBUG
        Log.Debug("Received message: {@0}", update.Message);
#else
        Log.Information("Received message ({0}) from chat {1}: {2}",
            update.Message.MessageId, update.Message.Chat.Id, update.Message.Text);
#endif

        try
        {
            await HandleTextMessageAsync(update.Message);
        }
        catch (Exception e)
        {
            Log.Error(e, "Unhandled exception raised while handling update: {@0}", update);

#if DEBUG
            throw;
#endif
        }
    }

    private async Task HandleTextMessageAsync(Message message)
    {
        // Fill command context
        await FillContextAsync(_context, message);

        // Find and execute command if possible
        if (_context.Resources is null)
            return;
        
        var scope = _scopeFactory.CreateScope();
        var command = _commands.GetCommandInstance(scope, _context.Resources.CommandTypeName);
        await command.ExecuteAsync(_context, new CancellationTokenSource().Token);
    }

    private async Task FillContextAsync(CommandContext context, Message message)
    {
        context.Message = message;
        context.TargetMessage = message.ReplyToMessage;
        context.IsReplyToMe = context.Message.ReplyToMessage?.From?.Id == context.Bot.BotId;
        context.IsPrivateMessage = message.Chat.Type == ChatType.Private;
        context.Payload = message.Text!;
        context.User = await _dbContext.Users.FindAsync(message.From.Id);
        context.Checkpoint = _interaction.TryGetCurrentCheckpoint(message.From.Id);

        // Extract command from checkpoint if possible
        if (context.Checkpoint is CommandCheckpoint commandCheckpoint)
        {
            context.Resources = _commands.GetResources(commandCheckpoint.CommandTypeName);
            return;
        }

        // Parse mention
        var mention = _parser.TryParseMentionFromBeginning(context.Payload);
        if (mention is null &&
            context.Checkpoint is null &&
            !context.IsReplyToMe &&
            !context.IsPrivateMessage)
        {
            Log.Information("No mention found in message");
            return;
        }

        // Remove possible mention from payload
        if (mention is not null)
        {
            context.Payload = context.Payload[mention.Segment.Length..].TrimStart();
            if (string.IsNullOrWhiteSpace(context.Payload))
            {
                Log.Information("Single mention message");
                context.Resources = _commands.GetResources<MentionCommand>();
                context.SetMentionCheckpoint("SingleMentionMessage");
                return;
            }
        }

        // Reset mention checkpoint
        if (context.Checkpoint is MentionCheckpoint)
            context.ResetCheckpoint();

        // Find command by alias
        var alias = _parser.TryParseCommandAliasFromBeginning(context.Payload);
        if (alias is null)
        {
            Log.Information("No command alias found in message");
            context.Resources = _commands.GetResources<MisunderstandingCommand>();
            return;
        }

        context.Resources = _commands.GetResourcesByAlias(alias.Case);
        context.Payload = context.Payload[alias.Segment.Length..].TrimStart();
    }
}