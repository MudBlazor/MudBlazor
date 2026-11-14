// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MudBlazor;


/// <summary>
/// Base class for popover components such as <see cref="MudPopover"/>, handling creation and lifecycle through the <see cref="IPopoverService"/>.
/// </summary>
/// <remarks>
/// This class provides a base implementation for a Popover component. It implements the <see cref="IPopover"/> interface
/// and utilizes the <see cref="IPopoverService"/> to handle the creation, updating, and destruction of the popover.
/// </remarks>
public abstract class MudPopoverBase : MudComponentBase, IPopover, IAsyncDisposable
{
    private bool _afterFirstRender;

    /// <inheritdoc />
    public virtual Guid Id { get; } = Guid.NewGuid();

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
        await PopoverService.CreatePopoverAsync(this);

        await base.OnInitializedAsync();
    }

    /// <inheritdoc />
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if (_afterFirstRender)
        {
            await PopoverService.UpdatePopoverAsync(this);
        }
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await PopoverService.UpdatePopoverAsync(this);

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
            if (IsJSRuntimeAvailable || CanDestroyBeforeFirstRender())
            {
                await PopoverService.DestroyPopoverAsync(this);
            }
        }
        catch (JSDisconnectedException)
        {
            // The circuit is gone, so the popover's browser-side state went with it.
        }
        catch (TaskCanceledException)
        {
            // The destroy call was cancelled while tearing down, and dispose must not throw.
        }
    }

    // The popover was created in OnInitializedAsync, so it must also be destroyed when it never rendered.
    // In interactive server rendering that happens when the next render batch removes it before the browser acknowledged the first one.
    // Destroying may initialize the service's JS side, which only prerendering cannot do.
    private bool CanDestroyBeforeFirstRender()
    {
        if (PopoverService.IsInitialized)
        {
            return true;
        }

#if NET9_0_OR_GREATER
        return RendererInfo.IsInteractive;
#else
        return false;
#endif
    }
}
