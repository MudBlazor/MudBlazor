// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services;

[TestFixture]
public class DialogServiceTests
{
    [Test]
    public void ShowAsync_WithoutProvider_LogsGuidanceOnce()
    {
        // With no <MudDialogProvider/> subscribed, a dialog never renders and ShowAsync blocks until its
        // render timeout. Surface actionable guidance once instead of failing silently.
        // The guidance is logged synchronously in ShowCoreAsync before the render-complete wait, so the
        // returned tasks are intentionally not awaited (awaiting would block on that timeout).
        var loggerMock = new Mock<ILogger<DialogService>>();
        var service = new DialogService(loggerMock.Object);

        _ = service.ShowAsync<MudButton>();
        _ = service.ShowAsync<MudButton>();

        loggerMock.VerifyLogging(DialogService.MissingProviderMessage, LogLevel.Error, Times.Once());
    }
}
