// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using MudBlazor.Utilities.Background;

namespace MudBlazor.UnitTests.Utilities.Background.Mocks;

internal class ThrowOnCancellationWorkerMock : BackgroundWorkerBase
{
    public int TokenCalls { get; set; }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.Register(() =>
        {
            TokenCalls++;
            throw new InvalidOperationException();
        });

        stoppingToken.Register(() =>
        {
            TokenCalls++;
        });

        return new TaskCompletionSource<object>().Task;
    }
}
