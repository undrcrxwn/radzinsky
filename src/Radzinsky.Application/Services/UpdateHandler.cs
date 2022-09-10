using Microsoft.Extensions.DependencyInjection;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Commands;
using Radzinsky.Application.Models;
using Radzinsky.Persistence;
using Serilog;
using Telegram.Bot.Types;

namespace Radzinsky.Application.Services;

public class UpdateHandler : IUpdateHandler
{
    private readonly ICommandsService _commands;
    private readonly ILinguisticParser _parser;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IInteractionService _interaction;
    private readonly ApplicationDbContext _dbContext;

    public UpdateHandler(
        ICommandsService commands,
        ILinguisticParser parser,
        IServiceScopeFactory scopeFactory,
        IInteractionService interaction,
        ApplicationDbContext dbContext)
    {
        _commands = commands;
        _parser = parser;
        _scopeFactory = scopeFactory;
        _interaction = interaction;
        _dbContext = dbContext;
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
        // Create command context
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CommandContext>();
        await FillContextAsync(context, message);
        
        // Execute command
        var command = _commands.GetCommandInstance(scope, context.Resources.CommandTypeName);
        await command.ExecuteAsync(context, new CancellationTokenSource().Token);
    }

    private async Task FillContextAsync(CommandContext context, Message message)
    {
        context.Payload = message.Text;
        context.User = await _dbContext.Users.FindAsync(message.From.Id);
        context.Checkpoint = _interaction.GetCurrentCheckpoint(context.User.Id);

        // Extract command from checkpoint if possible
        if (context.Checkpoint is not null)
        {
            context.Resources = _commands.GetResources(context.Checkpoint.CommandTypeName);
            return;
        }

        // Parse mention
        var mention = _parser.TryParseMentionFromBeginning(context.Payload);
        if (mention is null)
        {
            Log.Information("No mention found in message");
            return;
        }
        
        context.Payload = context.Payload[mention.Segment.Length..].TrimStart();
        if (string.IsNullOrWhiteSpace(context.Payload))
        {
            Log.Information("Single mention message detected");
            context.Resources = _commands.GetResources<MentionCommand>();
            return;
        }

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