﻿using System.Runtime.CompilerServices;
using Radzinsky.Application.Models.Contexts;
using Radzinsky.Application.Models.Resources;
using Radzinsky.Application.Services;

namespace Radzinsky.Application.Abstractions;

public interface IPersistentAsyncService
{
    public YieldAwaitable SaveCurrentState(string? identifier = null);
    public Task SaveState(IAsyncStateMachine machine, string? identifier = null);
    
    public YieldAwaitable RetrieveCurrentState(string? identifier = null);
    public Task RetrieveState(IAsyncStateMachine machine, string? identifier = null);
    
    public Task AwaitCallback();
}