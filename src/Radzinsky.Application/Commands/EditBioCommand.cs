using Mapster;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Abstractions.Persistence;
using Radzinsky.Application.Extensions;
using Radzinsky.Application.Models.Contexts;
using Radzinsky.Domain.Models.Entities;

namespace Radzinsky.Application.Commands;

public class EditBioCommand : ICommand
{
    private readonly IApplicationDbContext _dbContext;

    public EditBioCommand(IApplicationDbContext dbContext) =>
        _dbContext = dbContext;

    public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        var checkpoint = context.GetCheckpoint();
        
        if (checkpoint?.HandlerTypeName.EndsWith("Command") ?? false)
            context.ResetCheckpoint();
        else if (string.IsNullOrWhiteSpace(context.Payload))
        {
            await context.SendTextAsync(context.Resources!.GetRandom<string>("TellBio"));
            context.SetCheckpoint("TellBio");
            return;
        }

        var sender = await _dbContext.Users.FindOrAddAsync(
            context.Message.Sender.Id,
            () => context.Message.Sender.Adapt<User>());
        
        sender.Bio = context.Payload;

        await _dbContext.SaveChangesAsync(cancellationToken);
        await context.SendTextAsync(context.Resources!.GetRandom<string>("BioChanged"));
    }
}