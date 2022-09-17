using Microsoft.Extensions.DependencyInjection;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Delegates;
using Radzinsky.Application.Extensions;
using Radzinsky.Application.Models;
using Radzinsky.Application.Models.Checkpoints;
using Radzinsky.Application.Models.Contexts;
using Serilog;

namespace Radzinsky.Application.Behaviors;

public class CommandBehavior : IBehavior
{
    private readonly IInteractionService _interaction;
    private readonly ICommandsService _commands;
    private readonly IResourcesService _resources;
    private readonly ILinguisticParser _parser;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly CommandContext _commandContext;

    public CommandBehavior(
        IInteractionService interaction,
        ICommandsService commands,
        IResourcesService resources,
        ILinguisticParser parser,
        IServiceScopeFactory scopeFactory,
        CommandContext commandContext)
    {
        _interaction = interaction;
        _commands = commands;
        _resources = resources;
        _parser = parser;
        _scopeFactory = scopeFactory;
        _commandContext = commandContext;
    }

    public async Task HandleAsync(BehaviorContext context, BehaviorContextHandler next)
    {
        // Fill command context
        var considerCommand = FillCommandContext(_commandContext, context);

        // Find and execute command if possible
        if (!considerCommand)
        {
            await next(context);
            return;
        }

        if (_commandContext.Resources is null)
        {
            context.ReplyAsync(context.Resources.Variants["CannotUnderstandYou"].PickRandom());
            return;
        }

        using (var scope = _scopeFactory.CreateScope())
        {
            var command = _commands.GetCommandInstance(scope, _commandContext.Resources.CommandTypeName);
            await command.ExecuteAsync(_commandContext, new CancellationTokenSource().Token);
        }
    }

    private bool FillCommandContext(CommandContext commandContext, BehaviorContext behaviorContext)
    {
        commandContext.Message = behaviorContext.Message;
        commandContext.Checkpoint = behaviorContext.Checkpoint;
        commandContext.Payload = commandContext.Message.NormalizedText;

        // Extract command from checkpoint if possible
        if (commandContext.Checkpoint is CommandCheckpoint commandCheckpoint)
        {
            commandContext.Resources = _resources.GetCommandResources(commandCheckpoint.CommandTypeName);
            return false;
        }

        // Parse mention
        var mention = _parser.TryParseMentionFromBeginning(commandContext.Payload);
        if (mention is null &&
            commandContext.Checkpoint is null &&
            !commandContext.Message.IsReplyToMe &&
            !commandContext.Message.IsPrivate)
        {
            Log.Information("No mention found in message");
            return false;
        }

        // Remove possible mention from payload
        if (mention is not null)
            commandContext.Payload = commandContext.Payload[mention.Segment.Length..].TrimStart();

        // Reset mention checkpoint
        if (commandContext.Checkpoint is MentionCheckpoint)
            commandContext.ResetCheckpoint();

        if (string.IsNullOrWhiteSpace(commandContext.Payload))
            return false;

        // Find command by alias
        var alias = _parser.TryParseCommandAliasFromBeginning(commandContext.Payload);
        if (alias is null)
        {
            Log.Information("No command alias found in message");
            return true;
        }

        commandContext.Resources = _resources.GetCommandResourcesByAlias(alias.Case);
        commandContext.Payload = commandContext.Payload[alias.Segment.Length..].TrimStart();

        return true;
    }
}