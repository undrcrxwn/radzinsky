using Microsoft.EntityFrameworkCore;
using Radzinsky.Domain.Models.Entities;

namespace Radzinsky.Application.Abstractions.Persistence;

public interface IApplicationDbContext
{
    public DbSet<User> Users { get; }
    public DbSet<State> States { get; }
    public DbSet<Chat> Chats { get; }
    public DbSet<ChatPortal> ChatPortals { get; }
    
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}