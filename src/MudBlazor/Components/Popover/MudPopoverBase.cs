// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Base class for implementing Popover component.
/// </summary>
/// <remarks>
/// This class provides a base implementation for a Popover component. It implements the <see cref="IPopover"/> interface
/// and utilizes the <see cref="IPopoverService"/> to handle the creation, updating, and destruction of the popover.
/// </remarks>
public abstract class MudPopoverBase : MudComponentBase, IPopover, IAsyncDisposable
{
    [Obsolete("For Legacy compatibility mode only, will be removed in v7.")]
    private MudPopoverHandler? _handler;

    private bool _afterFirstRender;

    /// <inheritdoc />
    public virtual Guid Id { get; [Obsolete("Set is only needed for legacy mode only. Remove in v7.")] private set; } = Guid.NewGuid();

    [Inject]
    [Obsolete($"Replaced by {nameof(PopoverService)}. Will be removed in v7.")]
    public IMudPopoverService Service { get; set; } = null!;

    [Inject]
    protected IPopoverService PopoverService { get; set; } = null!;

    /// <inheritdoc />
    string IPopover.PopoverClass => PopoverClass;

    /// <inheritdoc />
    string IPopover.PopoverStyles => PopoverStyles;

    protected internal abstract string PopoverClass { get; }

    protected internal abstract string PopoverStyles { get; }

    /// <inheritdoc />
    [Parameter]
    [Category(CategoryTypes.Popover.Behavior)]
    public bool Open { get; set; }

    /// <inheritdoc />
    [Parameter]
    [Category(CategoryTypes.Popover.Behavior)]
    public RenderFragment? ChildContent { get; set; }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        if (PopoverService.PopoverOptions.Mode == PopoverMode.Legacy)
#pragma warning disable CS0618 // Type or member is obsolete
        {
            _handler = Service.Register(ChildContent ?? new RenderFragment((x) => { }));
            _handler.SetComponentBaseParameters(this, PopoverClass, PopoverStyles, Open);
            Id = _handler.Id;
        }
#pragma warning restore CS0618 // Type or member is obsolete
        else
        {
            await PopoverService.CreatePopoverAsync(this);
        }

        await base.OnInitializedAsync();
    }

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if (_afterFirstRender)
        {
            if (PopoverService.PopoverOptions.Mode == PopoverMode.Legacy)
#pragma warning disable CS0618 // Type or member is obsolete
            {
                if (_handler is not null)
                {
                    await _handler.UpdateFragmentAsync(ChildContent, this, PopoverClass, PopoverStyles, Open);
                }
            }
#pragma warning restore CS0618 // Type or member is obsolete
            else
            {
                await PopoverService.UpdatePopoverAsync(this);
            }
        }
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (PopoverService.PopoverOptions.Mode == PopoverMode.Legacy)
#pragma warning disable CS0618 // Type or member is obsolete
            {
                if (_handler is not null)
                {
                    await _handler.Initialize();
                    await Service.InitializeIfNeeded();
                    await _handler.UpdateFragmentAsync(ChildContent, this, PopoverClass, PopoverStyles, Open);
                }
            }
#pragma warning restore CS0618 // Type or member is obsolete
            else
            {
                await PopoverService.UpdatePopoverAsync(this);
            }

            _afterFirstRender = true;
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public virtual async ValueTask DisposeAsync()
    {
        try
        {
            if (IsJSRuntimeAvailable)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                if (PopoverService.PopoverOptions.Mode == PopoverMode.Legacy)
                {
                    await Service.Unregister(_handler);
                }
#pragma warning restore CS0618 // Type or member is obsolete
                else
                {
                    await PopoverService.DestroyPopoverAsync(this);
                }
            }
        }
        catch (JSDisconnectedException) { }
        catch (TaskCanceledException) { }
    }
}
