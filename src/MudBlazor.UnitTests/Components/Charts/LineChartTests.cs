// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using MudBlazor.Charts;
using MudBlazor.UnitTests.Components;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Charts
{
    public class LineChartTests : BunitTest
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

        private readonly string[] _customPalette =
        {
            "#015482", "#CC1512", "#FFE135", "#087830", "#D70040", "#B20931", "#202E54", "#F535AA", "#017B92",
            "#FA4224", "#062A78", "#56B4BE", "#207000", "#FF43A4", "#FB8989", "#5E9B8A", "#FFB7CE", "#C02B18",
            "#01153E", "#2EE8BB", "#EBDDE2"
        };

        private static Array GetInterpolationOptions()
        {
            return Enum.GetValues(typeof(InterpolationOption));
        }

        [SetUp]
        public void Init()
        {

        }

        [Test]
        public void LineChartEmptyData()
        {
            var comp = Context.RenderComponent<Bar>();
            comp.Markup.Should().Contain("mud-chart");
        }

        [Theory]
        [TestCaseSource("GetInterpolationOptions")]
        public void LineChartExampleData(InterpolationOption opt)
        {
            var chartSeries = new List<ChartSeries>()
            {
                new ChartSeries() { Name = "Series 1", Data = new double[] { 90, 79, -72, 69, 62, 62, -55, 65, 70 } },
                new ChartSeries() { Name = "Series 2", Data = new double[] { 10, 41, 35, 51, 49, 62, -69, 91, -148 } },
                new ChartSeries() { Name = "Series 3", Data = new double[] { 10, 41, 35, 51, 49, 62, -69, 91, -148 }, Visible = false }
            };
            string[] xAxisLabels = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep" };

            var comp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Line)
                .Add(p => p.Height, "350px")
                .Add(p => p.Width, "100%")
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.XAxisLabels, xAxisLabels)
                .Add(p => p.ChartOptions, new ChartOptions { ChartPalette = _baseChartPalette, InterpolationOption = opt }));

            comp.Instance.ChartSeries.Should().NotBeEmpty();

            comp.Markup.Should().Contain("class=\"mud-charts-xaxis\"");
            comp.Markup.Should().Contain("class=\"mud-charts-yaxis\"");
            comp.Markup.Should().Contain("mud-chart-legend-item");

            if (chartSeries.Count <= 3)
            {
                comp.Markup.Should().
                    Contain("Series 1").And.Contain("Series 2");
            }

            if (chartSeries.FirstOrDefault(x => x.Name == "Series 1") is not null)
            {
                switch (opt)
                {
                    case InterpolationOption.NaturalSpline:
                        comp.Markup.Should().Contain("d=\"M 30 36.5385 L 37.375 30.7688 L 44.75 25.4257 L 52.125 20.9358 L 59.5 17.7257 L 66.875 16.222 L 74.25 16.8514 L 81.625 20.0403 L 89 26.2154 L 96.375 35.8034 L 103.75 49.2308 L 111.125 66.6591 L 118.5 87.1897 L 125.875 109.6587 L 133.25 132.9024 L 140.625 155.757 L 148 177.0588 L 155.375 195.6439 L 162.75 210.3485 L 170.125 220.009 L 177.5 223.4615 L 184.875 219.9425 L 192.25 210.2895 L 199.625 195.7403 L 207 177.5324 L 214.375 156.9038 L 221.75 135.0921 L 229.125 113.335 L 236.5 92.8704 L 243.875 74.9359 L 251.25 60.7692 L 258.625 51.2784 L 266 46.0522 L 273.375 44.3495 L 280.75 45.4294 L 288.125 48.5509 L 295.5 52.9729 L 302.875 57.9545 L 310.25 62.7545 L 317.625 66.6321 L 325 68.8462 L 332.375 68.8881 L 339.75 67.1787 L 347.125 64.3713 L 354.5 61.1191 L 361.875 58.0753 L 369.25 55.8932 L 376.625 55.226 L 384 56.7269 L 391.375 61.0492 L 398.75 68.8462 L 406.125 80.4932 L 413.5 95.2546 L 420.875 112.1169 L 428.25 130.0666 L 435.625 148.0903 L 443 165.1744 L 450.375 180.3055 L 457.75 192.4702 L 465.125 200.6549 L 472.5 203.8462 L 479.875 201.3577 L 487.25 193.8123 L 494.625 182.1597 L 502 167.35 L 509.375 150.3329 L 516.75 132.0584 L 524.125 113.4765 L 531.5 95.5369 L 538.875 79.1897 L 546.25 65.3846 L 553.625 54.8552 L 561 47.4686 L 568.375 42.8758 L 575.75 40.7274 L 583.125 40.6743 L 590.5 42.3672 L 597.875 45.457 L 605.25 49.5945 L 612.625 54.4303 L 620 59.6154\"");
                        break;
                    case InterpolationOption.Straight:
                        comp.Markup.Should()
                            .Contain("d=\"M 30 36.5385 L 103.75 49.2308 L 177.5 223.4615 L 251.25 60.7692 L 325 68.8462 L 398.75 68.8462 L 472.5 203.8462 L 546.25 65.3846 L 620 59.6154\"");
                        break;
                    case InterpolationOption.EndSlope:
                        comp.Markup.Should().Contain("d=\"M 30 36.5385 L 37.375 35.6406 L 44.75 33.4025 L 52.125 30.5074 L 59.5 27.6384 L 66.875 25.4787 L 74.25 24.7115 L 81.625 26.0199 L 89 30.0871 L 96.375 37.5964 L 103.75 49.2308 L 111.125 65.3542 L 118.5 85.0535 L 125.875 107.0958 L 133.25 130.2487 L 140.625 153.2795 L 148 174.9557 L 155.375 194.0446 L 162.75 209.3136 L 170.125 219.5301 L 177.5 223.4615 L 184.875 220.2901 L 192.25 210.8575 L 199.625 196.4201 L 207 178.2344 L 214.375 157.557 L 221.75 135.6442 L 229.125 113.7525 L 236.5 93.1385 L 243.875 75.0586 L 251.25 60.7692 L 258.625 51.193 L 266 45.9166 L 273.375 44.193 L 280.75 45.2751 L 288.125 48.4156 L 295.5 52.8676 L 302.875 57.8837 L 310.25 62.7169 L 317.625 66.6201 L 325 68.8462 L 332.375 68.8822 L 339.75 67.1529 L 347.125 64.3174 L 354.5 61.0344 L 361.875 57.9631 L 369.25 55.7625 L 376.625 55.0915 L 384 56.6091 L 391.375 60.9743 L 398.75 68.8462 L 406.125 80.602 L 413.5 95.4931 L 420.875 112.4891 L 428.25 130.5596 L 435.625 148.6741 L 443 165.8025 L 450.375 180.9142 L 457.75 192.979 L 465.125 200.9664 L 472.5 203.8462 L 479.875 200.9281 L 487.25 192.8838 L 494.625 180.7247 L 502 165.4628 L 509.375 148.1095 L 516.75 129.6768 L 524.125 111.1762 L 531.5 93.6195 L 538.875 78.0184 L 546.25 65.3846 L 553.625 56.4646 L 561 50.9441 L 568.375 48.2435 L 575.75 47.7833 L 583.125 48.9839 L 590.5 51.2658 L 597.875 54.0494 L 605.25 56.7553 L 612.625 58.8038 L 620 59.6154\"");
                        break;
                    case InterpolationOption.Periodic:
                        comp.Markup.Should().Contain("d=\"M 30 36.5385 L 37.375 36.3538 L 44.75 34.5703 L 52.125 31.9087 L 59.5 29.0896 L 66.875 26.8338 L 74.25 25.8621 L 81.625 26.8952 L 89 30.6538 L 96.375 37.8588 L 103.75 49.2308 L 111.125 65.1633 L 118.5 84.7409 L 125.875 106.7209 L 133.25 129.8605 L 140.625 152.9172 L 148 174.6482 L 155.375 193.8109 L 162.75 209.1624 L 170.125 219.4602 L 177.5 223.4615 L 184.875 220.3407 L 192.25 210.94 L 199.625 196.5187 L 207 178.3359 L 214.375 157.6511 L 221.75 135.7234 L 229.125 113.8121 L 236.5 93.1765 L 243.875 75.0758 L 251.25 60.7692 L 258.625 51.1816 L 266 45.8991 L 273.375 44.1738 L 280.75 45.2573 L 288.125 48.4014 L 295.5 52.8581 L 302.875 57.8791 L 310.25 62.7163 L 317.625 66.6213 L 325 68.8462 L 332.375 68.8773 L 339.75 67.1404 L 347.125 64.296 L 354.5 61.0043 L 361.875 57.9258 L 369.25 55.721 L 376.625 55.0502 L 384 56.5738 L 391.375 60.9524 L 398.75 68.8462 L 406.125 80.6331 L 413.5 95.5607 L 420.875 112.5939 L 428.25 130.6979 L 435.625 148.8376 L 443 165.9779 L 450.375 181.0839 L 457.75 193.1207 L 465.125 201.0531 L 472.5 203.8462 L 479.875 200.8089 L 487.25 192.6262 L 494.625 180.3267 L 502 164.9395 L 509.375 147.4931 L 516.75 129.0166 L 524.125 110.5387 L 531.5 93.0881 L 538.875 77.6938 L 546.25 65.3846 L 553.625 56.9106 L 561 51.907 L 568.375 49.7307 L 575.75 49.7381 L 583.125 51.2861 L 590.5 53.7311 L 597.875 56.4299 L 605.25 58.7391 L 612.625 60.0154 L 620 59.6154\"");
                        break;
                }
            }

            if (comp.Instance.ChartOptions.InterpolationOption == InterpolationOption.Straight && chartSeries.FirstOrDefault(x => x.Name == "Series 2") is not null)
            {
                comp.Markup.Should()
                    .Contain("d=\"M 30 128.8462 L 103.75 93.0769 L 177.5 100 L 251.25 81.5385 L 325 83.8462 L 398.75 68.8462 L 472.5 220 L 546.25 35.3846 L 620 311.1538\"");
            }

            comp.SetParametersAndRender(parameters => parameters
                .Add(p => p.ChartOptions, new ChartOptions() { ChartPalette = _modifiedPalette }));

            comp.Markup.Should().Contain(_modifiedPalette[0]);

            comp.Markup.Should().Contain("class=\"mud-charts-xaxis\"");
            comp.Markup.Should().Contain("class=\"mud-charts-yaxis\"");
            comp.Markup.Should().Contain("mud-chart-legend-item");

            comp.SetParametersAndRender(parameters => parameters
                .Add(p => p.CanHideSeries, true)
                .Add(p => p.ChartOptions, new ChartOptions() { ChartPalette = _baseChartPalette, InterpolationOption = opt }));

            if (comp.Instance.CanHideSeries)
            {
                var seriesCheckboxes = comp.FindAll(".mud-checkbox-input");

                comp.InvokeAsync(() =>
                {
                    seriesCheckboxes[0].Change(false);
                });

                seriesCheckboxes = comp.FindAll(".mud-checkbox-input");

                comp.InvokeAsync(() =>
                {
                    seriesCheckboxes[2].Change(true);
                });

                seriesCheckboxes = comp.FindAll(".mud-checkbox-input");

                seriesCheckboxes[0].IsChecked().Should().BeFalse();
                seriesCheckboxes[1].IsChecked().Should().BeTrue();
                seriesCheckboxes[2].IsChecked().Should().BeTrue();
            }
        }

        [Theory]
        [TestCaseSource("GetInterpolationOptions")]
        public void LineChartExampleZeroValues(InterpolationOption opt)
        {
            var chartSeries = new List<ChartSeries>()
            {
                new ChartSeries() { Name = "Series 1", Data = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 } }
            };
            string[] xAxisLabels = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep" };

            var comp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Line)
                .Add(p => p.Height, "350px")
                .Add(p => p.Width, "100%")
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.XAxisLabels, xAxisLabels)
                .Add(p => p.ChartOptions, new ChartOptions { ChartPalette = _baseChartPalette, InterpolationOption = opt }));

            comp.Instance.ChartSeries.Should().NotBeEmpty();

            comp.Markup.Should().Contain("class=\"mud-charts-xaxis\"");
            comp.Markup.Should().Contain("class=\"mud-charts-yaxis\"");
            comp.Markup.Should().Contain("mud-chart-legend-item");
            comp.Markup.Should().Contain("Series 1");

            switch (opt)
            {
                case InterpolationOption.NaturalSpline:
                    comp.Markup.Should().Contain("d=\"M 30 325 L 37.375 325 L 44.75 325 L 52.125 325 L 59.5 325 L 66.875 325 L 74.25 325 L 81.625 325 L 89 325 L 96.375 325 L 103.75 325 L 111.125 325 L 118.5 325 L 125.875 325 L 133.25 325 L 140.625 325 L 148 325 L 155.375 325 L 162.75 325 L 170.125 325 L 177.5 325 L 184.875 325 L 192.25 325 L 199.625 325 L 207 325 L 214.375 325 L 221.75 325 L 229.125 325 L 236.5 325 L 243.875 325 L 251.25 325 L 258.625 325 L 266 325 L 273.375 325 L 280.75 325 L 288.125 325 L 295.5 325 L 302.875 325 L 310.25 325 L 317.625 325 L 325 325 L 332.375 325 L 339.75 325 L 347.125 325 L 354.5 325 L 361.875 325 L 369.25 325 L 376.625 325 L 384 325 L 391.375 325 L 398.75 325 L 406.125 325 L 413.5 325 L 420.875 325 L 428.25 325 L 435.625 325 L 443 325 L 450.375 325 L 457.75 325 L 465.125 325 L 472.5 325 L 479.875 325 L 487.25 325 L 494.625 325 L 502 325 L 509.375 325 L 516.75 325 L 524.125 325 L 531.5 325 L 538.875 325 L 546.25 325 L 553.625 325 L 561 325 L 568.375 325 L 575.75 325 L 583.125 325 L 590.5 325 L 597.875 325 L 605.25 325 L 612.625 325 L 620 325\"");
                    break;
                case InterpolationOption.Straight:
                    comp.Markup.Should()
                        .Contain("d=\"M 30 325 L 103.75 325 L 177.5 325 L 251.25 325 L 325 325 L 398.75 325 L 472.5 325 L 546.25 325 L 620 325\"");
                    break;
                case InterpolationOption.EndSlope:
                    comp.Markup.Should().Contain("d=\"M 30 325 L 37.375 325 L 44.75 325 L 52.125 325 L 59.5 325 L 66.875 325 L 74.25 325 L 81.625 325 L 89 325 L 96.375 325 L 103.75 325 L 111.125 325 L 118.5 325 L 125.875 325 L 133.25 325 L 140.625 325 L 148 325 L 155.375 325 L 162.75 325 L 170.125 325 L 177.5 325 L 184.875 325 L 192.25 325 L 199.625 325 L 207 325 L 214.375 325 L 221.75 325 L 229.125 325 L 236.5 325 L 243.875 325 L 251.25 325 L 258.625 325 L 266 325 L 273.375 325 L 280.75 325 L 288.125 325 L 295.5 325 L 302.875 325 L 310.25 325 L 317.625 325 L 325 325 L 332.375 325 L 339.75 325 L 347.125 325 L 354.5 325 L 361.875 325 L 369.25 325 L 376.625 325 L 384 325 L 391.375 325 L 398.75 325 L 406.125 325 L 413.5 325 L 420.875 325 L 428.25 325 L 435.625 325 L 443 325 L 450.375 325 L 457.75 325 L 465.125 325 L 472.5 325 L 479.875 325 L 487.25 325 L 494.625 325 L 502 325 L 509.375 325 L 516.75 325 L 524.125 325 L 531.5 325 L 538.875 325 L 546.25 325 L 553.625 325 L 561 325 L 568.375 325 L 575.75 325 L 583.125 325 L 590.5 325 L 597.875 325 L 605.25 325 L 612.625 325 L 620 325\"");
                    break;
                case InterpolationOption.Periodic:
                    comp.Markup.Should().Contain("d=\"M 30 325 L 37.375 325 L 44.75 325 L 52.125 325 L 59.5 325 L 66.875 325 L 74.25 325 L 81.625 325 L 89 325 L 96.375 325 L 103.75 325 L 111.125 325 L 118.5 325 L 125.875 325 L 133.25 325 L 140.625 325 L 148 325 L 155.375 325 L 162.75 325 L 170.125 325 L 177.5 325 L 184.875 325 L 192.25 325 L 199.625 325 L 207 325 L 214.375 325 L 221.75 325 L 229.125 325 L 236.5 325 L 243.875 325 L 251.25 325 L 258.625 325 L 266 325 L 273.375 325 L 280.75 325 L 288.125 325 L 295.5 325 L 302.875 325 L 310.25 325 L 317.625 325 L 325 325 L 332.375 325 L 339.75 325 L 347.125 325 L 354.5 325 L 361.875 325 L 369.25 325 L 376.625 325 L 384 325 L 391.375 325 L 398.75 325 L 406.125 325 L 413.5 325 L 420.875 325 L 428.25 325 L 435.625 325 L 443 325 L 450.375 325 L 457.75 325 L 465.125 325 L 472.5 325 L 479.875 325 L 487.25 325 L 494.625 325 L 502 325 L 509.375 325 L 516.75 325 L 524.125 325 L 531.5 325 L 538.875 325 L 546.25 325 L 553.625 325 L 561 325 L 568.375 325 L 575.75 325 L 583.125 325 L 590.5 325 L 597.875 325 L 605.25 325 L 612.625 325 L 620 325\"");
                    break;
            }

            comp.SetParametersAndRender(parameters => parameters.Add(p => p.ChartOptions, new ChartOptions() { ChartPalette = _modifiedPalette }));

            comp.Markup.Should().Contain(_modifiedPalette[0]);
            comp.Markup.Should().Contain("class=\"mud-charts-xaxis\"");
            comp.Markup.Should().Contain("class=\"mud-charts-yaxis\"");
            comp.Markup.Should().Contain("mud-chart-legend-item");
        }

        [Test]
        public void LineChartColoring()
        {
            var chartSeries = new List<ChartSeries>()
            {
                new ChartSeries() { Name = "Deep Sea Blue", Data = new double[] { 40, 20, 25, 27, 46 } },
                new ChartSeries() { Name = "Venetian Red", Data = new double[] { 19, 24, 35, 13, 28 } },
                new ChartSeries() { Name = "Banana Yellow", Data = new double[] { 8, 6, 11, 13, 4 } },
                new ChartSeries() { Name = "La Salle Green", Data = new double[] { 18, 9, 7, 10, 7 } },
                new ChartSeries() { Name = "Rich Carmine", Data = new double[] { 9, 14, 6, 15, 20 } },
                new ChartSeries() { Name = "Shiraz", Data = new double[] { 9, 4, 11, 5, 19 } },
                new ChartSeries() { Name = "Cloud Burst", Data = new double[] { 14, 9, 20, 16, 6 } },
                new ChartSeries() { Name = "Neon Pink", Data = new double[] { 14, 8, 4, 14, 8 } },
                new ChartSeries() { Name = "Ocean", Data = new double[] { 11, 20, 13, 5, 5 } },
                new ChartSeries() { Name = "Orangey Red", Data = new double[] { 6, 6, 19, 20, 6 } },
                new ChartSeries() { Name = "Catalina Blue", Data = new double[] { 3, 2, 20, 3, 10 } },
                new ChartSeries() { Name = "Fountain Blue", Data = new double[] { 3, 18, 11, 12, 3 } },
                new ChartSeries() { Name = "Irish Green", Data = new double[] { 20, 5, 15, 16, 13 } },
                new ChartSeries() { Name = "Wild Strawberry", Data = new double[] { 15, 9, 12, 12, 1 } },
                new ChartSeries() { Name = "Geraldine", Data = new double[] { 5, 13, 19, 15, 8 } },
                new ChartSeries() { Name = "Grey Teal", Data = new double[] { 12, 16, 20, 16, 17 } },
                new ChartSeries() { Name = "Baby Pink", Data = new double[] { 1, 18, 10, 19, 8 } },
                new ChartSeries() { Name = "Thunderbird", Data = new double[] { 15, 16, 10, 8, 5 } },
                new ChartSeries() { Name = "Navy", Data = new double[] { 16, 2, 3, 5, 5 } },
                new ChartSeries() { Name = "Aqua Marina", Data = new double[] { 17, 6, 11, 19, 6 } },
                new ChartSeries() { Name = "Lavender Pinocchio", Data = new double[] { 1, 11, 4, 18, 1 } },
                new ChartSeries() { Name = "Deep Sea Blue", Data = new double[] { 1, 11, 4, 18, 1 } }
            };

            var comp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Line)
                .Add(p => p.Height, "350px")
                .Add(p => p.Width, "100%")
                .Add(p => p.ChartOptions, new ChartOptions { ChartPalette = new string[] { "#1E9AB0" } })
                .Add(p => p.ChartSeries, chartSeries));

            var paths1 = comp.FindAll("path");

            int count;
            count = paths1.Count(p => p.OuterHtml.Contains($"stroke=\"{"#1E9AB0"}\""));
            count.Should().Be(22);

            comp.SetParametersAndRender(parameters => parameters
                .Add(p => p.ChartOptions, new ChartOptions() { ChartPalette = _customPalette }));

            var paths2 = comp.FindAll("path");

            foreach (var color in _customPalette)
            {
                count = paths2.Count(p => p.OuterHtml.Contains($"stroke=\"{color}\""));
                if (color == _customPalette[0])
                {
                    count.Should().Be(2, because: "the number of series defined exceeds the number of colors in the chart palette, thus, any new defined series takes the color from the chart palette in the same fashion as the previous series starting from the beginning");
                }
                else
                {
                    count.Should().Be(1);
                }
            }
        }
    }
}
