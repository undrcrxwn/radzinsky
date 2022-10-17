using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Radzinsky.Domain.Enumerations;

namespace Radzinsky.Persistence.Converters;

public class ChatMemberPermissionsCollectionConverter : ValueConverter<ICollection<ChatMemberPermissions>, string>
{
    public ChatMemberPermissionsCollectionConverter()
        : base(
            x => string.Join(',', x),
            x => x.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(Enum.Parse<ChatMemberPermissions>)
                .ToList()) { }
}