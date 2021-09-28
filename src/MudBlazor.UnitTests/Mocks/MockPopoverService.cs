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
        public IEnumerable<MudPopoverHandler> Handlers => _handlers;

        public event EventHandler FragmentsChanged;

        public Task InitializeIfNeeded() => Task.FromResult(true);

        public MudPopoverHandler Register(RenderFragment fragment) => new MudPopoverHandler(fragment, Mock.Of<IJSRuntime>(), () =>
        {
            FragmentsChanged?.Invoke(this, EventArgs.Empty);
        });

        public Task<bool> Unregister(MudPopoverHandler hanlder) => Task.FromResult(true);
    }
}
