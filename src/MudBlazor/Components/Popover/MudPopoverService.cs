// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace MudBlazor
{
    public interface IMudPopoverService
    {
        MudPopoverHandler Register(RenderFragment fragment);
        Task<bool> Unregister(MudPopoverHandler hanlder);
        IEnumerable<MudPopoverHandler> Handlers { get; }
        Task InitializeIfNeeded();
        event EventHandler FragmentsChanged;
    }

    public class MudPopoverHandler
    {
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private readonly IJSRuntime _runtime;
        private readonly Action _updater;
        private bool _locked;
        private bool _detached;

        public Guid Id { get; }
        public RenderFragment Fragment { get; private set; }
        public bool IsConnected { get; private set; }
        public string Class { get; private set; }
        public string Style { get; private set; }
        public object Tag { get; private set; }
        public bool ShowContent { get; private set; }
        public Dictionary<string, object> UserAttributes { get; set; } = new Dictionary<string, object>();

        public MudPopoverHandler(RenderFragment fragment, IJSRuntime jsInterop, Action updater)
        {
            Fragment = fragment ?? throw new ArgumentNullException(nameof(fragment));
            _runtime = jsInterop ?? throw new ArgumentNullException(nameof(jsInterop));
            _updater = updater ?? throw new ArgumentNullException(nameof(updater));
            Id = Guid.NewGuid();
        }

        public void SetComponentBaseParameters(MudComponentBase componentBase, string @class, string style, bool showContent)
        {
            Class = @class;
            Style = style;
            Tag = componentBase.Tag;
            UserAttributes = componentBase.UserAttributes;
            ShowContent = showContent;
        }

        public void UpdateFragment(RenderFragment fragment,
            MudComponentBase componentBase, string @class, string style, bool showContent)
        {
            Fragment = fragment;
            SetComponentBaseParameters(componentBase, @class, @style, showContent);
            if (_locked == false)
            {
                _locked = true;
                _updater.Invoke();
            }
        }

        public async Task Initialize()
        {
            await _semaphore.WaitAsync();
            try
            {
                if (_detached)
                {
                    // If _detached is True, it means Detach() was invoked before Initialize() has had
                    // a chance to run. In this case, we just want to return and not do anything else
                    // otherwise we will end up with a memory leak.
                    return;
                }

                await _runtime.InvokeVoidAsync("mudPopover.connect", Id);
                IsConnected = true;
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
                _detached = true;

                if (IsConnected)
                {
                    await _runtime.InvokeVoidAsync("mudPopover.disconnect", Id);
                }
            }
            catch (JSDisconnectedException) { }
            catch (TaskCanceledException) { }
            finally
            {
                IsConnected = false;
                _semaphore.Release();
            }
        }

        public void Release() => _locked = false;
    }

    public class MudPopoverService : IMudPopoverService, IAsyncDisposable
    {
        private Dictionary<Guid, MudPopoverHandler> _handlers = new();
        private bool _isInitilized = false;
        private readonly IJSRuntime _jsRuntime;
        private readonly PopoverOptions _options;
        private SemaphoreSlim _semaphoreSlim = new(1, 1);

        public event EventHandler FragmentsChanged;

        public IEnumerable<MudPopoverHandler> Handlers => _handlers.Values.AsEnumerable();

        public MudPopoverService(IJSRuntime jsInterop, IOptions<PopoverOptions> options = null)
        {
            this._options = options?.Value ?? new PopoverOptions();
            _jsRuntime = jsInterop ?? throw new ArgumentNullException(nameof(jsInterop));
        }

        public async Task InitializeIfNeeded()
        {
            if (_isInitilized == true) { return; }

            try
            {
                await _semaphoreSlim.WaitAsync();
                if (_isInitilized == true) { return; }

                await _jsRuntime.InvokeVoidAsync("mudPopover.initilize", _options.ContainerClass, _options.FlipMargin);
                _isInitilized = true;
            }
            catch (JSDisconnectedException) { }
            catch (TaskCanceledException) { }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public MudPopoverHandler Register(RenderFragment fragment)
        {
            var handler = new MudPopoverHandler(fragment, _jsRuntime, () => FragmentsChanged?.Invoke(this, EventArgs.Empty));
            _handlers.Add(handler.Id, handler);

            FragmentsChanged?.Invoke(this, EventArgs.Empty);

            return handler;
        }

        public async Task<bool> Unregister(MudPopoverHandler handler)
        {
            if (handler == null) { return false; }
            if (_handlers.Remove(handler.Id) == false) { return false; }

            await handler.Detach();

            FragmentsChanged?.Invoke(this, EventArgs.Empty);

            return true;
        }

        //TO DO add js test
        [ExcludeFromCodeCoverage]
        public async ValueTask DisposeAsync()
        {
            if (_isInitilized == false) { return; }

            try
            {
                await _jsRuntime.InvokeVoidAsync("mudPopover.dispose");
            }
            catch (JSDisconnectedException) { }
            catch (TaskCanceledException) { }
        }
    }
}
