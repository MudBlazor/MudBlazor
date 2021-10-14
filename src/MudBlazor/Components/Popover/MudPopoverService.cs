// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
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
        private IJSRuntime _runtime;
        private readonly Action _updater;
        private bool _locked;

        public Guid Id { get; init; }
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
            await _runtime.InvokeVoidAsync("mudPopover.connect", Id);
            IsConnected = true;
        }

        public async Task Detach()
        {
            try
            {
                await _runtime.InvokeVoidAsync("mudPopover.disconnect", Id);
            }
            catch (TaskCanceledException)
            {
            }
            finally
            {
                IsConnected = false;
            }
        }

        public void Release() => _locked = false;
    }

    public class MudPopoverService : IMudPopoverService, IAsyncDisposable
    {
        private List<MudPopoverHandler> _handlers = new();
        private bool _isInitilized = false;
        private readonly IJSRuntime _jsRuntime;
        private readonly PopoverOptions _options;
        private SemaphoreSlim _semaphoreSlim = new(1, 1);

        public event EventHandler FragmentsChanged;

        public IEnumerable<MudPopoverHandler> Handlers => _handlers.AsEnumerable();

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
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public MudPopoverHandler Register(RenderFragment fragment)
        {
            var handler = new MudPopoverHandler(fragment, _jsRuntime, () => FragmentsChanged?.Invoke(this, EventArgs.Empty));
            _handlers.Add(handler);

            FragmentsChanged?.Invoke(this, EventArgs.Empty);

            return handler;
        }

        public async Task<bool> Unregister(MudPopoverHandler handler)
        {
            if (handler == null) { return false; }
            if (_handlers.Contains(handler) == false) { return false; }

            if (handler.IsConnected == false) { return false; }

            await handler.Detach();
            _handlers.Remove(handler);

            FragmentsChanged?.Invoke(this, EventArgs.Empty);

            return true;
        }

        public async ValueTask DisposeAsync()
        {
            if (_isInitilized == false) { return; }

            try
            {
                await _jsRuntime.InvokeVoidAsync("mudPopover.dispose");
            }
            catch (TaskCanceledException)
            {
            }
        }
    }
}
