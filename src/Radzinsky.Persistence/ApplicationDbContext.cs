using Microsoft.EntityFrameworkCore;
using Radzinsky.Domain.Models.Entities;
using Radzinsky.Domain.Models.Entities.States;

namespace Radzinsky.Persistence;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<State> States { get; set; } = null!;
    public DbSet<SurveyState> SurveyStates { get; set; } = null!;
    public DbSet<Chat> Chats { get; set; } = null!;
    public DbSet<ChatPortal> ChatPortals { get; set; } = null!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
}