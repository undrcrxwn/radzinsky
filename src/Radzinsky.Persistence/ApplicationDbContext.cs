using System.ComponentModel.Design;
using Microsoft.EntityFrameworkCore;
using Radzinsky.Domain.Enumerations;
using Radzinsky.Domain.Models.Entities;

namespace Radzinsky.Persistence;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Chat> Chats { get; set; } = null!;
    public DbSet<ChatMember> ChatMembers { get; set; } = null!;
    public DbSet<ChatPortal> ChatPortals { get; set; } = null!;
    public DbSet<ChatMemberRole> MemberRoles { get; set; } = null!;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChatMemberRole>().HasData(new ChatMemberRole[]
        {
            new()
            {
                Title = "Administrator",
                Permissions = Enum.GetValues<MemberPermissions>(),
                Priority = int.MinValue
            }
        });
    }
}