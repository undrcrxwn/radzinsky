using Microsoft.EntityFrameworkCore;
using Radzinsky.Domain.Models.Entities;

namespace Radzinsky.Persistence;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<ChatPortal> ChatPortals { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
}