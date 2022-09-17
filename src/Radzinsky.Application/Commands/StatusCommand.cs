using System.Text;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Extensions;
using Radzinsky.Application.Models;
using Radzinsky.Application.Models.Contexts;
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
        var response = new StringBuilder();

        response.Append("UTC: ");
        response.AppendLine(DateTime.UtcNow.ToString());
        
        response.Append("Аптайм: ");
        response.AppendLine(_runtimeInfo.GetUptime().ToShortString());
        
        response.Append("Пользователей: ");
        response.AppendLine(_dbContext.Users.Count().ToString());

        response.Append("Чатов: ");
        response.AppendLine(_dbContext.Chats.Count().ToString());
        
        await context.ReplyAsync(response.ToString());
    }
}