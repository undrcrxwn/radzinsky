using Telegram.Bot.Types.Enums;

namespace Radzinsky.Bot.Common;

public enum MessagePrivacy
{
    Group, Private
}

public record MessageRequestBase(
    long ChatId,
    bool HasMention,
    ChatType ChatType);
