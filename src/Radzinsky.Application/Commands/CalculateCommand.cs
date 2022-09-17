using System.Globalization;
using Radzinsky.Application.Abstractions;
using Radzinsky.Application.Extensions;
using Radzinsky.Application.Models.Checkpoints;
using Radzinsky.Application.Models.Contexts;

namespace Radzinsky.Application.Commands;

public class CalculateCommand : ICommand
{
    private readonly ICalculator _calculator;

    public CalculateCommand(ICalculator calculator) =>
        _calculator = calculator;

    public async Task ExecuteAsync(CommandContext context, CancellationToken cancellationToken)
    {
        if (context.Checkpoint is not CommandCheckpoint &&
            string.IsNullOrWhiteSpace(context.Payload))
        {
            context.SetCommandCheckpoint("WaitingForExpression");
            await context.ReplyAsync(context.Resources.Variants["GiveMeExpression"].PickRandom());
            return;
        }

        if (!_calculator.CanCalculate(context.Payload))
        {
            await context.ReplyAsync(context.Resources.Variants["InvalidSyntax"].PickRandom());
            return;
        }

        var result = _calculator.Calculate(context.Payload);
        await context.ReplyAsync(result.ToString(CultureInfo.InvariantCulture));
    }
}