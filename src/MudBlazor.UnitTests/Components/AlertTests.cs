// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class AlertTests : BunitTest
    {
        [Test]
        public async Task AlertTest()
        {
            var comp = Context.RenderComponent<MudAlert>(Parameter("Severity", Severity.Info));
            await comp.InvokeAsync(() => comp.Instance.OnCloseIconClickAsync());
            comp.WaitForAssertion(() => comp.Instance.Icon.Should().BeNull());
        }

    }
}
