using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Radzinsky.Domain.Enumerations;
using Radzinsky.Domain.Models.Entities;
using Radzinsky.Persistence.Converters;

namespace Radzinsky.Persistence;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<State> States { get; set; } = null!;
    public DbSet<Chat> Chats { get; set; } = null!;
    public DbSet<ChatMember> ChatMembers { get; set; } = null!;
    public DbSet<ChatPortal> ChatPortals { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder
            .Entity<Role>()
            .Property(x => x.Permissions)
            .HasConversion<ChatMemberPermissionsCollectionConverter>();

        builder
            .Entity<ChatMember>()
            .HasKey(x => new { x.ChatId, x.UserId });
    }
}