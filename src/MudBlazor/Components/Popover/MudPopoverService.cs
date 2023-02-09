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
using MudBlazor.Utilities;

namespace MudBlazor
{
    public interface IMudPopoverService
    {
        MudPopoverHandler Register(RenderFragment fragment);
        Task<bool> Unregister(MudPopoverHandler hanlder);
        ValueTask<int> CountProviders();
        bool ThrowOnDuplicateProvider { get; }
        IEnumerable<MudPopoverHandler> Handlers { get; }
        Task InitializeIfNeeded();
        event EventHandler FragmentsChanged;
    }

    public class MudPopoverHandler
    {
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private readonly IJSRuntime _runtime;
        private readonly Action _updater;
        private bool _detached;

        public Guid Id { get; }
        public RenderFragment Fragment { get; private set; }
        public bool IsConnected { get; private set; }
        public string Class { get; private set; }
        public string Style { get; private set; }
        public object Tag { get; private set; }
        public bool ShowContent { get; private set; }
        public DateTime? ActivationDate { get; private set; }
        public Dictionary<string, object> UserAttributes { get; set; } = new Dictionary<string, object>();
        public MudRender ElementReference { get; set; }

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
            if (showContent == true)
            {
                ActivationDate = DateTime.Now;
            }
            else
            {
                ActivationDate = null;
            }
        }

        public void UpdateFragment(RenderFragment fragment,
            MudComponentBase componentBase, string @class, string style, bool showContent)
        {
            Fragment = fragment;
            SetComponentBaseParameters(componentBase, @class, @style, showContent);
            // this basically calls StateHasChanged on the Popover
            ElementReference?.ForceRender();
            _updater?.Invoke(); // <-- this doesn't do anything anymore except making unit tests happy 
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

                IsConnected = await _runtime.InvokeVoidAsyncWithErrorHandling("mudPopover.connect", Id); ;
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
                    await _runtime.InvokeVoidAsyncWithErrorHandling("mudPopover.disconnect", Id);
                }
            }
            finally
            {
                IsConnected = false;
                _semaphore.Release();
            }
        }

    }

    public class MudPopoverService : IMudPopoverService, IAsyncDisposable
    {
        private Dictionary<Guid, MudPopoverHandler> _handlers = new();
        private bool _isInitialized = false;
        private readonly IJSRuntime _jsRuntime;
        private readonly PopoverOptions _options;
        private SemaphoreSlim _semaphoreSlim = new(1, 1);

        public event EventHandler FragmentsChanged;

        public bool ThrowOnDuplicateProvider => _options.ThrowOnDuplicateProvider;
        public IEnumerable<MudPopoverHandler> Handlers => _handlers.Values.AsEnumerable();

        public MudPopoverService(IJSRuntime jsInterop, IOptions<PopoverOptions> options = null)
        {
            this._options = options?.Value ?? new PopoverOptions();
            _jsRuntime = jsInterop ?? throw new ArgumentNullException(nameof(jsInterop));
        }

        public async Task InitializeIfNeeded()
        {
            if (_isInitialized == true) { return; }

            try
            {
                await _semaphoreSlim.WaitAsync();
                if (_isInitialized == true) { return; }

                await _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudPopover.initialize", _options.ContainerClass, _options.FlipMargin);
                _isInitialized = true;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public MudPopoverHandler Register(RenderFragment fragment)
        {
            var handler = new MudPopoverHandler(fragment, _jsRuntime, () => { /*not doing anything on purpose for now*/ });
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

        public async ValueTask<int> CountProviders()
        {
            if (!_isInitialized) { return -1; }

            var (success, value) = await _jsRuntime.InvokeAsyncWithErrorHandling<int>("mudpopoverHelper.countProviders");
            if (success)
                return value;
            return 0;
        }

        //TO DO add js test
        [ExcludeFromCodeCoverage]
        public async ValueTask DisposeAsync()
        {
            if (_isInitialized == false) { return; }

            await _jsRuntime.InvokeVoidAsyncWithErrorHandling("mudPopover.dispose");
        }
    }
}
