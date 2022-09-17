using org.mariuszgromada.math.mxparser;
using Radzinsky.Application.Abstractions;

namespace Radzinsky.Application.Services;

public class Calculator : ICalculator
{
    public bool CanCalculate(string expression) =>
        new Expression(expression.ToLower()).checkSyntax();

    public double Calculate(string expression) =>
        new Expression(expression.ToLower()).calculate();
}