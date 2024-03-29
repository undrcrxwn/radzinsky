﻿using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Abstractions.Persistence;
using Radzinsky.Application.Models.Contexts;

namespace Radzinsky.Application.Commands;

public class BioCommand : ICommand
{
    private readonly IApplicationDbContext _dbContext;

    public BioCommand(IApplicationDbContext dbContext) => _dbContext = dbContext;
    
    public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        if (context.Message.ReplyTarget is null)
        {
            await context.SendTextAsync(context.Resources!.GetRandom<string>("NoTarget"));
            return;
        }
        
        var target = await _dbContext.Users.FindAsync(context.Message.ReplyTarget.Sender.Id);
        if (target?.Bio is null)
        {
            await context.SendTextAsync(context.Resources!.GetRandom<string>("NoBio"));
            return;
        }

        await context.SendTextAsync(target.Bio);
    }
}