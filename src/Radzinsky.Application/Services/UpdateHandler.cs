using Mapster;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models;
using Radzinsky.Application.Models.Contexts;
using Serilog;
using Telegram.Bot;
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
            var previousBehaviorTypeName = context.BehaviorTypeName;
            var previousResources = context.Resources;
            
            if (enumerator.MoveNext())
            {
                context.BehaviorTypeName = enumerator.Current.GetType().FullName!;
                context.Resources =
                    _resources.GetBehaviorResources(context.BehaviorTypeName);
                
                Log.Debug("Entering behavior {0}", context.BehaviorTypeName);
                
                try
                {
                    await enumerator.Current.HandleAsync(context, RunNextBehaviorAsync);
                }
                finally
                {
                    Log.Debug("Leaving behavior {0}", context.BehaviorTypeName);
                    
                    context.BehaviorTypeName = previousBehaviorTypeName;
                    context.Resources = previousResources;
                }
            }
        }
    }

    private void FillBehaviorContext(BehaviorContext context, Telegram.Bot.Types.Message message)
    {
        context.Message = message.Adapt<Message>();
        context.Message.NormalizedText = _keyboardLayoutTranslator.Translate(context.Message.Text);
        context.Message.IsReplyToMe = context.Message.ReplyTarget?.Sender.Id == context.Bot.BotId;
        context.Message.IsPrivate = message.Chat.Type == ChatType.Private;
        context.Checkpoint = _interaction.TryGetCurrentCheckpoint(context.Message.Sender.Id);
    }
}