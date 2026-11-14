// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services;

[TestFixture]
public class DialogServiceTests
{
    [Test]
    public void ParameterlessConstructor_IsEmittedInMetadata()
    {
        // Asserted through reflection rather than `new DialogService()`, which compiles either way:
        // an optional parameter on the logger constructor satisfies the call site at compile time but
        // emits no `.ctor()`, so only assemblies compiled against an earlier version notice it missing.
        var constructor = typeof(DialogService).GetConstructor(Type.EmptyTypes);

        constructor.Should().NotBeNull();
        constructor.Invoke(null).Should().BeOfType<DialogService>();
    }

    [Test]
    public void ServiceProvider_InjectsTheRegisteredLogger()
    {
        // The parameterless constructor must not shadow the logger one during activation,
        // or the missing-provider guidance would silently stop being logged.
        var loggerMock = new Mock<ILogger<DialogService>>();
        var services = new ServiceCollection();
        services.AddSingleton(loggerMock.Object);
        services.AddScoped<IDialogService, DialogService>();
        using var provider = services.BuildServiceProvider();
        var service = provider.GetRequiredService<IDialogService>();

        _ = service.ShowAsync<MudButton>();

        loggerMock.VerifyLogging(DialogService.MissingProviderMessage, LogLevel.Error, Times.Once());
    }

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
