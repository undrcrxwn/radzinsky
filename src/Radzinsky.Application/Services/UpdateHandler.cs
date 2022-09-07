using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Commands;
using Radzinsky.Application.Models;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Radzinsky.Application.Services;

public class UpdateHandler : IUpdateHandler
{
    private readonly ITelegramBotClient _bot;
    private readonly IEnumerable<CommandResources> _commands;
    private readonly ILinguisticParser _parser;
    private readonly IServiceScopeFactory _scopeFactory;

    public UpdateHandler(
        IEnumerable<CommandResources> commands,
        IServiceScopeFactory scopeFactory,
        ILinguisticParser parser,
        ITelegramBotClient bot)
    {
        _commands = commands;
        _scopeFactory = scopeFactory;
        _parser = parser;
        _bot = bot;
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
        }
    }

    private async Task HandleTextMessageAsync(Message message)
    {
        // Create command context
        var context = new CommandContext
        {
            Bot = _bot,
            Message = message
        };
        
        // Parse mention
        var mention = _parser.TryParseMentionFromBeginning(message.Text);
        var rest = message.Text[mention.Segment.Length..];
        if (string.IsNullOrWhiteSpace(rest))
        {
            Log.Information("Single mention message detected");
            context.Resources = _commands.FirstOrDefault(x => x.CommandName == nameof(MentionCommand))
                ?? new CommandResources { CommandName = nameof(MentionCommand) };
            await new MentionCommand().ExecuteAsync(context);
            return;
        }

        // Parse command alias
        var alias = _parser.TryParseCommandAliasFromBeginning(rest);
        context.Payload = rest[mention.Segment.Length..];
        if (alias is null)
        {
            Log.Information("No command alias found in message");
            return;
        }

        // Find command
        context.Resources =
            _commands.FirstOrDefault(x => x.Aliases.Contains(alias.Segment.ToString()))
            ?? new CommandResources { CommandName = nameof(MentionCommand) };
        var command = GetCommandInstanceByName(context.Resources.CommandName);

        // Execute command
        await command.ExecuteAsync(context);
    }

    private ICommand GetCommandInstanceByName(string commandName)
    {
        var commandType = Assembly.GetExecutingAssembly().GetType(commandName);
        using var scope = _scopeFactory.CreateScope();
        return (ICommand)scope.ServiceProvider.GetRequiredService(commandType);
    }
}