using Microsoft.Extensions.DependencyInjection;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Behaviors.Base;
using Radzinsky.Application.Models.Checkpoints;
using Radzinsky.Application.Models.Contexts;
using Serilog;

namespace Radzinsky.Application.Behaviors;

public class LinguisticCommandBehavior : CommandBehaviorBase
{
    private readonly ILinguisticParser _parser;
    private readonly ICommandsService _commands;
    private readonly IResourcesService _resources;

    public LinguisticCommandBehavior(
        ILinguisticParser parser,
        ICommandsService commands,
        IResourcesService resources,
        IServiceScopeFactory scopeFactory,
        CommandContext commandContext)
        : base(commands, resources, scopeFactory, commandContext)
    {
        _parser = parser;
        _commands = commands;
        _resources = resources;
    }

    protected override bool FillCommandContext(CommandContext commandContext, BehaviorContext behaviorContext)
    {
        commandContext.Payload = commandContext.Message.NormalizedText;
        
        // Extract command from checkpoint if possible
        if (commandContext.Checkpoint is CommandCheckpoint commandCheckpoint)
        {
            commandContext.Resources = _resources.GetCommandResources(commandCheckpoint.CommandTypeName);
            return true;
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

        commandContext.CommandTypeName = _commands.GetCommandTypeNameByAlias(alias.Case);
        commandContext.Resources = _resources.GetCommandResources(commandContext.CommandTypeName);
        commandContext.Payload = commandContext.Payload[alias.Segment.Length..].TrimStart();
        return true;
    }
}