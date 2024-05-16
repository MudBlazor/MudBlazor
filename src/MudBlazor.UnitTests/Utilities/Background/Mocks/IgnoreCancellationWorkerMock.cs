// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;
using MudBlazor.Utilities.Background;

namespace MudBlazor.UnitTests.Utilities.Background.Mocks;

internal class IgnoreCancellationWorkerMock : BackgroundWorkerBase
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return new TaskCompletionSource<object>().Task;
    }
}
