using Microsoft.Extensions.Primitives;

namespace Radzinsky.Framework.Routing.StringDistance;

public record StringDistanceRoute(
    Type EndpointType,
    string Alias,
    StringSegment BotAddressSegment,
    StringSegment AliasSegment,
    StringSegment? TailSegment) : Route(EndpointType);