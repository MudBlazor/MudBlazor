// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Interfaces;
using MudBlazor.Interop;

namespace MudBlazor;

[Obsolete($"Please use {nameof(IPopoverService)} in conjunction with {nameof(IMudPopoverHolder)}. This will be removed in a future version.")]
public class MudPopoverHandler : IMudPopoverHolder
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly PopoverJsInterop _popoverJsInterop;
    internal readonly Action _updater;

    public Guid Id { get; }

    public RenderFragment Fragment { get; internal set; }

    public bool IsConnected { get; internal set; }

    public bool IsDetached { get; internal set; }

    public string Class { get; private set; }

    public string Style { get; private set; }

    public object Tag { get; private set; }

    public bool ShowContent { get; private set; }

    public DateTime? ActivationDate { get; private set; }

    public Dictionary<string, object> UserAttributes { get; set; } = new();

    public MudRender ElementReference { get; set; }

    public MudPopoverHandler(RenderFragment fragment, IJSRuntime jsInterop, Action updater)
    {
        ArgumentNullException.ThrowIfNull(jsInterop);
        Fragment = fragment ?? throw new ArgumentNullException(nameof(fragment));
        _updater = updater ?? throw new ArgumentNullException(nameof(updater));
        Id = Guid.NewGuid();
        _popoverJsInterop = new PopoverJsInterop(jsInterop);
    }

    public void SetComponentBaseParameters(MudComponentBase componentBase, string @class, string style, bool showContent)
    {
        Class = @class;
        Style = style;
        Tag = componentBase.Tag;
        UserAttributes = componentBase.UserAttributes;
        ShowContent = showContent;
        if (showContent)
        {
            ActivationDate = DateTime.Now;
        }
        else
        {
            ActivationDate = null;
        }
    }

    [Obsolete($"Use {nameof(UpdateFragmentAsync)} instead. This method will be removed in a future version.")]
    public void UpdateFragment(RenderFragment fragment,
        MudComponentBase componentBase, string @class, string style, bool showContent)
    {
        Fragment = fragment;
        SetComponentBaseParameters(componentBase, @class, @style, showContent);
        // this basically calls StateHasChanged on the Popover
        (ElementReference as IMudStateHasChanged)?.StateHasChanged();
        _updater?.Invoke(); // <-- this doesn't do anything anymore except making unit tests happy 
    }

    public async Task UpdateFragmentAsync(RenderFragment fragment,
        MudComponentBase componentBase, string @class, string style, bool showContent)
    {
        await _semaphore.WaitAsync();
        try
        {
            if (IsDetached)
            {
                return;
            }

            Fragment = fragment;
            SetComponentBaseParameters(componentBase, @class, @style, showContent);
            (ElementReference as IMudStateHasChanged)?.StateHasChanged();
            _updater?.Invoke(); // <-- this doesn't do anything anymore except making unit tests happy
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task Initialize()
    {
        await _semaphore.WaitAsync();
        try
        {
            if (IsDetached)
            {
                // If _detached is True, it means Detach() was invoked before Initialize() has had
                // a chance to run. In this case, we just want to return and not do anything else
                // otherwise we will end up with a memory leak.
                return;
            }

            IsConnected = await _popoverJsInterop.Connect(Id);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task Detach()
    {
        await _semaphore.WaitAsync();
        try
        {
            IsDetached = true;

            if (IsConnected)
            {
                await _popoverJsInterop.Disconnect(Id);
            }
        }
        finally
        {
            IsConnected = false;
            _semaphore.Release();
        }
    }
}
