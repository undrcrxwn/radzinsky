using Mapster;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models.AuthorizationResults;
using Radzinsky.Application.Models.Contexts;
using Radzinsky.Domain.Enumerations;
using Radzinsky.Domain.Models.Entities;
using Radzinsky.Persistence;
using Radzinsky.Persistence.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Radzinsky.Application.Commands;

public class BanCommand : ICommand
{
    private readonly ITelegramBotClient _bot;
    private readonly IAuthorizationService _authorization;
    private readonly ApplicationDbContext _dbContext;

    public BanCommand(
        ITelegramBotClient bot,
        IAuthorizationService authorization,
        ApplicationDbContext dbContext)
    {
        _bot = bot;
        _authorization = authorization;
        _dbContext = dbContext;
    }

    public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        if (context.Update.InteractorUserId == _bot.BotId)
        {
            await context.SendTextAsync(context.Resources!.GetRandom<string>("CannotBanMe"));
            return;
        }
        
        if (context.Message.IsPrivate)
        {
            await context.SendTextAsync(context.Resources!.GetRandom<string>("CannotBanInPrivateChat"));
            return;
        }
        
        if (context.Message.ReplyTarget is null)
        {
            await context.SendTextAsync(context.Resources!.GetRandom<string>("NoReplyTarget"));
            return;
        }

        var authorizationResult = await _authorization.AuthorizeAgainstAsync(
            context.Update.InteractorUserId!.Value,
            context.Message.ReplyTarget.Id,
            context.Update.ChatId!.Value,
            ChatMemberPermissions.BanLowerPriorities);

        switch (authorizationResult)
        {
            case NoPermission:
                await context.SendTextAsync(context.Resources!.GetRandom("NoPermission"));
                break;
            
            case FailedAgainstSuperadmin:
                await context.SendTextAsync(context.Resources!.GetRandom("FailedAgainstSuperadmin"));
                break;
            
            case PriorityDifference difference:
                await context.SendTextAsync(context.Resources!.GetRandom("PriorityDifference",
                    difference.ActualPriority, difference.TargetPriority));
                break;
            
            case Failure:
                await context.SendTextAsync(context.Resources!.GetRandom("Failure"));
                break;
            
            case Success:
                await _bot.BanChatMemberAsync(
                    context.Message.Chat.Id, context.Message.ReplyTarget.Sender.Id);

                var response = context.Resources!.GetRandom(
                    "Success", context.Message.ReplyTarget.Sender.FirstName);
        
                await context.SendTextAsync(response);
                break;
            
            case Undefined:
                await context.SendTextAsync(context.Resources!.GetRandom("Undefined"));
                break;
            
            default:
                throw new InvalidOperationException();
        }
    }
}