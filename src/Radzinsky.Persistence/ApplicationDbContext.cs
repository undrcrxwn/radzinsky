using Microsoft.EntityFrameworkCore;
using Radzinsky.Domain.Models.Entities;
using Radzinsky.Application.Abstractions.Persistence;

namespace Radzinsky.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<State> States => Set<State>();
    public DbSet<Chat> Chats => Set<Chat>();
    public DbSet<ChatPortal> ChatPortals => Set<ChatPortal>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
}