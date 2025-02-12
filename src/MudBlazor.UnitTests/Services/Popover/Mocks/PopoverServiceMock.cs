// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;

namespace MudBlazor.UnitTests.Services.Popover.Mocks;

#nullable enable
/// <summary>
/// Extended <see cref="PopoverService"/> will the ability to define <see cref="IPopoverTimerMock"/>
/// </summary>
internal class PopoverServiceMock : PopoverService
{
    private readonly IPopoverTimerMock _popoverTimerMock;

    public PopoverServiceMock(ILogger<PopoverService> logger, IJSRuntime jsInterop, IPopoverTimerMock? popoverTimerMock = null, IOptions<PopoverOptions>? options = null)
        : base(logger, jsInterop, options)
    {
        _popoverTimerMock = popoverTimerMock ?? new PopoverTimerEmpty();
    }

    public override async Task OnBatchTimerElapsedAsync(IReadOnlyCollection<MudPopoverHolder> items, CancellationToken cancellationToken)
    {
        await _popoverTimerMock.OnBatchTimerElapsedBeforeAsync(items, cancellationToken).ConfigureAwait(false);
        await base.OnBatchTimerElapsedAsync(items, cancellationToken).ConfigureAwait(false);
        await _popoverTimerMock.OnBatchTimerElapsedAfterAsync(items, cancellationToken).ConfigureAwait(false);
    }

    internal interface IPopoverTimerMock
    {
        Task OnBatchTimerElapsedBeforeAsync(IReadOnlyCollection<MudPopoverHolder> items, CancellationToken cancellationToken);

        Task OnBatchTimerElapsedAfterAsync(IReadOnlyCollection<MudPopoverHolder> items, CancellationToken cancellationToken);
    }

    internal class PopoverTimerEmpty : IPopoverTimerMock
    {
        public Task OnBatchTimerElapsedBeforeAsync(IReadOnlyCollection<MudPopoverHolder> items, CancellationToken cancellationToken) => Task.CompletedTask;

        public Task OnBatchTimerElapsedAfterAsync(IReadOnlyCollection<MudPopoverHolder> items, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
