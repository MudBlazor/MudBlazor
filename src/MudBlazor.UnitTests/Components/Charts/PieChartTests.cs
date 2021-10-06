﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
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
        [SetUp]
        public void Init()
        {
            
        }

        [Test]
        public void PieChartEmptyData()
        {
            var comp = Context.RenderComponent<Pie>();
            comp.Markup.Should().Contain("mud-chart-pie");
        }

        [Test]
        [TestCase(new double[]{77, 25, 20, 5})]
        [TestCase(new double[]{77, 25, 20, 5, 8})]
        public void PieChartExampleData(double[] data)
        {
            string[] labels = { "Uranium", "Plutonium", "Thorium", "Caesium", "Technetium", "Promethium",
                "Polonium", "Astatine", "Radon", "Francium", "Radium", "Actinium", "Protactinium",
                "Neptunium", "Americium", "Curium", "Berkelium", "Californium", "Einsteinium", "Mudblaznium" };
            
            var comp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Pie)
                .Add(p => p.Height, "300px")
                .Add(p => p.Width, "300px")
                .Add(p => p.InputData, data)
                .Add(p => p.InputLabels,labels));
            
            comp.Markup.Should().Contain("class=\"mud-chart-pie\"");
            comp.Markup.Should().Contain("class=\"mud-chart-serie\"");
            comp.Markup.Should().Contain("mud-chart-legend-item");
            
            if (data.Length <= 4)
            {
                comp.Markup.Should().
                    Contain("Uranium").And.Contain("Plutonium").And.Contain("Thorium").And.Contain("Caesium");
            }
            
            if (data.Length >= 5)
            {
                comp.Markup.Should()
                    .Contain("Technetium");
            }

            if (data.Length == 4 && data.Contains(77))
            {
                comp.Markup.Should()
                    .Contain("M 1 0 A 1 1 0 1 1 -0.7851254621398548 -0.6193367490305087 L 0 0");
            }

            if (data.Length == 4 && data.Contains(5))
            {
                comp.Markup.Should()
                    .Contain("M 0.9695598647982466 -0.24485438238350116 A 1 1 0 0 1 1 -2.4492935982947064E-16 L 0 0");
            }
        }
    }
}
