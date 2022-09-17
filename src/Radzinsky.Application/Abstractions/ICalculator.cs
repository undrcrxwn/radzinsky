namespace Radzinsky.Application.Abstractions;

public interface ICalculator
{
    public bool CanCalculate(string expression);
    public double Calculate(string expression);
}