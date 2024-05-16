// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using MudBlazor.Services;

namespace MudBlazor;

#nullable enable
[DebuggerDisplay("{DebuggerToString(),nq}")]
internal class BrowserViewportSubscription : IEquatable<BrowserViewportSubscription>
{
    public Guid JavaScriptListenerId { get; }

    public Guid ObserverId { get; }

    public ResizeOptions? Options { get; }

    public BrowserViewportSubscription(Guid javaScriptListenerId, Guid observerId)
        : this(javaScriptListenerId, observerId, null)
    {
    }

    public BrowserViewportSubscription(Guid javaScriptListenerId, Guid observerId, ResizeOptions? resizeOptions)
    {
        JavaScriptListenerId = javaScriptListenerId;
        ObserverId = observerId;
        Options = resizeOptions;
    }

    public bool Equals(BrowserViewportSubscription? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return JavaScriptListenerId.Equals(other.JavaScriptListenerId) && ObserverId.Equals(other.ObserverId);
    }

    public override bool Equals(object? obj) => obj is BrowserViewportSubscription browserViewportSubscription && Equals(browserViewportSubscription);

    public override int GetHashCode() => HashCode.Combine(JavaScriptListenerId, ObserverId);

    [ExcludeFromCodeCoverage]
    private string DebuggerToString()
    {
        return $"JavaScript Listener Id = {JavaScriptListenerId}, Observer Id = {ObserverId}";
    }
}
