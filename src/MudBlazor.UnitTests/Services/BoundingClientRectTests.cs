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

        [Test]
        public void BoundingClientRectProperties_ShouldBeSetAndComputedCorrectly()
        {
            // Arrange
            var rect = new BoundingClientRect
            {
                Top = 10,
                Left = 20,
                Width = 100,
                Height = 200,
                WindowHeight = 1080,
                WindowWidth = 1920,
                ScrollX = 5,
                ScrollY = 10
            };

            // Act
            var clone = rect.Clone();

            // Assert
            clone.Top.Should().Be(rect.Top);
            clone.Left.Should().Be(rect.Left);
            clone.Width.Should().Be(rect.Width);
            clone.Height.Should().Be(rect.Height);
            clone.WindowHeight.Should().Be(rect.WindowHeight);
            clone.WindowWidth.Should().Be(rect.WindowWidth);
            clone.ScrollX.Should().Be(rect.ScrollX);
            clone.ScrollY.Should().Be(rect.ScrollY);

            // Check computed properties
            clone.X.Should().Be(rect.Left);
            clone.Y.Should().Be(rect.Top);
            clone.Bottom.Should().Be(rect.Top + rect.Height);
            clone.Right.Should().Be(rect.Left + rect.Width);
            clone.AbsoluteLeft.Should().Be(rect.Left + rect.ScrollX);
            clone.AbsoluteTop.Should().Be(rect.Top + rect.ScrollY);
            clone.AbsoluteRight.Should().Be(rect.Right + rect.ScrollX);
            clone.AbsoluteBottom.Should().Be(rect.Bottom + rect.ScrollY);

            // Check if the rect is outside of the viewport
            clone.IsOutsideBottom.Should().BeFalse();
            clone.IsOutsideLeft.Should().BeFalse();
            clone.IsOutsideTop.Should().BeFalse();
            clone.IsOutsideRight.Should().BeFalse();
        }

        [Test]
        public void BoundingClientRectIsEqualTo_ShouldReturnTrueForEqualRects()
        {
            // Arrange
            var rect1 = new BoundingClientRect { Top = 10, Left = 20, Width = 100, Height = 200 };
            var rect2 = new BoundingClientRect { Top = 10, Left = 20, Width = 100, Height = 200 };

            // Act & Assert
            rect1.IsEqualTo(rect2).Should().BeTrue();
        }

        [Test]
        public void BoundingClientRectIsEqualTo_ShouldReturnFalseForDifferentRects()
        {
            // Arrange
            var rect1 = new BoundingClientRect { Top = 10, Left = 20, Width = 100, Height = 200 };
            var rect2 = new BoundingClientRect { Top = 10, Left = 30, Width = 100, Height = 200 };

            // Act & Assert
            rect1.IsEqualTo(rect2).Should().BeFalse();
        }

        [Test]
        public void BoundingClientRectIsEqualTo_ShouldReturnFalseWhenEitherIsNull()
        {
            // Arrange
            var rect = new BoundingClientRect { Top = 10, Left = 20, Width = 100, Height = 200 };

            // Act & Assert
            rect.IsEqualTo(null).Should().BeFalse();
            ((BoundingClientRect)null).IsEqualTo(rect).Should().BeFalse();
        }
    }
}
