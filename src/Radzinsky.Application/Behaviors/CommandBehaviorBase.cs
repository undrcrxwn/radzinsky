using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Delegates;
using Radzinsky.Application.Misc;
using Radzinsky.Application.Misc.Attributes;
using Radzinsky.Application.Models.Contexts;
using Serilog;
using Telegram.Bot.Types.Enums;

namespace Radzinsky.Application.Behaviors;

public abstract class CommandBehaviorBase : IBehavior
{
    private readonly ICommandsService _commands;
    private readonly IResourcesService _resources;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly CommandContext _commandContext;
    private readonly IStateService _states;

    public CommandBehaviorBase(
        ICommandsService commands,
        IResourcesService resources,
        IServiceScopeFactory scopeFactory,
        CommandContext commandContext,
        IStateService states)
    {
        _commands = commands;
        _resources = resources;
        _scopeFactory = scopeFactory;
        _commandContext = commandContext;
        _states = states;
    }

    public async Task HandleAsync(BehaviorContext context, BehaviorContextHandler next)
    {
        await Task.Yield();
        
        if (context.Update.Type != UpdateType.Message)
        {
            await next(context);
            return;
        }

        var message = context.Update.Message!;

        // Fill command context
        _commandContext.Update = context.Update;
        _commandContext.Message = message;
        _commandContext.Payload = message.NormalizedText;

        // Extract command from checkpoint if possible
        var checkpoint = context.GetCheckpoint();
        if (checkpoint is not null && checkpoint.HandlerTypeName.EndsWith("Command"))
        {
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

        if (checkpoint is { Name: "BotMentioned" })
            context.ResetCheckpoint();

        using var scope = _scopeFactory.CreateScope();
        var command = _commands.GetCommandInstance(scope, _commandContext.HandlerTypeName);

        var shouldRunWithPersistedState =
            command.GetType().GetMethod(nameof(ICommand.ExecuteAsync))!
                .GetCustomAttribute<PersistedAsyncStateAttribute>() is not null;

        if (shouldRunWithPersistedState)
        {
            var synchronizationContext = new PersistentSynchronizationContext(_states);
            SynchronizationContext.SetSynchronizationContext(synchronizationContext);
        }

        try
        {
            await command.ExecuteAsync(_commandContext, new CancellationTokenSource().Token);
        }
        catch(AsyncOperationInterruptedException) {}
    }

    protected abstract bool FillCommandContext(CommandContext commandContext, BehaviorContext behaviorContext);
}