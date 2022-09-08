using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Extensions;
using Radzinsky.Application.Models;

namespace Radzinsky.Application.Commands;

public class AliasesCommand : ICommand
{
    private readonly IEnumerable<CommandResources> _commands;
    private readonly ILinguisticParser _parser;

    public AliasesCommand(IEnumerable<CommandResources> commands, ILinguisticParser parser)
    {
        _commands = commands;
        _parser = parser;
    }

    public async Task ExecuteAsync(CommandContext context)
    {
        var alias = _parser.TryParseCommandAliasFromBeginning(context.Payload);
        if (alias is null)
        {
            await context.ReplyAsync(context.Resources.Variants["CommandNotFound"].PickRandom());
            return;
        }

        var aliases = _commands.First(x => x.Aliases.Contains(alias.Case)).Aliases;
        await context.ReplyAsync(aliases.Any()
            ? String.Join('\n', aliases)
            : context.Resources.Variants["SingleAlias"].PickRandom());
    }
}