using Mapster;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models;
using Serilog;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Message = Radzinsky.Domain.Models.Message;

namespace Radzinsky.Application.Services;

public class UpdateHandler : IUpdateHandler
{
    private readonly IEnumerable<IBehavior> _behaviors;
    private readonly IResourcesService _resources;
    private readonly IInteractionService _interaction;
    private readonly IKeyboardLayoutTranslator _keyboardLayoutTranslator;
    private readonly BehaviorContext _behaviorContext;

    public UpdateHandler(
        IEnumerable<IBehavior> behaviors,
        IResourcesService resources,
        IInteractionService interaction,
        IKeyboardLayoutTranslator keyboardLayoutTranslator,
        BehaviorContext behaviorContext)
    {
        _behaviors = behaviors;
        _resources = resources;
        _interaction = interaction;
        _keyboardLayoutTranslator = keyboardLayoutTranslator;
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

        FillBehaviorContext(_behaviorContext, update.Message);

        var enumerator = _behaviors.GetEnumerator();
        await RunNextBehaviorAsync(_behaviorContext);

        async Task RunNextBehaviorAsync(BehaviorContext context)
        {
            if (enumerator.MoveNext())
            {
                context.Resources =
                    _resources.GetBehaviorResources(enumerator.Current.GetType().FullName!);
                
                await enumerator.Current.HandleAsync(_behaviorContext, RunNextBehaviorAsync);
            }
        }
    }

    private void FillBehaviorContext(BehaviorContext context, Telegram.Bot.Types.Message message)
    {
        context.Message = message.Adapt<Message>();
        context.Message.NormalizedText = _keyboardLayoutTranslator.Translate(context.Message.Text);
        context.Message.IsReplyToMe = context.Message.Sender.Id == context.Bot.BotId;
        context.Message.IsPrivate = message.Chat.Type == ChatType.Private;
        context.Checkpoint = _interaction.TryGetCurrentCheckpoint(context.Message.Sender.Id);
    }
}