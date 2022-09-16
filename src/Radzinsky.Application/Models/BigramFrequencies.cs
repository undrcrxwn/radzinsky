﻿namespace Radzinsky.Application.Models;

public class BigramFrequencies
{
    public IDictionary<string, double> RussianBigramFrequencies { get; init; }
    public IDictionary<string, double> EnglishBigramFrequencies { get; init; }
}