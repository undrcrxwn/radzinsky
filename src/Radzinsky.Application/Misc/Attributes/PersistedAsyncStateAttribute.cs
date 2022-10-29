using System.Linq.Expressions;
using Radzinsky.Application.Models.Contexts;
using Radzinsky.Application.Models.Resources;

namespace Radzinsky.Application.Misc.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class PersistedAsyncStateAttribute : Attribute {}