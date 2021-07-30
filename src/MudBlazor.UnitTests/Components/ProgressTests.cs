
using System;
using Bunit;
using FluentAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ProgressTests : BunitTest
    {
        /// <summary>
        /// Value is converted from the min - max range into 0 - 100 percent range
        /// </summary>
        [Test]
        public void Progress_Should_ConvertValueRangeToPercent()
        {
            var comp = Context.RenderComponent<MudProgressLinear>(x =>
                {
                    x.Add(y => y.Min, -500);
                    x.Add(y => y.Max, 500);
                    x.Add(y => y.Value, -400);
                    x.Add(y => y.BufferValue, 400);
                });
            Console.WriteLine(comp.Markup);
            // checking range conversion
            comp.Instance.GetValuePercent().Should().Be(10);
            comp.Instance.GetBufferPercent().Should().Be(90);
            // checking cut-off at min and max
            comp.SetParam(x => x.Min, 0.0);
            comp.Instance.GetValuePercent().Should().Be(0);
            comp.SetParam(x => x.Min, -500.0);
            comp.SetParam(x => x.Max, 0.0);
            comp.Instance.GetBufferPercent().Should().Be(100);
            comp.SetParam(x => x.Min, 0.0);
            comp.SetParam(x => x.Max, 100.0);
            comp.SetParam(x => x.Value, 100.0);
            comp.Instance.GetValuePercent().Should().Be(100);
            comp.SetParam(x => x.Value, -2.0);
            comp.SetParam(x => x.Min, -7.0);
            comp.SetParam(x => x.Max, 7.0);
            comp.SetParam(x => x.Buffer, false);
            var percent = (-2 - (-7)) / 14.0 * 100;
            comp.Instance.GetValuePercent().Should().Be(percent);
            comp.Find("div.mud-progress-linear-bar").MarkupMatches(
                $"<div class=\"mud-progress-linear-bar mud-default mud-progress-linear-bar-1-determinate\" style=\"transform: translateX(-{Math.Round(100 - percent)}%);\"></div>");
        }
    }
}
