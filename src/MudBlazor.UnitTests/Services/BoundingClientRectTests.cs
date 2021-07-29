using FluentAssertions;
using MudBlazor.Interop;
using NUnit.Framework;
namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class BoundingClientRectTests
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
        /// In this example, there is a mouseover event conditionally attached
        /// if the property Attached is set to true is attached
        /// if not, there shouldn't have any event present 
        /// </summary>
        [Test]
        public void Detect_Boundaries()
        {
            var client = new BoundingClientRect();
            client.Top = -10;
            client.Left = -10;

            client.Height = 100;
            client.Width = 100;

            client.ScrollX = 100;
            client.ScrollY = 100;

            client.WindowHeight = 1000;
            client.WindowWidth = 1000;

            client.IsOutsideBottom.Should().BeFalse();
            client.IsOutsideTop.Should().BeTrue();
            client.IsOutsideLeft.Should().BeTrue();
            client.IsOutsideRight.Should().BeFalse();



        }
    }
}
