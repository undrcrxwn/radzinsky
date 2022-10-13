using Mapster;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models.Contexts;
using Radzinsky.Application.Models.DTOs;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Radzinsky.Application.Services;

public class UpdateHandler : IUpdateHandler
{
    private readonly IEnumerable<IBehavior> _behaviors;
    private readonly IResourcesService _resources;
    private readonly ITelegramBotClient _bot;
    private readonly IKeyboardLayoutTranslator _keyboardLayoutTranslator;
    private readonly ILinguisticParser _parser;
    private readonly BehaviorContext _behaviorContext;

    public UpdateHandler(
        IEnumerable<IBehavior> behaviors,
        IResourcesService resources,
        ITelegramBotClient bot,
        IKeyboardLayoutTranslator keyboardLayoutTranslator,
        ILinguisticParser parser,
        BehaviorContext behaviorContext)
    {
        _behaviors = behaviors;
        _resources = resources;
        _bot = bot;
        _keyboardLayoutTranslator = keyboardLayoutTranslator;
        _parser = parser;
        _behaviorContext = behaviorContext;
    }

    public async Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
#if DEBUG
        Log.Debug("Received update: {@0}", update);
#else
        Log.Information("Received message ({0}) from chat {1}: {2}",
            update.Message.MessageId, update.Message.Chat.Id, update.Message.Text);
#endif

        FillBehaviorContext(_behaviorContext, update);

        var enumerator = _behaviors.GetEnumerator();
        await RunNextBehaviorAsync(_behaviorContext);

        async Task RunNextBehaviorAsync(BehaviorContext context)
        {
            var previousBehaviorTypeName = context.HandlerTypeName;
            var previousResources = context.Resources;

            if (enumerator.MoveNext())
            {
                context.HandlerTypeName = enumerator.Current.GetType().FullName!;
                context.Resources =
                    _resources.GetBehaviorResources(context.HandlerTypeName);

                Log.Debug("Entering behavior {0}", context.HandlerTypeName);

                try
                {
                    await enumerator.Current.HandleAsync(context, RunNextBehaviorAsync);
                }
                finally
                {
                    Log.Debug("Leaving behavior {0}", context.HandlerTypeName);

                    context.HandlerTypeName = previousBehaviorTypeName;
                    context.Resources = previousResources;
                }
            }
        }
    }

    private void FillBehaviorContext(BehaviorContext context, Update update)
    {
        context.Update = update.Adapt<UpdateDto>();
        context.Update.Message!.NormalizedText = _keyboardLayoutTranslator.Translate(context.Update.Message!.Text);
        context.Update.Message.IsReplyToMe = context.Update.Message.ReplyTarget?.Sender.Id == _bot.BotId;
        context.Update.Message.IsPrivate = update.Message!.Chat.Type == ChatType.Private;
        context.Update.Message.StartsWithMyName =
            _parser.TryParseMentionFromBeginning(context.Update.Message.NormalizedText) is not null;
    }
}