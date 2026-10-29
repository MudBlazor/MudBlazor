// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Bunit;
using MudBlazor.UnitTests.TestComponents.Card;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class CardTests : BunitTest
    {
        [Test]
        public async Task CardChildContent()
        {
            //Card header with child content should be render successfully
            var comp = Context.Render<CardChildContentTest>();
            var button = comp.FindComponent<MudButton>();
            var numeric = comp.FindComponent<MudNumericField<int>>();
            await comp.WaitForAssertionAsync(() => numeric.Instance.Value.Should().Be(0));
            await comp.InvokeAsync(() => button.Instance.OnClick.InvokeAsync());
            await comp.WaitForAssertionAsync(() => numeric.Instance.Value.Should().Be(1));
        }

        /// <summary>
        /// A caller-supplied title used to be erased by the trailing null literal.
        /// </summary>
        [Test]
        public void CardMedia_Should_KeepUserSuppliedTitle()
        {
            var comp = Context.Render<MudCardMedia>(parameters => parameters
                .Add(p => p.UserAttributes!, new Dictionary<string, object> { { "title", "Cover art" } }));

            comp.Find("div").GetAttribute("title").Should().Be("Cover art");
        }
    }
}
