// Copyright (c) mudblazor 2021
// License MIT

#pragma warning disable CS1998 // async without await
#pragma warning disable IDE1006 // leading underscore
#pragma warning disable BL0005 // Set parameter outside component

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
using static MudBlazor.UnitTests.TestComponents.AutocompleteSetParametersInitialization;

namespace MudBlazor.UnitTests.Components
{

    [TestFixture]
    public class PortalTests
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
        /// Initial value should be shown and popup should not open.
        /// </summary>
        [Test]
        public async Task AutocompleteTest1()
        {
            var comp = ctx.RenderComponent<PortalMenu>();
            Console.WriteLine(comp.Markup);
            // select elements needed for the test
            var menuComp = comp.FindComponent<MudMenu>();
            menuComp.Find("button")
            
            var portalContainer = comp.Find("#mud-portal-container");
            var portal = comp.Find(".portal");
            var autocomplete = menuComp.Instance;
            
           
        }


    }
}
