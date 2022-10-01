using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models.Contexts;
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
            await context.ReplyAsync(context.Resources!.GetRandom<string>("NoTarget"));
            return;
        }
        
        var target = await _dbContext.Users.FindAsync(context.Message.ReplyTarget.Sender.Id);
        if (target?.Bio is null)
        {
            await context.ReplyAsync(context.Resources!.GetRandom<string>("NoBio"));
            return;
        }

        await context.ReplyAsync(target.Bio);
    }
}