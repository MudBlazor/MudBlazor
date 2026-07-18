// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services.Overlay;

#nullable enable
[TestFixture]
public class OverlayServiceTests
{
    [Test]
    public void HasVisibleOverlay_ShouldBeFalse_AtInitialization()
    {
        var service = new OverlayService();

        service.HasVisibleOverlay.Should().BeFalse();
    }

    [Test]
    public void RegisterOverlay_ShouldThrow_WhenCallbackIsNull()
    {
        var service = new OverlayService();

        var act = () => service.RegisterOverlay(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void RegisterOverlay_ShouldMarkOverlayVisible_UntilTokenDisposed()
    {
        var service = new OverlayService();

        var token = service.RegisterOverlay(() => Task.CompletedTask);
        service.HasVisibleOverlay.Should().BeTrue();

        token.Dispose();
        service.HasVisibleOverlay.Should().BeFalse();
    }

    [Test]
    public async Task CloseLastOverlayAsync_ShouldReturnFalse_WhenNoOverlayVisible()
    {
        var service = new OverlayService();

        var result = await service.CloseLastOverlayAsync();

        result.Should().BeFalse();
    }

    [Test]
    public async Task CloseLastOverlayAsync_ShouldCloseMostRecentlyRegistered_First()
    {
        var service = new OverlayService();
        var closed = new List<int>();
        service.RegisterOverlay(() => { closed.Add(1); return Task.CompletedTask; });
        service.RegisterOverlay(() => { closed.Add(2); return Task.CompletedTask; });

        (await service.CloseLastOverlayAsync()).Should().BeTrue();
        closed.Should().Equal(2);

        (await service.CloseLastOverlayAsync()).Should().BeTrue();
        closed.Should().Equal(2, 1);

        service.HasVisibleOverlay.Should().BeFalse();
        (await service.CloseLastOverlayAsync()).Should().BeFalse();
    }

    [Test]
    public async Task CloseLastOverlayAsync_ShouldRemoveOverlay_EvenWhenCallbackDoesNotDisposeToken()
    {
        var service = new OverlayService();
        service.RegisterOverlay(() => Task.CompletedTask);

        await service.CloseLastOverlayAsync();

        service.HasVisibleOverlay.Should().BeFalse();
    }
}
