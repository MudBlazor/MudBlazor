// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.JSInterop;
using Moq;
using MudBlazor.Services;
using NUnit.Framework;

#pragma warning disable CS1998 // async without await

namespace MudBlazor.UnitTests.Services
{
    [TestFixture]
    public class JsEventTests
    {

        [Test]
        public async Task NoSubscriptionWithoutConnectTest()
        {
            var jsevent = new JsEvent(new Mock<IJSRuntime>().Object);
            Assert.Throws<InvalidOperationException>(() => jsevent.Paste += x => Console.WriteLine(x));
            Assert.Throws<InvalidOperationException>(() => jsevent.CaretPositionChanged += x => Console.WriteLine(x));
            Assert.Throws<InvalidOperationException>(() => jsevent.Select += (x, y) => Console.WriteLine());
            Assert.Throws<InvalidOperationException>(() => jsevent.Subscribe("copy"));
            // unsubscribing before connection is ignored
            await jsevent.Unsubscribe("copy");
            await jsevent.Disconnect();
            await jsevent.UnsubscribeAll();
            jsevent.Dispose();
        }

        [Test]
        public async Task EventSubscriptionTest()
        {
            var jsevent = new JsEvent(new Mock<IJSRuntime>().Object);
            await jsevent.Connect("asdf", new JsEventOptions { });
            // second connect is ignored
            await jsevent.Connect("asdf", new JsEventOptions { });

            // paste
            var pasteCount = 0;
            string pasteData = null;
            var pasteHandler = new Action<string>(x => { pasteCount++; pasteData = x; });
            jsevent.Paste += pasteHandler;
            jsevent.OnPaste("Muad`Dib");
            pasteData.Should().Be("Muad`Dib");
            pasteCount.Should().Be(1);
            jsevent._subscribedEvents.Should().Contain("paste");
            jsevent.Paste -= pasteHandler;
            // second remove is ignored
            jsevent.Paste -= pasteHandler;
            jsevent.OnPaste("Fremen");
            pasteData.Should().Be("Muad`Dib");
            pasteCount.Should().Be(1);
            jsevent._subscribedEvents.Should().BeEmpty();

            // caret position changed
            var caretPositionChangedCount = 0;
            int? caretPositionChangedData = null;
            var caretPositionChangedHandler = new Action<int>(x => { caretPositionChangedCount++; caretPositionChangedData = x; });
            jsevent.CaretPositionChanged += caretPositionChangedHandler;
            jsevent.OnCaretPositionChanged(17);
            caretPositionChangedData.Should().Be(17);
            caretPositionChangedCount.Should().Be(1);
            jsevent._subscribedEvents.Should().Contain("click");
            jsevent._subscribedEvents.Should().Contain("keyup");
            jsevent.CaretPositionChanged -= caretPositionChangedHandler;
            // second remove is ignored
            jsevent.CaretPositionChanged -= caretPositionChangedHandler;
            jsevent.OnCaretPositionChanged(99);
            caretPositionChangedData.Should().Be(17);
            caretPositionChangedCount.Should().Be(1);
            jsevent._subscribedEvents.Should().BeEmpty();

            // select
            var selectCount = 0;
            (int, int)? selectData = null;
            var selectHandler = new Action<int, int>((x, y) => { selectCount++; selectData = (x, y); });
            jsevent.Select += selectHandler;
            jsevent.OnSelect(77, 78);
            selectData.Should().Be((77, 78));
            selectCount.Should().Be(1);
            jsevent._subscribedEvents.Should().Contain("select");
            jsevent.Select -= selectHandler;
            // second remove is ignored
            jsevent.Select -= selectHandler;
            jsevent.OnSelect(99, 100);
            selectData.Should().Be((77, 78));
            selectCount.Should().Be(1);
            jsevent._subscribedEvents.Should().BeEmpty();

            // dispose unsubscribes all
            jsevent.Select += selectHandler;
            jsevent.CaretPositionChanged += caretPositionChangedHandler;
            jsevent.Paste += pasteHandler;
            jsevent.Dispose();
            // second dispose is ignored
            jsevent.Dispose();
            jsevent._subscribedEvents.Should().BeEmpty();
        }
    }
}
