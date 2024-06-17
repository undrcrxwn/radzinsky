using Microsoft.EntityFrameworkCore;
using Radzinsky.Persistence.Entities;

namespace Radzinsky.Persistence;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Chat> Chats => Set<Chat>();
    public DbSet<ChatMember> ChatMembers => Set<ChatMember>();
    public DbSet<Role> Roles => Set<Role>();
}