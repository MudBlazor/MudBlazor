using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Moq;

namespace MudBlazor.UnitTests.Mocks
{
    /// <summary>
    /// Mock for scroll spy
    /// </summary>
    public class MockPopoverService : IMudPopoverService
    {
        private List<MudPopoverHandler> _handlers = new();
        private static RenderFragment DefaultFragment = (builder) => { };
        public IEnumerable<MudPopoverHandler> Handlers => _handlers;

        public bool ThrowOnDuplicateProvider => false;

        public event EventHandler FragmentsChanged;

        public Task InitializeIfNeeded() => Task.FromResult(true);

        public MudPopoverHandler Register(RenderFragment fragment) => new MudPopoverHandler(fragment ?? DefaultFragment, Mock.Of<IJSRuntime>(), () =>
        {
            FragmentsChanged?.Invoke(this, EventArgs.Empty);
        });

        public Task<bool> Unregister(MudPopoverHandler handler) => Task.FromResult(true);

        public ValueTask<int> CountProviders() => ValueTask.FromResult(0);
    }
}
