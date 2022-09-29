using Microsoft.Extensions.DependencyInjection;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Delegates;
using Radzinsky.Application.Models.Checkpoints;
using Radzinsky.Application.Models.Contexts;

namespace Radzinsky.Application.Behaviors.Base;

public abstract class CommandBehaviorBase : IBehavior
{
    private readonly ICommandsService _commands;
    private readonly IResourcesService _resources;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly CommandContext _commandContext;

    public CommandBehaviorBase(
        ICommandsService commands,
        IResourcesService resources,
        IServiceScopeFactory scopeFactory,
        CommandContext commandContext)
    {
        _commands = commands;
        _resources = resources;
        _scopeFactory = scopeFactory;
        _commandContext = commandContext;
    }

    public async Task HandleAsync(BehaviorContext context, BehaviorContextHandler next)
    {
        // Fill command context
        _commandContext.Message = context.Message;
        _commandContext.Checkpoint = context.Checkpoint;

        // Extract command from checkpoint if possible
        if (_commandContext.Checkpoint is CommandCheckpoint commandCheckpoint)
        {
            _commandContext.Payload = context.Message.NormalizedText;
            _commandContext.CommandTypeName = commandCheckpoint.CommandTypeName;
            _commandContext.Resources = _resources.GetCommandResources(_commandContext.CommandTypeName);
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
        
        if (context.Checkpoint is MentionCheckpoint)
            context.ResetCheckpoint();
        
        using var scope = _scopeFactory.CreateScope();
        var command = _commands.GetCommandInstance(scope, _commandContext.CommandTypeName);
        await command.ExecuteAsync(_commandContext, new CancellationTokenSource().Token);
    }

    protected abstract bool FillCommandContext(CommandContext commandContext, BehaviorContext behaviorContext);
}