using System.Text;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models;
using Radzinsky.Persistence;

namespace Radzinsky.Application.Commands;

public class StatusCommand : ICommand
{
    private readonly ApplicationDbContext _dbContext;

    public StatusCommand(ApplicationDbContext dbContext) =>
        _dbContext = dbContext;
    
    public async Task ExecuteAsync(CommandContext context)
    {
        var responseBuilder = new StringBuilder();

        responseBuilder.Append("UTC: ");
        responseBuilder.AppendLine(DateTime.UtcNow.ToString());

        responseBuilder.Append("Пользователей: ");
        responseBuilder.AppendLine(_dbContext.Users.Count().ToString());

        responseBuilder.Append("Чатов: ");
        responseBuilder.AppendLine(_dbContext.Chats.Count().ToString());

        await context.ReplyAsync(responseBuilder.ToString());
    }
}