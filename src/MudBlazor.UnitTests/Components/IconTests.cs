﻿#pragma warning disable CS1998 // async without await
#pragma warning disable IDE1006 // leading underscore

using Bunit;
using FluentAssertions;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;
namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class IconTests
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
        /// MudIcon renders first an svg and then a span, both with style
        /// </summary>
        [Test]
        public void ShouldRenderIconWithStyle()
        {
            var colorStyle = "color: greenyellow;";
            var icon = Parameter(nameof(MudIcon.Icon), Icons.Filled.Add);
            var style = Parameter(nameof(MudIcon.Style), colorStyle);
            var comp = ctx.RenderComponent<MudIcon>(icon, style);
            comp.Markup.Trim().Should().StartWith("<svg")
                .And.Contain(Icons.Filled.Add)
                .And.Contain($"style=\"{colorStyle}\"");

            icon = Parameter(nameof(MudIcon.Icon), "customicon");
            comp.SetParametersAndRender(icon, style);
            comp.Markup.Trim().Should().StartWith("<span")
                .And.Contain("customicon")
                .And.Contain($"style=\"{colorStyle}\"");
        }
    }
}
