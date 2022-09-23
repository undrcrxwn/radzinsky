using Microsoft.Extensions.DependencyInjection;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Behaviors.Base;
using Radzinsky.Application.Models.Checkpoints;
using Radzinsky.Application.Models.Contexts;
using Serilog;

namespace Radzinsky.Application.Behaviors;

public class SlashCommandBehavior : CommandBehaviorBase
{
    private readonly ILinguisticParser _parser;
    private readonly ICommandsService _commands;
    private readonly IResourcesService _resources;

    public SlashCommandBehavior(
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
        if (string.IsNullOrWhiteSpace(commandContext.Message.Text))
            return false;

        // Find command by alias
        var slash = commandContext.Message.Text.Split().First().ToLower();
        if (!slash.StartsWith('/'))
        {
            Log.Information("First word is not a slash");
            return true;
        }
        
        commandContext.CommandTypeName = _commands.GetCommandTypeNameBySlash(slash);
        if (commandContext.CommandTypeName is null)
        {
            Log.Information("No command with the given slash found in message");
            return false;
        }

        commandContext.Resources = _resources.GetCommandResources(commandContext.CommandTypeName);
        commandContext.Payload = commandContext.Message.Text[slash.Length..].TrimStart();
        return true;
    }
}