using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Radzinsky.Domain.Models.Entities;

namespace Radzinsky.Persistence;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<State> States { get; set; } = null!;
    public DbSet<Chat> Chats { get; set; } = null!;
    public DbSet<ChatPortal> ChatPortals { get; set; } = null!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        var settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        builder
            .Entity<State>()
            .Property(x => x.Payload)
            .HasConversion(
                x => JsonConvert.SerializeObject(x, settings),
                x => JsonConvert.DeserializeObject(x, settings));
    }
}