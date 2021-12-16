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
            Assert.Throws<InvalidOperationException>(() => jsevent.Copy += () => Console.WriteLine());
            Assert.Throws<InvalidOperationException>(() => jsevent.Paste += x => Console.WriteLine(x));
            Assert.Throws<InvalidOperationException>(() => jsevent.CaretPositionChanged += x => Console.WriteLine(x));
            Assert.Throws<InvalidOperationException>(() => jsevent.Select += (x,y) => Console.WriteLine());
        }

        [Test]
        public async Task EventSubscriptionTest()
        {
            var jsevent = new JsEvent(new Mock<IJSRuntime>().Object);
            await jsevent.Connect("asdf", new JsEventOptions { });
            
            // copy
            var copyCount = 0;
            var copyHandler = new Action(() => copyCount++);
            jsevent.Copy += copyHandler;
            jsevent.OnCopy();
            copyCount.Should().Be(1);
            jsevent._subscribedEvents.Should().Contain("copy");
            jsevent.Copy -= copyHandler;
            jsevent.OnCopy();
            copyCount.Should().Be(1);
            jsevent._subscribedEvents.Should().BeEmpty();

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
            jsevent.OnCaretPositionChanged(99);
            caretPositionChangedData.Should().Be(17);
            caretPositionChangedCount.Should().Be(1);
            jsevent._subscribedEvents.Should().BeEmpty();

            // select
            var selectCount = 0;
            (int, int)? selectData = null;
            var selectHandler = new Action<int, int>((x,y) => { selectCount++; selectData = (x,y); });
            jsevent.Select += selectHandler;
            jsevent.OnSelect(77, 78);
            selectData.Should().Be((77, 78));
            selectCount.Should().Be(1);
            jsevent._subscribedEvents.Should().Contain("select");
            jsevent.Select -= selectHandler;
            jsevent.OnSelect(99, 100);
            selectData.Should().Be((77, 78));
            selectCount.Should().Be(1);
            jsevent._subscribedEvents.Should().BeEmpty();

            // dispose unsubscribes all
            jsevent.Select += selectHandler;
            jsevent.CaretPositionChanged += caretPositionChangedHandler;
            jsevent.Paste += pasteHandler;
            jsevent.Copy += copyHandler;
            jsevent.Dispose();
            jsevent._subscribedEvents.Should().BeEmpty();
        }
    }
}
