using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models.Checkpoints;
using Radzinsky.Application.Models.DTOs;
using Radzinsky.Application.Models.Resources;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Radzinsky.Application.Models.Contexts;

public abstract class ContextBase<TResources> where TResources : ResourcesBase
{
    public string HandlerTypeName = null!;
    public UpdateDto Update = null!;
    public TResources? Resources;

    private readonly ITelegramBotClient _bot;
    private readonly ICheckpointMemoryService _checkpoints;
    private readonly IReplyMemoryService _replies;

    protected ContextBase(
        ITelegramBotClient bot,
        ICheckpointMemoryService checkpoints,
        IReplyMemoryService replies)
    {
        _bot = bot;
        _checkpoints = checkpoints;
        _replies = replies;
    }

    #region Checkpoints

    public void SetCheckpoint(string name, TimeSpan? duration = null)
    {
        var checkpoint = new Checkpoint(name, HandlerTypeName, Update.ChatId);
        SetCheckpoint(checkpoint, duration);
    }

    public void SetCheckpoint<TPayload>(string name, TPayload payload, TimeSpan? duration = null)
    {
        var checkpoint = new Checkpoint<TPayload>(name, HandlerTypeName, Update.ChatId, payload);
        SetCheckpoint(checkpoint, duration);
    }

    public void SetCheckpoint(Checkpoint checkpoint, TimeSpan? duration = null)
    {
        if (Update.InteractorUserId is null)
            throw new InvalidOperationException($"No '{nameof(Update.InteractorUserId)}' specified by the context.");

        _checkpoints.SetCheckpoint(Update.InteractorUserId.Value, checkpoint, duration);
    }

    public Checkpoint<TPayload>? GetLocalCheckpoint<TPayload>() =>
        GetLocalCheckpoint() as Checkpoint<TPayload>;

    public Checkpoint? GetLocalCheckpoint()
    {
        if (Update.InteractorUserId is null)
            throw new InvalidOperationException($"No '{nameof(Update.InteractorUserId)}' specified by the context.");

        if (Update.ChatId is null)
            throw new InvalidOperationException($"No '{nameof(Update.ChatId)}' specified by the context.");

        return _checkpoints.GetLocalCheckpoint(Update.InteractorUserId.Value, Update.ChatId.Value, HandlerTypeName);
    }
    
    public Checkpoint? GetCheckpoint<TPayload>() =>
        GetCheckpoint() as Checkpoint<TPayload>;

    public Checkpoint? GetCheckpoint()
    {
        if (Update.InteractorUserId is null)
            throw new InvalidOperationException($"No '{nameof(Update.InteractorUserId)}' specified by the context.");

        return _checkpoints.GetCheckpoint(Update.InteractorUserId.Value);
    }

    public void ResetCheckpoint()
    {
        if (Update.InteractorUserId is null)
            throw new InvalidOperationException($"No '{nameof(Update.InteractorUserId)}' specified by the context.");

        _checkpoints.ResetCheckpoint(Update.InteractorUserId.Value);
    }

    #endregion

    #region Replies

    public void SetPreviousReplyMessageId(int messageId)
    {
        if (Update.ChatId is null)
            throw new InvalidOperationException($"No '{nameof(Update.ChatId)}' specified by the context.");

        _replies.SetPreviousReplyMessageId(HandlerTypeName, Update.ChatId.Value, messageId);
    }

    public int? GetPreviousReplyMessageId()
    {
        if (Update.ChatId is null)
            throw new InvalidOperationException($"No '{nameof(Update.ChatId)}' specified by the context.");

        return _replies.TryGetPreviousReplyMessageId(HandlerTypeName, Update.ChatId.Value);
    }

    public async Task DeletePreviousReplyAsync()
    {
        var messageId = GetPreviousReplyMessageId();
        if (messageId is not null)
            await DeleteMessageAsync(messageId.Value);
    }

    #endregion

    public async ValueTask<int> SendTextAsync(
        string text,
        ParseMode? parseMode = null,
        IReplyMarkup? replyMarkup = null,
        bool? disableWebPagePreview = null)
    {
        if (Update.ChatId is null)
            throw new InvalidOperationException($"No '{nameof(Update.ChatId)}' specified by the context.");

        var message = await _bot.SendTextMessageAsync(Update.ChatId.Value, text, parseMode, replyMarkup: replyMarkup,
            disableWebPagePreview: disableWebPagePreview ?? true);

        SetPreviousReplyMessageId(message.MessageId);

        return message.MessageId;
    }

    public async Task DeleteMessageAsync(int messageId)
    {
        if (Update.ChatId is null)
            throw new InvalidOperationException($"No '{nameof(Update.ChatId)}' specified by the context.");

        await _bot.DeleteMessageAsync(Update.ChatId, messageId);
    }
}