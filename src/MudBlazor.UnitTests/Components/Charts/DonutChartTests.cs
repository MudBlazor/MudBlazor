// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.UnitTests.Components;
using System.Linq;
using FluentAssertions;
using MudBlazor.Charts;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Charts
{
    public class DonutChartTests : BunitTest
    {
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
            
            var comp = Context.RenderComponent<Donut>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Donut)
                .Add(p => p.MudChartParent, new MudChart())
                .Add(p => p.Height, "300px")
                .Add(p => p.Width, "300px")
                .Add(p => p.InputData, data)
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
            
        }
    }
}
