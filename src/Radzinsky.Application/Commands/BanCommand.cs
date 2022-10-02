using Microsoft.EntityFrameworkCore;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Models.Contexts;
using Radzinsky.Domain.Enumerations;
using Radzinsky.Persistence;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace Radzinsky.Application.Commands;

public class BanCommand : ICommand
{
    private readonly ApplicationDbContext _dbContext;

    public BanCommand(ApplicationDbContext dbContext) =>
        _dbContext = dbContext;

    public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        if (context.Message.ReplyTarget?.Sender.Id == context.Bot.BotId)
        {
            await context.ReplyAsync(context.Resources!.GetRandom<string>("CannotBanMe"));
            return;
        }

        if (context.Message.IsPrivate)
        {
            await context.ReplyAsync(context.Resources!.GetRandom<string>("CannotBanInPrivateChat"));
            return;
        }

        if (context.Message.ReplyTarget is null)
        {
            await context.ReplyAsync(context.Resources!.GetRandom<string>("NoReplyTarget"));
            return;
        }

        var sender = await _dbContext.ChatMembers.FirstAsync(x =>
            x.Chat.Id == context.Message.Chat.Id &&
            x.User.Id == context.Message.Sender.Id);

        var target = await _dbContext.ChatMembers.FirstAsync(x =>
            x.Chat.Id == context.Message.Chat.Id &&
            x.User.Id == context.Message.ReplyTarget.Sender.Id);

        if (!sender.IsChatAdministrator && !sender.Role.Permissions.Contains(MemberPermissions.Ban))
        {
            await context.ReplyAsync(context.Resources!.GetRandom<string>("NoPermission"));
            return;
        }

        if (target.IsChatAdministrator || target.Role.Priority >= sender.Role.Priority)
        {
            await context.ReplyAsync(context.Resources!.GetRandom<string>("CannotBanHigherRole"));
            return;
        }

        target.Role = null;

        await context.Bot.BanChatMemberAsync(
            context.Message.Chat.Id, context.Message.ReplyTarget.Sender.Id);

        var response = context.Resources!.GetRandom(
            "UserBanned", context.Message.ReplyTarget.Sender.FirstName);

        await _dbContext.SaveChangesAsync();
        await context.ReplyAsync(response);
    }
}