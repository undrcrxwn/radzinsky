using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Extensions;
using Radzinsky.Application.Models;
using Radzinsky.Persistence;

namespace Radzinsky.Application.Commands;

public class BioCommand : ICommand
{
    private readonly ApplicationDbContext _dbContext;

    public BioCommand(ApplicationDbContext dbContext) =>
        _dbContext = dbContext;
    
    public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        if (context.Message.ReplyTarget is null)
        {
            await context.ReplyAsync(context.Resources.Variants["NoTarget"].PickRandom());
            return;
        }
        
        var bio = await _dbContext.UserBios.FindAsync(context.Message.ReplyTarget.Sender.Id);
        if (bio is null)
        {
            await context.ReplyAsync(context.Resources.Variants["NoBio"].PickRandom());
            return;
        }

        await context.ReplyAsync(bio.Description);
    }
}