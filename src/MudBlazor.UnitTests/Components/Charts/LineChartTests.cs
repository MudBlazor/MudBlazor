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
                var path = comp.Find("path.mud-chart-line");
                var d = path.GetAttribute("d");

                switch (opt)
                {
                    case InterpolationOption.NaturalSpline:
                        d.Should().Contain("M 30 36.3462 L 37.375 30.6726 L 44.75 25.4186 L 52.125 21.0035 L 59.5 17.8469 L 66.875 16.3683 L 74.25 16.9872 L 81.625 20.123 L 89 26.1952 L 96.375 35.6233 L 103.75 48.8269 L 111.125 65.9648 L 118.5 86.1532 L 125.875 108.2477 L 133.25 131.104 L 140.625 153.5777 L 148 174.5244 L 155.375 192.7998 L 162.75 207.2594 L 170.125 216.7589 L 177.5 220.1538 L 184.875 216.6935 L 192.25 207.2014 L 199.625 192.8946 L 207 174.9902 L 214.375 154.7054 L 221.75 133.2572 L 229.125 111.8628 L 236.5 91.7392 L 243.875 74.1036 L 251.25 60.1731 L 258.625 50.8404 L 266 45.7013 L 273.375 44.027 L 280.75 45.0889 L 288.125 48.1584 L 295.5 52.5067 L 302.875 57.4052 L 310.25 62.1253 L 317.625 65.9382 L 325 68.1154 L 332.375 68.1566 L 339.75 66.4757 L 347.125 63.7151 L 354.5 60.5171 L 361.875 57.524 L 369.25 55.3783 L 376.625 54.7222 L 384 56.1981 L 391.375 60.4484 L 398.75 68.1154 L 406.125 79.5683 L 413.5 94.0837 L 420.875 110.6649 L 428.25 128.3155 L 435.625 146.0388 L 443 162.8382 L 450.375 177.7171 L 457.75 189.679 L 465.125 197.7273 L 472.5 200.8654 L 479.875 198.4184 L 487.25 190.9987 L 494.625 179.5404 L 502 164.9775 L 509.375 148.244 L 516.75 130.2741 L 524.125 112.0019 L 531.5 94.3613 L 538.875 78.2865 L 546.25 64.7115 L 553.625 54.3576 L 561 47.0941 L 568.375 42.5778 L 575.75 40.4653 L 583.125 40.4131 L 590.5 42.0778 L 597.875 45.1161 L 605.25 49.1846 L 612.625 53.9398 L 620 59.0385");
                        break;
                    case InterpolationOption.Straight:
                        d.Should().Contain("M 30 36.3462 L 103.75 48.8269 L 177.5 220.1538 L 251.25 60.1731 L 325 68.1154 L 398.75 68.1154 L 472.5 200.8654 L 546.25 64.7115 L 620 59.0385");
                        break;
                    case InterpolationOption.EndSlope:
                        d.Should().Contain("M 30 36.3462 L 37.375 35.4633 L 44.75 33.2625 L 52.125 30.4156 L 59.5 27.5944 L 66.875 25.4707 L 74.25 24.7163 L 81.625 26.0029 L 89 30.0023 L 96.375 37.3864 L 103.75 48.8269 L 111.125 64.6817 L 118.5 84.0526 L 125.875 105.7275 L 133.25 128.4946 L 140.625 151.1415 L 148 172.4564 L 155.375 191.2272 L 162.75 206.2417 L 170.125 216.2879 L 177.5 220.1538 L 184.875 217.0353 L 192.25 207.7599 L 199.625 193.5631 L 207 175.6805 L 214.375 155.3477 L 221.75 133.8001 L 229.125 112.2733 L 236.5 92.0029 L 243.875 74.2243 L 251.25 60.1731 L 258.625 50.7564 L 266 45.568 L 273.375 43.8732 L 280.75 44.9372 L 288.125 48.0254 L 295.5 52.4031 L 302.875 57.3356 L 310.25 62.0883 L 317.625 65.9265 L 325 68.1154 L 332.375 68.1508 L 339.75 66.4504 L 347.125 63.6621 L 354.5 60.4338 L 361.875 57.4137 L 369.25 55.2498 L 376.625 54.59 L 384 56.0823 L 391.375 60.3747 L 398.75 68.1154 L 406.125 79.6753 L 413.5 94.3182 L 420.875 111.0309 L 428.25 128.8002 L 435.625 146.6129 L 443 163.4558 L 450.375 178.3157 L 457.75 190.1794 L 465.125 198.0337 L 472.5 200.8654 L 479.875 197.996 L 487.25 190.0857 L 494.625 178.1293 L 502 163.1217 L 509.375 146.0577 L 516.75 127.9322 L 524.125 109.7399 L 531.5 92.4759 L 538.875 77.1348 L 546.25 64.7115 L 553.625 55.9402 L 561 50.5117 L 568.375 47.8561 L 575.75 47.4035 L 583.125 48.5841 L 590.5 50.828 L 597.875 53.5652 L 605.25 56.226 L 612.625 58.2404 L 620 59.0385");
                        break;
                    case InterpolationOption.Periodic:
                        d.Should().Contain("M 30 36.3462 L 37.375 36.1646 L 44.75 34.4108 L 52.125 31.7935 L 59.5 29.0214 L 66.875 26.8032 L 74.25 25.8477 L 81.625 26.8636 L 89 30.5596 L 96.375 37.6445 L 103.75 48.8269 L 111.125 64.4939 L 118.5 83.7452 L 125.875 105.3589 L 133.25 128.1129 L 140.625 150.7853 L 148 172.1541 L 155.375 190.9974 L 162.75 206.093 L 170.125 216.2192 L 177.5 220.1538 L 184.875 217.085 L 192.25 207.841 L 199.625 193.66 L 207 175.7803 L 214.375 155.4402 L 221.75 133.878 L 229.125 112.3319 L 236.5 92.0402 L 243.875 74.2412 L 251.25 60.1731 L 258.625 50.7452 L 266 45.5508 L 273.375 43.8542 L 280.75 44.9196 L 288.125 48.0114 L 295.5 52.3938 L 302.875 57.3311 L 310.25 62.0877 L 317.625 65.9276 L 325 68.1154 L 332.375 68.146 L 339.75 66.4381 L 347.125 63.641 L 354.5 60.4042 L 361.875 57.3771 L 369.25 55.209 L 376.625 54.5494 L 384 56.0476 L 391.375 60.3532 L 398.75 68.1154 L 406.125 79.7058 L 413.5 94.3846 L 420.875 111.134 L 428.25 128.9363 L 435.625 146.7736 L 443 163.6283 L 450.375 178.4825 L 457.75 190.3186 L 465.125 198.1188 L 472.5 200.8654 L 479.875 197.8788 L 487.25 189.8324 L 494.625 177.738 L 502 162.6071 L 509.375 145.4516 L 516.75 127.283 L 524.125 109.113 L 531.5 91.9533 L 538.875 76.8156 L 546.25 64.7115 L 553.625 56.3787 L 561 51.4586 L 568.375 49.3185 L 575.75 49.3258 L 583.125 50.848 L 590.5 53.2522 L 597.875 55.9061 L 605.25 58.1768 L 612.625 59.4318 L 620 59.0385");
                        break;
                }
            }

            if (comp.Instance.ChartOptions.InterpolationOption == InterpolationOption.Straight && chartSeries.FirstOrDefault(x => x.Name == "Series 2") is not null)
            {
                var path = comp.FindAll("path.mud-chart-line").Skip(1).First();
                var d = path.GetAttribute("d");

                d.Should().Contain("M 30 127.1154 L 103.75 91.9423 L 177.5 98.75 L 251.25 80.5962 L 325 82.8654 L 398.75 68.1154 L 472.5 216.75 L 546.25 35.2115 L 620 306.3846");
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

            var path = comp.Find("path.mud-chart-line");
            var d = path.GetAttribute("d");

            switch (opt)
            {
                case InterpolationOption.NaturalSpline:
                    d.Should().Contain("M 30 320 L 37.375 320 L 44.75 320 L 52.125 320 L 59.5 320 L 66.875 320 L 74.25 320 L 81.625 320 L 89 320 L 96.375 320 L 103.75 320 L 111.125 320 L 118.5 320 L 125.875 320 L 133.25 320 L 140.625 320 L 148 320 L 155.375 320 L 162.75 320 L 170.125 320 L 177.5 320 L 184.875 320 L 192.25 320 L 199.625 320 L 207 320 L 214.375 320 L 221.75 320 L 229.125 320 L 236.5 320 L 243.875 320 L 251.25 320 L 258.625 320 L 266 320 L 273.375 320 L 280.75 320 L 288.125 320 L 295.5 320 L 302.875 320 L 310.25 320 L 317.625 320 L 325 320 L 332.375 320 L 339.75 320 L 347.125 320 L 354.5 320 L 361.875 320 L 369.25 320 L 376.625 320 L 384 320 L 391.375 320 L 398.75 320 L 406.125 320 L 413.5 320 L 420.875 320 L 428.25 320 L 435.625 320 L 443 320 L 450.375 320 L 457.75 320 L 465.125 320 L 472.5 320 L 479.875 320 L 487.25 320 L 494.625 320 L 502 320 L 509.375 320 L 516.75 320 L 524.125 320 L 531.5 320 L 538.875 320 L 546.25 320 L 553.625 320 L 561 320 L 568.375 320 L 575.75 320 L 583.125 320 L 590.5 320 L 597.875 320 L 605.25 320 L 612.625 320 L 620 320");
                    break;
                case InterpolationOption.Straight:
                    d.Should().Contain("M 30 320 L 103.75 320 L 177.5 320 L 251.25 320 L 325 320 L 398.75 320 L 472.5 320 L 546.25 320 L 620 320");
                    break;
                case InterpolationOption.EndSlope:
                    d.Should().Contain("M 30 320 L 37.375 320 L 44.75 320 L 52.125 320 L 59.5 320 L 66.875 320 L 74.25 320 L 81.625 320 L 89 320 L 96.375 320 L 103.75 320 L 111.125 320 L 118.5 320 L 125.875 320 L 133.25 320 L 140.625 320 L 148 320 L 155.375 320 L 162.75 320 L 170.125 320 L 177.5 320 L 184.875 320 L 192.25 320 L 199.625 320 L 207 320 L 214.375 320 L 221.75 320 L 229.125 320 L 236.5 320 L 243.875 320 L 251.25 320 L 258.625 320 L 266 320 L 273.375 320 L 280.75 320 L 288.125 320 L 295.5 320 L 302.875 320 L 310.25 320 L 317.625 320 L 325 320 L 332.375 320 L 339.75 320 L 347.125 320 L 354.5 320 L 361.875 320 L 369.25 320 L 376.625 320 L 384 320 L 391.375 320 L 398.75 320 L 406.125 320 L 413.5 320 L 420.875 320 L 428.25 320 L 435.625 320 L 443 320 L 450.375 320 L 457.75 320 L 465.125 320 L 472.5 320 L 479.875 320 L 487.25 320 L 494.625 320 L 502 320 L 509.375 320 L 516.75 320 L 524.125 320 L 531.5 320 L 538.875 320 L 546.25 320 L 553.625 320 L 561 320 L 568.375 320 L 575.75 320 L 583.125 320 L 590.5 320 L 597.875 320 L 605.25 320 L 612.625 320 L 620 320");
                    break;
                case InterpolationOption.Periodic:
                    d.Should().Contain("M 30 320 L 37.375 320 L 44.75 320 L 52.125 320 L 59.5 320 L 66.875 320 L 74.25 320 L 81.625 320 L 89 320 L 96.375 320 L 103.75 320 L 111.125 320 L 118.5 320 L 125.875 320 L 133.25 320 L 140.625 320 L 148 320 L 155.375 320 L 162.75 320 L 170.125 320 L 177.5 320 L 184.875 320 L 192.25 320 L 199.625 320 L 207 320 L 214.375 320 L 221.75 320 L 229.125 320 L 236.5 320 L 243.875 320 L 251.25 320 L 258.625 320 L 266 320 L 273.375 320 L 280.75 320 L 288.125 320 L 295.5 320 L 302.875 320 L 310.25 320 L 317.625 320 L 325 320 L 332.375 320 L 339.75 320 L 347.125 320 L 354.5 320 L 361.875 320 L 369.25 320 L 376.625 320 L 384 320 L 391.375 320 L 398.75 320 L 406.125 320 L 413.5 320 L 420.875 320 L 428.25 320 L 435.625 320 L 443 320 L 450.375 320 L 457.75 320 L 465.125 320 L 472.5 320 L 479.875 320 L 487.25 320 L 494.625 320 L 502 320 L 509.375 320 L 516.75 320 L 524.125 320 L 531.5 320 L 538.875 320 L 546.25 320 L 553.625 320 L 561 320 L 568.375 320 L 575.75 320 L 583.125 320 L 590.5 320 L 597.875 320 L 605.25 320 L 612.625 320 L 620 320");
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
