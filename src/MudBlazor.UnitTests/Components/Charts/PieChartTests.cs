// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Bunit;
using FluentAssertions;
using MudBlazor.Charts;
using MudBlazor.UnitTests.Components;
using NUnit.Framework;
using Moq;

namespace MudBlazor.UnitTests.Charts
{
    public class PieChartTests : BunitTest
    {
        private readonly Mock<MudChartBase> _mockMudChart = new ();

        private readonly string[] ChartPalette = 
        {
            "#2979FF", "#1DE9B6", "#FFC400", "#FF9100", "#651FFF", "#00E676", "#00B0FF", "#26A69A", "#FFCA28",
            "#FFA726", "#EF5350", "#EF5350", "#7E57C2", "#66BB6A", "#29B6F6", "#FFA000", "#F57C00", "#D32F2F",
            "#512DA8", "#616161"
        };
        
        [SetUp]
        public void Init()
        {
            _mockMudChart.Setup(s => s.ChartOptions.ChartPalette).Returns(ChartPalette);
        }

        [Test]
        public void PieChartEmptyData()
        {
            var comp = Context.RenderComponent<Pie>();
            comp.Markup.Should().Contain("mud-chart-pie");
            
            //comp.MarkupMatches(@"");
        }

        [Test]
        public void PieChartExampleData()
        {
            double[] data = { 77, 25, 20, 5 };
            string[] labels = { "Uranium", "Plutonium", "Thorium", "Caesium", "Technetium", "Promethium",
                "Polonium", "Astatine", "Radon", "Francium", "Radium", "Actinium", "Protactinium",
                "Neptunium", "Americium", "Curium", "Berkelium", "Californium", "Einsteinium", "Mudblaznium" };
            
            var comp = Context.RenderComponent<Pie>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Pie)
                .Add(p => p.MudChartParent, new MudChart())
                .Add(p => p.Height, "300px")
                .Add(p => p.Width, "300px")
                .Add(p => p.InputData, data)
                .Add(p => p.InputLabels,labels));

            var t = comp;
        }
    }
}
