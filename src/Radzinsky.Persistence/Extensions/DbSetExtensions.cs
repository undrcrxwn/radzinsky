using Microsoft.EntityFrameworkCore;

namespace Radzinsky.Persistence.Extensions;

public static class DbSetExtensions
{
    public static async Task<T> FindOrAddAsync<T>(
        this DbSet<T> items, object key, Func<T> factory) where T : class
    {
        var item = await items.FindAsync(key);
        
        if (item is not null)
            return item;
        
        item = factory();
        await items.AddAsync(item);
        return item;
    }
}