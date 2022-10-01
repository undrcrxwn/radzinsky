using Mapster;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models.Checkpoints;
using Radzinsky.Application.Models.Contexts;
using Radzinsky.Domain.Models.Entities;
using Radzinsky.Persistence;
using Radzinsky.Persistence.Extensions;

namespace Radzinsky.Application.Commands;

public class EditBioCommand : ICommand
{
    private readonly ApplicationDbContext _dbContext;

    public EditBioCommand(ApplicationDbContext dbContext) =>
        _dbContext = dbContext;

    public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        if (context.Checkpoint is CommandCheckpoint)
            context.ResetCheckpoint();
        else if (string.IsNullOrWhiteSpace(context.Payload))
        {
            await context.ReplyAsync(context.Resources!.GetRandom<string>("TellBio"));
            context.SetCommandCheckpoint("TellBio");
            return;
        }

        var sender = await _dbContext.Users.FindOrAddAsync(
            context.Message.Sender.Id,
            () => context.Message.Sender.Adapt<User>());
        
        sender.Bio = context.Payload;

        await _dbContext.SaveChangesAsync();
        await context.ReplyAsync(context.Resources!.GetRandom<string>("BioChanged"));
    }
}