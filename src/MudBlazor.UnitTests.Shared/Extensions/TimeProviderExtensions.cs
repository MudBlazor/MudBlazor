// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;

namespace MudBlazor.UnitTests.Shared.Extensions;

/// <summary>
/// Extension methods for working with TimeProvider in unit tests.
/// </summary>
public static class TimeProviderExtensions
{
    /// <summary>
    /// Advances the fake time provider by the specified number of milliseconds.
    /// </summary>
    /// <param name="ctx">The bUnit context.</param>
    /// <param name="milliseconds">The number of milliseconds to advance time.</param>
    public static void AdvanceTime(this BunitContext ctx, double milliseconds)
    {
        var provider = (FakeTimeProvider)ctx.Services.GetRequiredService<TimeProvider>();
        provider.Advance(TimeSpan.FromMilliseconds(milliseconds));
    }

    /// <summary>
    /// Advances the fake time provider by the specified time span.
    /// </summary>
    /// <param name="ctx">The bUnit context.</param>
    /// <param name="timeSpan">The time span to advance time.</param>
    public static void AdvanceTime(this BunitContext ctx, TimeSpan timeSpan)
    {
        var provider = (FakeTimeProvider)ctx.Services.GetRequiredService<TimeProvider>();
        provider.Advance(timeSpan);
    }
}
