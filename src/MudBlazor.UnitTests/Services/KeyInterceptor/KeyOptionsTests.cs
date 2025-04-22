// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using MudBlazor.Services;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services.KeyInterceptor;

#nullable enable
[TestFixture]
public class KeyOptionsTests
{
    [Test]
    public void Defaults()
    {
        // Arrange
        var keyOptions1 = new KeyOptions();
        var keyOptions2 = new KeyOptions(null);

        // Act & Assert
        keyOptions1.Key.Should().Be(keyOptions2.Key);
        keyOptions1.SubscribeDown.Should().Be(keyOptions2.SubscribeDown);
        keyOptions1.SubscribeUp.Should().Be(keyOptions2.SubscribeUp);
        keyOptions1.PreventDown.Should().Be(keyOptions2.PreventDown);
        keyOptions1.PreventUp.Should().Be(keyOptions2.PreventUp);
        keyOptions1.StopDown.Should().Be(keyOptions2.StopDown);
        keyOptions1.StopUp.Should().Be(keyOptions2.StopUp);
    }
}
