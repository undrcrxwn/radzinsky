using Microsoft.Extensions.Configuration;
using Radzinsky.Application.Abstractions;
using Telegram.Bot.Types.ReplyMarkups;

namespace Radzinsky.Application.Services;

public class ButtonFactory : IButtonFactory
{
    private readonly string _callbackHandlerTypeNameHash;
    private readonly string _dataFormat;

    public ButtonFactory(IHashingService hasher, string callbackHandlerTypeName, string? dataFormat = null)
    {
        _callbackHandlerTypeNameHash = hasher.HashKey(callbackHandlerTypeName);
        _dataFormat = dataFormat ?? "{0}";
    }

    public InlineKeyboardButton CreateCallbackDataButton(string label, string data) =>
        InlineKeyboardButton.WithCallbackData(label, DecorateWithCallbackDataHeaders(data));

    private string DecorateWithCallbackDataHeaders(string data) =>
        $"{_callbackHandlerTypeNameHash}{string.Format(_dataFormat, data)}";
}