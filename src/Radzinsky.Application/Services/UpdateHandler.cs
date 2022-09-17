using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models;
using Radzinsky.Persistence;
using Serilog;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Message = Radzinsky.Domain.Models.Message;

namespace Radzinsky.Application.Services;

public class UpdateHandler : IUpdateHandler
{
    private readonly IEnumerable<IBehavior> _behaviors;
    private readonly ICommandsService _commands;
    private readonly IResourcesService _resources;
    private readonly ILinguisticParser _parser;
    private readonly IInteractionService _interaction;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IKeyboardLayoutTranslator _keyboardLayoutTranslator;
    private readonly ApplicationDbContext _dbContext;
    private readonly CommandContext _commandContext;
    private readonly BehaviorContext _behaviorContext;

    public UpdateHandler(
        IEnumerable<IBehavior> behaviors,
        ICommandsService commands,
        IResourcesService resources,
        ILinguisticParser parser,
        IInteractionService interaction,
        IServiceScopeFactory scopeFactory,
        IKeyboardLayoutTranslator keyboardLayoutTranslator,
        ApplicationDbContext dbContext,
        CommandContext commandContext,
        BehaviorContext behaviorContext)
    {
        _behaviors = behaviors;
        _commands = commands;
        _resources = resources;
        _parser = parser;
        _interaction = interaction;
        _scopeFactory = scopeFactory;
        _keyboardLayoutTranslator = keyboardLayoutTranslator;
        _dbContext = dbContext;
        _commandContext = commandContext;
        _behaviorContext = behaviorContext;
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

        await FillBehaviorContextAsync(_behaviorContext, update.Message);


        using var enumerator = _behaviors.GetEnumerator();
        await RunNextBehaviorAsync(_behaviorContext);

        async Task RunNextBehaviorAsync(BehaviorContext context)
        {
            if (enumerator.MoveNext())
            {
                var behaviorTypeName = enumerator.Current.GetType().FullName;
                Log.Warning(behaviorTypeName);
                context.Resources =
                    _resources.GetBehaviorResources(behaviorTypeName);
                
                await enumerator.Current.HandleAsync(_behaviorContext, RunNextBehaviorAsync);
            }
        }
    }

    private async Task FillBehaviorContextAsync(BehaviorContext context, Telegram.Bot.Types.Message message)
    {
        context.Message = message.Adapt<Message>();
        context.Message.NormalizedText = _keyboardLayoutTranslator.Translate(context.Message.Text);
        context.Message.IsReplyToMe = context.Message.Sender.Id == context.Bot.BotId;
        context.Message.IsPrivate = message.Chat.Type == ChatType.Private;
        context.Checkpoint = _interaction.TryGetCurrentCheckpoint(context.Message.Sender.Id);
    }
}