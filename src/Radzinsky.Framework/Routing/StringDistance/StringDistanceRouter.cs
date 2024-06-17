using Telegram.Bot.Types;

namespace Radzinsky.Framework.Routing.StringDistance;

public class StringDistanceRouter(
    StringDistanceEndpointDiscovery discovery,
    DamerauLevenshteinStringDistanceCalculator distanceCalculator) : IRouter
{
    private const float MaxDistancePerCharacter = 0.5f;
    private const float MaxBotAddressDistancePerCharacter = 0.5f;

    private readonly IEnumerable<string> _botAddresses =
    [
        "радзински", "радзинский", "рафик", "шеф", "бот", "@radzinsky_bot"
    ];

    public Route? TryMatchEndpoint(Update update)
    {
        if (update.Message?.Text is null)
            return null;

        var textView = new StringView(update.Message.Text);
        var firstWordLooksLikeBotAddress = _botAddresses.Any(address =>
        {
            var distancePerCharacter = distanceCalculator.CalculateDistancePerCharacter(textView.NormalizedTextWords.First(), address);
            return distancePerCharacter <= MaxBotAddressDistancePerCharacter;
        });

        if (!firstWordLooksLikeBotAddress || textView.NormalizedTextWords.Length <= 1)
            return null;

        var closestAliasByEndpoint = discovery.Endpoints.Select(endpoint =>
        {
            var distancePerCharacterByAlias = endpoint.Aliases.Select(alias =>
            {
                var potentialAlias = textView.SelectTextWords(1, alias.WordCount);
                var normalizedPotentialAlias = textView.SelectNormalizedTextWords(1, alias.WordCount);

                return new
                {
                    Text = potentialAlias,
                    Alias = alias,
                    AliasWordCount = alias.WordCount,
                    DistancePerCharacter = distanceCalculator.CalculateDistancePerCharacter(normalizedPotentialAlias, alias.Value)
                };
            });

            var closestAlias = distancePerCharacterByAlias
                .OrderByDescending(match => match.Alias.Value.Length)
                .MinBy(match => match.DistancePerCharacter)!;

            return new
            {
                endpoint.EndpointType,
                ClosestAlias = closestAlias
            };
        });

        var bestMatch = closestAliasByEndpoint.MinBy(match => match.ClosestAlias.DistancePerCharacter);
        if (bestMatch is null || bestMatch.ClosestAlias.DistancePerCharacter > MaxDistancePerCharacter)
            return null;

        var tail = textView.SelectTextWordsFrom(1 + bestMatch.ClosestAlias.AliasWordCount);
        return new StringDistanceRoute(
            EndpointType: bestMatch.EndpointType,
            Alias: bestMatch.ClosestAlias.Alias.Value,
            BotAddressSegment: textView.TextWords[0],
            AliasSegment: bestMatch.ClosestAlias.Text,
            TailSegment: tail.Length > 0 ? tail : null);
    }
}