using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models.Contexts;
using Radzinsky.Domain.Models;
using Radzinsky.Persistence;

namespace Radzinsky.Application.Commands;

public class EditBioCommand : ICommand
{
    private readonly ApplicationDbContext _dbContext;

    public EditBioCommand(ApplicationDbContext dbContext) =>
        _dbContext = dbContext;

    public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        if (context.Checkpoint is not null)
            context.ResetCheckpoint();
        else if (string.IsNullOrWhiteSpace(context.Payload))
        {
            await context.ReplyAsync(context.Resources.GetRandom<string>("TellBio"));
            context.SetCommandCheckpoint("TellBio");
            return;
        }

        var bio = await _dbContext.UserBios.FindAsync(context.Message.Sender.Id);
        if (bio is null)
        {
            bio = new UserBio()
            {
                UserId = context.Message.Sender.Id,
                Description = context.Payload
            };
            await _dbContext.UserBios.AddAsync(bio);
        }
        else
        {
            bio.Description = context.Payload;
            _dbContext.UserBios.Update(bio);
        }

        await _dbContext.SaveChangesAsync();
        await context.ReplyAsync(context.Resources.GetRandom<string>("BioChanged"));
    }
}