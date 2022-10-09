using Microsoft.Extensions.DependencyInjection;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Delegates;
using Radzinsky.Application.Models.Contexts;
using Radzinsky.Application.Models.DTOs;
using Telegram.Bot.Types.Enums;

namespace Radzinsky.Application.Behaviors;

public abstract class CommandBehaviorBase : IBehavior
{
    private readonly ICommandsService _commands;
    private readonly IResourcesService _resources;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly CommandContext _commandContext;
    private readonly ICheckpointMemoryService _checkpoints;

    public CommandBehaviorBase(
        ICommandsService commands,
        IResourcesService resources,
        IServiceScopeFactory scopeFactory,
        CommandContext commandContext,
        ICheckpointMemoryService checkpoints)
    {
        _commands = commands;
        _resources = resources;
        _scopeFactory = scopeFactory;
        _commandContext = commandContext;
        _checkpoints = checkpoints;
    }

    public async Task HandleAsync(BehaviorContext context, BehaviorContextHandler next)
    {
        if (context.Update.Type != UpdateType.Message)
        {
            await next(context);
            return;
        }
        
        var message = context.Update.Message!;
        
        // Fill command context
        _commandContext.Message = message;
        
        // Extract command from checkpoint if possible
        var checkpoint = context.GetCheckpoint();
        if (checkpoint is CommandCheckpoint)
        {
            _commandContext.Payload = message.NormalizedText;
            _commandContext.HandlerTypeName = checkpoint.HandlerTypeName;
            _commandContext.Resources = _resources.GetCommandResources(_commandContext.HandlerTypeName);
        }
        else
        {
            var considerCommand = FillCommandContext(_commandContext, context);
            if (!considerCommand || _commandContext.Resources is null)
            {
                await next(context);
                return;
            }
        }
        
        var mentionCheckpoint =  _checkpoints.TryGetCurrentCheckpoint(context.Update.InteractorUserId!.Value, typeof(MentionBehavior).FullName!);
        if (mentionCheckpoint is { Name: "BotMentioned" })
            context.ResetCheckpoint();
        
        using var scope = _scopeFactory.CreateScope();
        var command = _commands.GetCommandInstance(scope, _commandContext.CommandTypeName);
        await command.ExecuteAsync(_commandContext, new CancellationTokenSource().Token);
    }

    protected abstract bool FillCommandContext(CommandContext commandContext, BehaviorContext behaviorContext);
}