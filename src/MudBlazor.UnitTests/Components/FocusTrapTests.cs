// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class FocusTrapTests : BunitTest
    {
        [Test]
        public void FocusTrap_Test()
        {
            var comp = Context.RenderComponent<MudFocusTrap>();
#pragma warning disable BL0005
            comp.Instance.Disabled = true;
            comp.Instance.Disabled = false;

            comp.Instance.InitializeFocusAsync();

            comp.Instance.GetShouldRender().Should().BeTrue();
        }

    }
}
