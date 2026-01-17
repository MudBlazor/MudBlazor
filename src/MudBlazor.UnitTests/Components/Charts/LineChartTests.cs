// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using AngleSharp.Dom;
using AwesomeAssertions;
using Bunit;
using MudBlazor.Charts;
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
            var comp = Context.Render<Bar<double>>();
            comp.Markup.Should().Contain("mud-chart");
        }

        [Theory]
        [TestCaseSource("GetInterpolationOptions")]
        public async Task LineChartExampleData(InterpolationOption opt)
        {
            var chartSeries = new List<ChartSeries<double>>()
            {
                new ChartSeries<double>() { Name = "Series 1", Data = new double[] { 90, 79, -72, 69, 62, 62, -55, 65, 70 } },
                new ChartSeries<double>() { Name = "Series 2", Data = new double[] { 10, 41, 35, 51, 49, 62, -69, 91, -148 } },
                new ChartSeries<double>() { Name = "Series 3", Data = new double[] { 10, 41, 35, 51, 49, 62, -69, 91, -148 }, Visible = false }
            };
            string[] xAxisLabels = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep" };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Line)
                .Add(p => p.Height, "350px")
                .Add(p => p.Width, "100%")
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels)
                .Add(p => p.ChartOptions, new LineChartOptions { ChartPalette = _baseChartPalette, InterpolationOption = opt }));

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
                        d.Should().Contain("M 30 36.3462 L 38 30.6726 L 46 25.4186 L 54 21.0035 L 62 17.8469 L 70 16.3683 L 78 16.9872 L 86 20.123 L 94 26.1952 L 102 35.6233 L 110 48.8269 L 118 65.9648 L 126 86.1532 L 134 108.2477 L 142 131.104 L 150 153.5777 L 158 174.5244 L 166 192.7998 L 174 207.2594 L 182 216.7589 L 190 220.1538 L 198 216.6935 L 206 207.2014 L 214 192.8946 L 222 174.9902 L 230 154.7054 L 238 133.2572 L 246 111.8628 L 254 91.7392 L 262 74.1036 L 270 60.1731 L 278 50.8404 L 286 45.7013 L 294 44.027 L 302 45.0889 L 310 48.1584 L 318 52.5067 L 326 57.4052 L 334 62.1253 L 342 65.9382 L 350 68.1154 L 358 68.1566 L 366 66.4757 L 374 63.7151 L 382 60.5171 L 390 57.524 L 398 55.3783 L 406 54.7222 L 414 56.1981 L 422 60.4484 L 430 68.1154 L 438 79.5683 L 446 94.0837 L 454 110.6649 L 462 128.3155 L 470 146.0388 L 478 162.8382 L 486 177.7171 L 494 189.679 L 502 197.7273 L 510 200.8654 L 518 198.4184 L 526 190.9987 L 534 179.5404 L 542 164.9775 L 550 148.244 L 558 130.2741 L 566 112.0019 L 574 94.3613 L 582 78.2865 L 590 64.7115 L 598 54.3576 L 606 47.0941 L 614 42.5778 L 622 40.4653 L 630 40.4131 L 638 42.0778 L 646 45.1161 L 654 49.1846 L 662 53.9398 L 670 59.0385");
                        break;
                    case InterpolationOption.Straight:
                        d.Should().Contain("M 30 36.3462 L 110 48.8269 L 190 220.1538 L 270 60.1731 L 350 68.1154 L 430 68.1154 L 510 200.8654 L 590 64.7115 L 670 59.0385");
                        break;
                    case InterpolationOption.EndSlope:
                        d.Should().Contain("M 30 36.3462 L 38 35.4633 L 46 33.2625 L 54 30.4156 L 62 27.5944 L 70 25.4707 L 78 24.7163 L 86 26.0029 L 94 30.0023 L 102 37.3864 L 110 48.8269 L 118 64.6817 L 126 84.0526 L 134 105.7275 L 142 128.4946 L 150 151.1415 L 158 172.4564 L 166 191.2272 L 174 206.2417 L 182 216.2879 L 190 220.1538 L 198 217.0353 L 206 207.7599 L 214 193.5631 L 222 175.6805 L 230 155.3477 L 238 133.8001 L 246 112.2733 L 254 92.0029 L 262 74.2243 L 270 60.1731 L 278 50.7564 L 286 45.568 L 294 43.8732 L 302 44.9372 L 310 48.0254 L 318 52.4031 L 326 57.3356 L 334 62.0883 L 342 65.9265 L 350 68.1154 L 358 68.1508 L 366 66.4504 L 374 63.6621 L 382 60.4338 L 390 57.4137 L 398 55.2498 L 406 54.59 L 414 56.0823 L 422 60.3747 L 430 68.1154 L 438 79.6753 L 446 94.3182 L 454 111.0309 L 462 128.8002 L 470 146.6129 L 478 163.4558 L 486 178.3157 L 494 190.1794 L 502 198.0337 L 510 200.8654 L 518 197.996 L 526 190.0857 L 534 178.1293 L 542 163.1217 L 550 146.0577 L 558 127.9322 L 566 109.7399 L 574 92.4759 L 582 77.1348 L 590 64.7115 L 598 55.9402 L 606 50.5117 L 614 47.8561 L 622 47.4035 L 630 48.5841 L 638 50.828 L 646 53.5652 L 654 56.226 L 662 58.2404 L 670 59.0385");
                        break;
                    case InterpolationOption.Periodic:
                        d.Should().Contain("M 30 36.3462 L 38 36.1646 L 46 34.4108 L 54 31.7935 L 62 29.0214 L 70 26.8032 L 78 25.8477 L 86 26.8636 L 94 30.5596 L 102 37.6445 L 110 48.8269 L 118 64.4939 L 126 83.7452 L 134 105.3589 L 142 128.1129 L 150 150.7853 L 158 172.1541 L 166 190.9974 L 174 206.093 L 182 216.2192 L 190 220.1538 L 198 217.085 L 206 207.841 L 214 193.66 L 222 175.7803 L 230 155.4402 L 238 133.878 L 246 112.3319 L 254 92.0402 L 262 74.2412 L 270 60.1731 L 278 50.7452 L 286 45.5508 L 294 43.8542 L 302 44.9196 L 310 48.0114 L 318 52.3938 L 326 57.3311 L 334 62.0877 L 342 65.9276 L 350 68.1154 L 358 68.146 L 366 66.4381 L 374 63.641 L 382 60.4042 L 390 57.3771 L 398 55.209 L 406 54.5494 L 414 56.0476 L 422 60.3532 L 430 68.1154 L 438 79.7058 L 446 94.3846 L 454 111.134 L 462 128.9363 L 470 146.7736 L 478 163.6283 L 486 178.4825 L 494 190.3186 L 502 198.1188 L 510 200.8654 L 518 197.8788 L 526 189.8324 L 534 177.738 L 542 162.6071 L 550 145.4516 L 558 127.283 L 566 109.113 L 574 91.9533 L 582 76.8156 L 590 64.7115 L 598 56.3787 L 606 51.4586 L 614 49.3185 L 622 49.3258 L 630 50.848 L 638 53.2522 L 646 55.9061 L 654 58.1768 L 662 59.4318 L 670 59.0385");
                        break;
                }
            }

            if (comp.Instance.ChartReference is Line<double> { ChartOptions.InterpolationOption: InterpolationOption.Straight } && chartSeries.FirstOrDefault(x => x.Name == "Series 2") is not null)
            {
                var path = comp.FindAll("path.mud-chart-line").Skip(1).First();
                var d = path.GetAttribute("d");

                d.Should().Contain("M 30 127.1154 L 110 91.9423 L 190 98.75 L 270 80.5962 L 350 82.8654 L 430 68.1154 L 510 216.75 L 590 35.2115 L 670 306.3846");
            }

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.ChartOptions, new ChartOptions() { ChartPalette = _modifiedPalette }));

            comp.Markup.Should().Contain(_modifiedPalette[0]);

            comp.Markup.Should().Contain("class=\"mud-charts-xaxis\"");
            comp.Markup.Should().Contain("class=\"mud-charts-yaxis\"");
            comp.Markup.Should().Contain("mud-chart-legend-item");

            await comp.SetParametersAndRenderAsync(parameters => parameters
                .Add(p => p.CanHideSeries, true)
                .Add(p => p.ChartOptions, new LineChartOptions() { ChartPalette = _baseChartPalette, InterpolationOption = opt }));

            if (comp.Instance.CanHideSeries)
            {
                var seriesCheckboxes = comp.FindAll(".mud-checkbox-input");
                seriesCheckboxes[0].IsChecked().Should().BeTrue("Series 1 checkbox initially checked");
                seriesCheckboxes[1].IsChecked().Should().BeTrue("Series 2 checkbox initially checked");
                seriesCheckboxes[2].IsChecked().Should().BeFalse("Series 3 checkbox initially unchecked");

                var series1 = "[stroke='#2979FF']";
                var series2 = "[stroke='#1DE9B6']";
                var series3 = "[stroke='#FFC400']";

                comp.FindAll($"path.mud-chart-line{series1}").Count.Should().Be(1, "Series 1 path expected to be visible");
                comp.FindAll($"path.mud-chart-line{series2}").Count.Should().Be(1, "Series 2 path expected to be visible");
                comp.FindAll($"path.mud-chart-line{series3}").Count.Should().Be(0, "Series 3 path expected to be hidden");

                // Hide Series 1
                await comp.InvokeAsync(() => seriesCheckboxes[0].ChangeAsync(false));

                seriesCheckboxes = comp.FindAll(".mud-checkbox-input");
                seriesCheckboxes[0].IsChecked().Should().BeFalse("Series 1 checkbox hidden");
                chartSeries[0].Visible.Should().BeFalse("Series 1 data Visible false");

                comp.FindAll($"path.mud-chart-line{series1}").Count.Should().Be(0, "Series 1 path hidden");
                comp.FindAll($"path.mud-chart-line{series2}").Count.Should().Be(1, "Series 2 path still visible");
                comp.FindAll($"path.mud-chart-line{series3}").Count.Should().Be(0, "Series 3 path still hidden");

                // Show Series 1 again
                await comp.InvokeAsync(() => seriesCheckboxes[0].ChangeAsync(true));

                seriesCheckboxes = comp.FindAll(".mud-checkbox-input");
                seriesCheckboxes[0].IsChecked().Should().BeTrue("Series 1 checkbox visible again");
                chartSeries[0].Visible.Should().BeTrue("Series 1 data Visible true");
                comp.FindAll($"path.mud-chart-line{series1}").Count.Should().Be(1, "Series 1 path visible again");

                // Show Series 3 (was initially hidden)
                await comp.InvokeAsync(() => seriesCheckboxes[2].ChangeAsync(true));

                seriesCheckboxes = comp.FindAll(".mud-checkbox-input");
                seriesCheckboxes[2].IsChecked().Should().BeTrue("Series 3 checkbox visible");
                chartSeries[2].Visible.Should().BeTrue("Series 3 data Visible true");

                comp.FindAll($"path.mud-chart-line{series3}").Count.Should().Be(1, "Series 3 path visible");
                comp.FindAll($"path.mud-chart-line{series1}").Count.Should().Be(1, "Series 1 path still visible (after Series 3 shown)");
                comp.FindAll($"path.mud-chart-line{series2}").Count.Should().Be(1, "Series 2 path still visible (after Series 3 shown)");

                // Hide Series 3 again
                await comp.InvokeAsync(() => seriesCheckboxes[2].ChangeAsync(false));

                seriesCheckboxes = comp.FindAll(".mud-checkbox-input");
                seriesCheckboxes[2].IsChecked().Should().BeFalse("Series 3 checkbox hidden again");
                chartSeries[2].Visible.Should().BeFalse("Series 3 data Visible false again");
                comp.FindAll($"path.mud-chart-line{series3}").Count.Should().Be(0, "Series 3 path hidden again");

                // Final checkbox states
                seriesCheckboxes[0].IsChecked().Should().BeTrue(); // Series 1 is visible
                seriesCheckboxes[1].IsChecked().Should().BeTrue(); // Series 2 was untouched and visible
                seriesCheckboxes[2].IsChecked().Should().BeFalse(); // Series 3 is hidden
            }
        }

        [Theory]
        [TestCaseSource("GetInterpolationOptions")]
        public async Task LineChartExampleZeroValues(InterpolationOption opt)
        {
            var chartSeries = new List<ChartSeries<double>>()
            {
                new() { Name = "Series 1", Data = new double[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 } }
            };
            string[] xAxisLabels = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep" };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Line)
                .Add(p => p.Height, "350px")
                .Add(p => p.Width, "100%")
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.ChartLabels, xAxisLabels)
                .Add(p => p.ChartOptions, new LineChartOptions { ChartPalette = _baseChartPalette, InterpolationOption = opt }));

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
                    d.Should().Contain("M 30 320 L 38 320 L 46 320 L 54 320 L 62 320 L 70 320 L 78 320 L 86 320 L 94 320 L 102 320 L 110 320 L 118 320 L 126 320 L 134 320 L 142 320 L 150 320 L 158 320 L 166 320 L 174 320 L 182 320 L 190 320 L 198 320 L 206 320 L 214 320 L 222 320 L 230 320 L 238 320 L 246 320 L 254 320 L 262 320 L 270 320 L 278 320 L 286 320 L 294 320 L 302 320 L 310 320 L 318 320 L 326 320 L 334 320 L 342 320 L 350 320 L 358 320 L 366 320 L 374 320 L 382 320 L 390 320 L 398 320 L 406 320 L 414 320 L 422 320 L 430 320 L 438 320 L 446 320 L 454 320 L 462 320 L 470 320 L 478 320 L 486 320 L 494 320 L 502 320 L 510 320 L 518 320 L 526 320 L 534 320 L 542 320 L 550 320 L 558 320 L 566 320 L 574 320 L 582 320 L 590 320 L 598 320 L 606 320 L 614 320 L 622 320 L 630 320 L 638 320 L 646 320 L 654 320 L 662 320 L 670 320");
                    break;
                case InterpolationOption.Straight:
                    d.Should().Contain("M 30 320 L 110 320 L 190 320 L 270 320 L 350 320 L 430 320 L 510 320 L 590 320 L 670 320");
                    break;
                case InterpolationOption.EndSlope:
                    d.Should().Contain("M 30 320 L 38 320 L 46 320 L 54 320 L 62 320 L 70 320 L 78 320 L 86 320 L 94 320 L 102 320 L 110 320 L 118 320 L 126 320 L 134 320 L 142 320 L 150 320 L 158 320 L 166 320 L 174 320 L 182 320 L 190 320 L 198 320 L 206 320 L 214 320 L 222 320 L 230 320 L 238 320 L 246 320 L 254 320 L 262 320 L 270 320 L 278 320 L 286 320 L 294 320 L 302 320 L 310 320 L 318 320 L 326 320 L 334 320 L 342 320 L 350 320 L 358 320 L 366 320 L 374 320 L 382 320 L 390 320 L 398 320 L 406 320 L 414 320 L 422 320 L 430 320 L 438 320 L 446 320 L 454 320 L 462 320 L 470 320 L 478 320 L 486 320 L 494 320 L 502 320 L 510 320 L 518 320 L 526 320 L 534 320 L 542 320 L 550 320 L 558 320 L 566 320 L 574 320 L 582 320 L 590 320 L 598 320 L 606 320 L 614 320 L 622 320 L 630 320 L 638 320 L 646 320 L 654 320 L 662 320 L 670 320");
                    break;
                case InterpolationOption.Periodic:
                    d.Should().Contain("M 30 320 L 38 320 L 46 320 L 54 320 L 62 320 L 70 320 L 78 320 L 86 320 L 94 320 L 102 320 L 110 320 L 118 320 L 126 320 L 134 320 L 142 320 L 150 320 L 158 320 L 166 320 L 174 320 L 182 320 L 190 320 L 198 320 L 206 320 L 214 320 L 222 320 L 230 320 L 238 320 L 246 320 L 254 320 L 262 320 L 270 320 L 278 320 L 286 320 L 294 320 L 302 320 L 310 320 L 318 320 L 326 320 L 334 320 L 342 320 L 350 320 L 358 320 L 366 320 L 374 320 L 382 320 L 390 320 L 398 320 L 406 320 L 414 320 L 422 320 L 430 320 L 438 320 L 446 320 L 454 320 L 462 320 L 470 320 L 478 320 L 486 320 L 494 320 L 502 320 L 510 320 L 518 320 L 526 320 L 534 320 L 542 320 L 550 320 L 558 320 L 566 320 L 574 320 L 582 320 L 590 320 L 598 320 L 606 320 L 614 320 L 622 320 L 630 320 L 638 320 L 646 320 L 654 320 L 662 320 L 670 320");
                    break;
            }

            await comp.SetParametersAndRenderAsync(parameters => parameters.Add(p => p.ChartOptions, new ChartOptions() { ChartPalette = _modifiedPalette }));

            comp.Markup.Should().Contain(_modifiedPalette[0]);
            comp.Markup.Should().Contain("class=\"mud-charts-xaxis\"");
            comp.Markup.Should().Contain("class=\"mud-charts-yaxis\"");
            comp.Markup.Should().Contain("mud-chart-legend-item");
        }

        [Test]
        public async Task LineChartColoring()
        {
            var chartSeries = new List<ChartSeries<double>>()
            {
                new ChartSeries<double>() { Name = "Deep Sea Blue", Data = new double[] { 40, 20, 25, 27, 46 } },
                new ChartSeries<double>() { Name = "Venetian Red", Data = new double[] { 19, 24, 35, 13, 28 } },
                new ChartSeries<double>() { Name = "Banana Yellow", Data = new double[] { 8, 6, 11, 13, 4 } },
                new ChartSeries<double>() { Name = "La Salle Green", Data = new double[] { 18, 9, 7, 10, 7 } },
                new ChartSeries<double>() { Name = "Rich Carmine", Data = new double[] { 9, 14, 6, 15, 20 } },
                new ChartSeries<double>() { Name = "Shiraz", Data = new double[] { 9, 4, 11, 5, 19 } },
                new ChartSeries<double>() { Name = "Cloud Burst", Data = new double[] { 14, 9, 20, 16, 6 } },
                new ChartSeries<double>() { Name = "Neon Pink", Data = new double[] { 14, 8, 4, 14, 8 } },
                new ChartSeries<double>() { Name = "Ocean", Data = new double[] { 11, 20, 13, 5, 5 } },
                new ChartSeries<double>() { Name = "Orangey Red", Data = new double[] { 6, 6, 19, 20, 6 } },
                new ChartSeries<double>() { Name = "Catalina Blue", Data = new double[] { 3, 2, 20, 3, 10 } },
                new ChartSeries<double>() { Name = "Fountain Blue", Data = new double[] { 3, 18, 11, 12, 3 } },
                new ChartSeries<double>() { Name = "Irish Green", Data = new double[] { 20, 5, 15, 16, 13 } },
                new ChartSeries<double>() { Name = "Wild Strawberry", Data = new double[] { 15, 9, 12, 12, 1 } },
                new ChartSeries<double>() { Name = "Geraldine", Data = new double[] { 5, 13, 19, 15, 8 } },
                new ChartSeries<double>() { Name = "Grey Teal", Data = new double[] { 12, 16, 20, 16, 17 } },
                new ChartSeries<double>() { Name = "Baby Pink", Data = new double[] { 1, 18, 10, 19, 8 } },
                new ChartSeries<double>() { Name = "Thunderbird", Data = new double[] { 15, 16, 10, 8, 5 } },
                new ChartSeries<double>() { Name = "Navy", Data = new double[] { 16, 2, 3, 5, 5 } },
                new ChartSeries<double>() { Name = "Aqua Marina", Data = new double[] { 17, 6, 11, 19, 6 } },
                new ChartSeries<double>() { Name = "Lavender Pinocchio", Data = new double[] { 1, 11, 4, 18, 1 } },
                new ChartSeries<double>() { Name = "Deep Sea Blue", Data = new double[] { 1, 11, 4, 18, 1 } }
            };

            var comp = Context.Render<MudChart<double>>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Line)
                .Add(p => p.Height, "350px")
                .Add(p => p.Width, "100%")
                .Add(p => p.ChartOptions, new ChartOptions { ChartPalette = new string[] { "#1E9AB0" } })
                .Add(p => p.ChartSeries, chartSeries));

            var paths1 = comp.FindAll("path");

            int count;
            count = paths1.Count(p => p.OuterHtml.Contains($"stroke=\"{"#1E9AB0"}\""));
            count.Should().Be(22);

            await comp.SetParametersAndRenderAsync(parameters => parameters
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
