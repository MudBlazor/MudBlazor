// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using MudBlazor.Interop;

namespace MudBlazor
{
#nullable enable
    [Obsolete($"Please use {nameof(PopoverService)}. This will be removed in a future version.")]
    public class MudPopoverService : IMudPopoverService, IAsyncDisposable
    {
        private bool _isInitialized;
        private readonly IJSRuntime _jsRuntime;
        private readonly PopoverOptions _options;
        private readonly PopoverJsInterop _popoverJsInterop;
        private readonly Dictionary<Guid, MudPopoverHandler> _handlers = new();
        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

        public event EventHandler? FragmentsChanged;

        public bool ThrowOnDuplicateProvider => _options.ThrowOnDuplicateProvider;

        public IEnumerable<MudPopoverHandler> Handlers => _handlers.Values.AsEnumerable();

        public MudPopoverService(IJSRuntime jsInterop, IOptions<PopoverOptions>? options = null)
        {
            _options = options?.Value ?? new PopoverOptions();
            _jsRuntime = jsInterop ?? throw new ArgumentNullException(nameof(jsInterop));
            _popoverJsInterop = new PopoverJsInterop(jsInterop);
        }

        public async Task InitializeIfNeeded()
        {
            if (_isInitialized)
            {
                return;
            }

            try
            {
                await _semaphoreSlim.WaitAsync();
                if (_isInitialized)
                {
                    return;
                }

                await _popoverJsInterop.Initialize(_options.ContainerClass, _options.FlipMargin);
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
            _handlers.TryAdd(handler.Id, handler);

            FragmentsChanged?.Invoke(this, EventArgs.Empty);

            return handler;
        }

        public async Task<bool> Unregister(MudPopoverHandler? handler)
        {
            if (handler is null)
            {
                return false;
            }

            if (!_handlers.Remove(handler.Id))
            {
                return false;
            }

            await handler.Detach();

            FragmentsChanged?.Invoke(this, EventArgs.Empty);

            return true;
        }

        public async ValueTask<int> CountProviders()
        {
            if (!_isInitialized)
            {
                return -1;
            }

            var (success, value) = await _popoverJsInterop.CountProviders();

            return success ? value : 0;
        }

        //TO DO add js test
        [ExcludeFromCodeCoverage]
        public ValueTask DisposeAsync()
        {
            if (!_isInitialized)
            {
                return ValueTask.CompletedTask;
            }

            _ = _popoverJsInterop.Dispose();

            return ValueTask.CompletedTask;
        }
    }
}
