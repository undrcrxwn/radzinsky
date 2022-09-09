using System.Text;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Extensions;
using Radzinsky.Application.Models;
using Radzinsky.Persistence;

namespace Radzinsky.Application.Commands;

public class StatusCommand : ICommand
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IRuntimeInfoService _runtimeInfo;

    public StatusCommand(ApplicationDbContext dbContext, IRuntimeInfoService runtimeInfo)
    {
        _dbContext = dbContext;
        _runtimeInfo = runtimeInfo;
    }

    public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        var responseBuilder = new StringBuilder();

        responseBuilder.Append("UTC: ");
        responseBuilder.AppendLine(DateTime.UtcNow.ToString());
        
        responseBuilder.Append("Аптайм: ");
        responseBuilder.AppendLine(_runtimeInfo.GetUptime().ToShortString());

        responseBuilder.Append("Пользователей: ");
        responseBuilder.AppendLine(_dbContext.Users.Count().ToString());

        responseBuilder.Append("Чатов: ");
        responseBuilder.AppendLine(_dbContext.Chats.Count().ToString());
        
        await context.ReplyAsync(responseBuilder.ToString());
    }
}