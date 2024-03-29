﻿using System.Globalization;
using System.Text;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Abstractions.Persistence;
using Radzinsky.Application.Extensions;
using Radzinsky.Application.Models.Contexts;

namespace Radzinsky.Application.Commands;

public class StatusCommand : ICommand
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRuntimeInfoService _runtimeInfo;

    public StatusCommand(IApplicationDbContext dbContext, IRuntimeInfoService runtimeInfo)
    {
        _dbContext = dbContext;
        _runtimeInfo = runtimeInfo;
    }

    public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        var response = new StringBuilder();

        response.Append("UTC: ");
        response.AppendLine(DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
        
        response.Append("Аптайм: ");
        response.AppendLine(_runtimeInfo.GetUptime().ToShortString());
        
        response.Append("Пользователей: ");
        response.AppendLine(_dbContext.Users.Count().ToString());

        response.Append("Чатов: ");
        response.AppendLine(_dbContext.Chats.Count().ToString());
        
        await context.SendTextAsync(response.ToString());
    }
}