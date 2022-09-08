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

#if DEBUG
            throw;
#endif
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
        if (mention is null)
        {
            Log.Information("No mention found in message");
            return;
        }
        
        var rest = message.Text[mention.Segment.Length..].TrimStart();
        if (string.IsNullOrWhiteSpace(rest))
        {
            Log.Information("Single mention message detected");
            context.Resources = _commands.First(x => x.CommandTypeName == typeof(MentionCommand).FullName);
            await new MentionCommand().ExecuteAsync(context);
            return;
        }

        // Parse command alias
        var alias = _parser.TryParseCommandAliasFromBeginning(rest);
        if (alias is null)
        {
            Log.Information("No command alias found in message");
            context.Resources = _commands.First(x => x.CommandTypeName == typeof(MisunderstandingCommand).FullName);
            await new MisunderstandingCommand().ExecuteAsync(context);
            return;
        }
        
        context.Payload = rest[alias.Segment.Length..].TrimStart();

        // Find command
        context.Resources = _commands.First(x => x.Aliases.Contains(alias.Case));
        var command = GetCommandInstanceByName(context.Resources.CommandTypeName);

        // Execute command
        await command.ExecuteAsync(context);
    }

    private ICommand GetCommandInstanceByName(string commandTypeName)
    {
        var commandType = Type.GetType(commandTypeName);
        using var scope = _scopeFactory.CreateScope();
        return (ICommand)scope.ServiceProvider.GetRequiredService(commandType);
    }
}