// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Bunit;
using FluentAssertions;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class AppBarTests
    {
        private Bunit.TestContext ctx;

        [SetUp]
        public void Setup()
        {
            ctx = new Bunit.TestContext();
            ctx.AddTestServices();
        }

        [TearDown]
        public void TearDown() => ctx.Dispose();

        /// <summary>
        /// AppBar with modified Toolbar class
        /// </summary>
        [Test]
        public void AppBarWithModifiedToolBarClass()
        {
            var comp = ctx.RenderComponent<MudAppBar>(new[]
            {
                Parameter(nameof(MudAppBar.ToolBarClass), "test-class")
            });

            // Find the Toolbar inside the AppBar
            comp.Find("div").ToMarkup()
                .Should()
                .Contain("test-class");
        }

    }
}
