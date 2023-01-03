// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.UnitTests.Components;
using System;
using System.Linq;
using FluentAssertions;
using MudBlazor.Charts;
using NUnit.Framework;
using Bunit;

namespace MudBlazor.UnitTests.Charts
{
    public class DonutChartTests : BunitTest
    {
        private readonly string[] _baseChartPalette = 
        {
            "#2979FF", "#1DE9B6", "#FFC400", "#FF9100", "#651FFF", "#00E676", "#00B0FF", "#26A69A", "#FFCA28",
            "#FFA726", "#EF5350", "#EF5350", "#7E57C2", "#66BB6A", "#29B6F6", "#FFA000", "#F57C00", "#D32F2F",
            "#512DA8", "#616161"
        };
        
        private readonly string[] _modifiedPalette =
        {
            "#264653", "#2a9d8f", "#e9c46a", "#f4a261", "#e76f51"
        };
        
         [SetUp]
        public void Init()
        {
            
        }

        [Test]
        public void DonutChartEmptyData()
        {
            var comp = Context.RenderComponent<Donut>();
            comp.Markup.Should().Contain("mud-chart-donut");
        }

        [Test]
        [TestCase(new double[]{50, 25, 20, 5 })]
        [TestCase(new double[]{50, 25, 20, 5 , 12})]
        public void DonutChartExampleData(double[] data)
        {
            string[] labels = { "Fossil", "Nuclear", "Solar", "Wind", "Oil", "Coal", "Gas", "Biomass",
                "Hydro", "Geothermal", "Fossil", "Nuclear", "Solar", "Wind", "Oil",
                "Coal", "Gas", "Biomass", "Hydro", "Geothermal" };
            
            var comp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Donut)
                .Add(p => p.Height, "300px")
                .Add(p => p.Width, "300px")
                .Add(p => p.InputData, data)
                .Add(p => p.ChartOptions, new ChartOptions {ChartPalette = _baseChartPalette})
                .Add(p => p.InputLabels,labels));
            
            comp.Markup.Should().Contain("class=\"mud-chart-donut\"");
            comp.Markup.Should().Contain("class=\"mud-chart-serie mud-donut-segment\"");
            comp.Markup.Should().Contain("mud-chart-legend-item");
            
            if (data.Length <= 4)
            {
                comp.Markup.Should().
                    Contain("Fossil").And.Contain("Nuclear").And.Contain("Solar").And.Contain("Wind");
            }
            
            if (data.Length >= 5)
            {
                comp.Markup.Should()
                    .Contain("Oil");
            }
            
            if (data.Length == 4 && data.Contains(50))
            {
                comp.Markup.Should()
                    .Contain("stroke-dasharray=\"50 50\" stroke-dashoffset=\"125\"");
            }

            if (data.Length == 4 && data.Contains(5))
            {
                comp.Markup.Should()
                    .Contain("stroke-dasharray=\"5 95\" stroke-dashoffset=\"30\"");
            }
            
            comp.SetParametersAndRender(parameters => parameters
                .Add(p => p.ChartOptions, new ChartOptions(){ChartPalette = _modifiedPalette}));

            comp.Markup.Should().Contain(_modifiedPalette[0]);
        }

        [Test]
        [TestCase(new double[]{50, 25, 20, 5 })]
        [TestCase(new double[]{50, 25, 20, 5 , 12})]
        public void DonutCirclePosition(double[] data)
        {
            string[] labels = { "Fossil", "Nuclear", "Solar", "Wind", "Oil", "Coal", "Gas", "Biomass",
                "Hydro", "Geothermal", "Fossil", "Nuclear", "Solar", "Wind", "Oil",
                "Coal", "Gas", "Biomass", "Hydro", "Geothermal" };
            
            var comp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Donut)
                .Add(p => p.Height, "300px")
                .Add(p => p.Width, "300px")
                .Add(p => p.InputData, data)
                .Add(p => p.ChartOptions, new ChartOptions {ChartPalette = _baseChartPalette})
                .Add(p => p.InputLabels,labels));
            
            var svgViewBox = comp.Find("svg").GetAttribute("viewBox")?.Split(" ")?.Select(s => Int32.Parse(s))?.ToArray();
            var circles = comp.FindAll("circle");

            svgViewBox.Should().NotBeNullOrEmpty("must have a valid viewbox", svgViewBox);

            foreach (var c in circles)
            {
                var cx = Int32.Parse(c.GetAttribute("cx") ?? "0");
                var cy = Int32.Parse(c.GetAttribute("cy") ?? "0");

                cx.Should().Be(svgViewBox[2]/2);

                cx.Should().Be(svgViewBox[3]/2);
            }
        }
    }
}
