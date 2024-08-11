// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor;

#nullable enable
[Obsolete($"Please use {nameof(IPopoverService)}. This will be removed in a future version.")]
public interface IMudPopoverService
{
    MudPopoverHandler Register(RenderFragment fragment);

    Task<bool> Unregister(MudPopoverHandler? handler);

    ValueTask<int> CountProviders();

    bool ThrowOnDuplicateProvider { get; }

    IEnumerable<MudPopoverHandler> Handlers { get; }

    Task InitializeIfNeeded();

    event EventHandler? FragmentsChanged;
}
