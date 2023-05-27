// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MudBlazor.UnitTests.Mocks
{
    /// <summary>
    /// Mock for popover
    /// </summary>
    internal class MockPopoverServiceV2 : IPopoverService
    {
        public PopoverOptions PopoverOptions { get; } = new();

        public IEnumerable<IMudPopoverState> ActivePopovers { get; } = Enumerable.Empty<IMudPopoverState>();

        public bool IsInitialized => false;

        public void SubscribeOnPopoverUpdate(IPopoverObserver observer)
        {
        }

        public void UnsubscribeOnSubscribeOnPopoverUpdate(IPopoverObserver observer)
        {
        }

        public Task CreatePopoverAsync(IPopover mudPopover) => Task.CompletedTask;

        public Task<bool> UpdatePopoverAsync(IPopover mudPopover) => Task.FromResult(true);

        public Task<bool> DestroyPopoverAsync(IPopover mudPopover) => Task.FromResult(true);

        public ValueTask<int> CountProvidersAsync() => ValueTask.FromResult(0);

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
