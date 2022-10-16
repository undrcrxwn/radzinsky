﻿using Radzinsky.Domain.Models.Entities;

namespace Radzinsky.Application.Abstractions;

public interface IStateService
{
    public Task<T?> ReadStateAsync<T>(string key) where T : class;
    public Task WriteStateAsync(string key, object payload);
}