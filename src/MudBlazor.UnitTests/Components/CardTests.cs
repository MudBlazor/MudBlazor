// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class CardTests : BunitTest
    {
        [Test]
        public async Task CardChildContent()
        {
            //Card header with child content should be render succesfully
            var comp = Context.RenderComponent<CardChildContentTest>();
            var button = comp.FindComponent<MudButton>();
            var numeric = comp.FindComponent<MudNumericField<int>>();
            comp.WaitForAssertion(() => numeric.Instance.Value.Should().Be(0));
            await comp.InvokeAsync(() => button.Instance.OnClick.InvokeAsync());
            comp.WaitForAssertion(() => numeric.Instance.Value.Should().Be(1));
        }
    }
}
