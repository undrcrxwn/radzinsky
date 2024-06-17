using System.Text.RegularExpressions;

namespace Radzinsky.Framework.Routing.RegEx;

public record RegExRoute(Type EndpointType, GroupCollection Groups) : Route(EndpointType);