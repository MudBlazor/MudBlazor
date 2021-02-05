#pragma warning disable CS1998 // async without await
#pragma warning disable IDE1006 // leading underscore

using Bunit;
using FluentAssertions;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;
namespace MudBlazor.UnitTests
{
    [TestFixture]
    public class ProgressTests
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
        /// Value is converted from the min - max range into 0 - 100 percent range
        /// </summary>
        [Test]
        public void Progress_Should_ConvertValueRangeToPercent()
        {
            var comp = ctx.RenderComponent<MudProgressLinear>(x =>
                {
                    x.Add(y => y.Min, -500);
                    x.Add(y => y.Max, 500);
                    x.Add(y => y.Value, -400);
                    x.Add(y => y.BufferValue, 400);
                });
            // checking range conversion
            comp.Instance.GetValuePercent().Should().Be(10);
            comp.Instance.GetBufferPercent().Should().Be(90);
            // checking cut-off at min and max
            comp.SetParam(x=>x.Min, 0.0);
            comp.Instance.GetValuePercent().Should().Be(0);
            comp.SetParam(x => x.Min, -500.0);
            comp.SetParam(x => x.Max, 0.0);
            comp.Instance.GetBufferPercent().Should().Be(100);
        }
    }
}
