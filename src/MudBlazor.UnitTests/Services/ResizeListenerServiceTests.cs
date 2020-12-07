using FluentAssertions;
using Moq;
using MudBlazor.Providers;
using MudBlazor.Services;
using NUnit.Framework;
using System.Threading.Tasks;

namespace MudBlazor.UnitTests.Services
{
    [TestFixture]
    public class ResizeListenerServiceTests
    {
        private Mock<IBrowserWindowSizeProvider> _browserWindowSizeProvider;
        private ResizeListenerService _service;

        [SetUp]
        public void SetUp()
        {
            _browserWindowSizeProvider = new Mock<IBrowserWindowSizeProvider>();
            _service = new ResizeListenerService(null, _browserWindowSizeProvider.Object);
        }

        // 0 - 599
        [TestCase(Breakpoint.Xs, 0, true)]
        [TestCase(Breakpoint.Xs, 599, true)]
        [TestCase(Breakpoint.Xs, 600, false)]

        // 600 - 959
        [TestCase(Breakpoint.Sm, 599, false)]
        [TestCase(Breakpoint.Sm, 600, true)]
        [TestCase(Breakpoint.Sm, 959, true)]
        [TestCase(Breakpoint.Sm, 960, false)]

        // 960 - 1279
        [TestCase(Breakpoint.Md, 959, false)]
        [TestCase(Breakpoint.Md, 960, true)]
        [TestCase(Breakpoint.Md, 1279, true)]
        [TestCase(Breakpoint.Md, 1280, false)]

        // 1280 - 1919
        [TestCase(Breakpoint.Lg, 1279, false)]
        [TestCase(Breakpoint.Lg, 1280, true)]
        [TestCase(Breakpoint.Lg, 1919, true)]
        [TestCase(Breakpoint.Lg, 1920, false)]

        // 1920 - *
        [TestCase(Breakpoint.Xl, 1919, false)]
        [TestCase(Breakpoint.Xl, 1920, true)]
        [TestCase(Breakpoint.Xl, 9999, true)]

        // >= 600
        [TestCase(Breakpoint.SmAndUp, 599, false)]
        [TestCase(Breakpoint.SmAndUp, 600, true)]
        [TestCase(Breakpoint.SmAndUp, 9999, true)]

        // >= 960
        [TestCase(Breakpoint.MdAndUp, 959, false)]
        [TestCase(Breakpoint.MdAndUp, 960, true)]
        [TestCase(Breakpoint.MdAndUp, 9999, true)]

        // >= 1280
        [TestCase(Breakpoint.LgAndUp, 1279, false)]
        [TestCase(Breakpoint.LgAndUp, 1280, true)]
        [TestCase(Breakpoint.LgAndUp, 9999, true)]

        // < 960
        [TestCase(Breakpoint.SmAndDown, 960, false)]
        [TestCase(Breakpoint.SmAndDown, 959, true)]
        [TestCase(Breakpoint.SmAndDown, 0, true)]

        // < 1280
        [TestCase(Breakpoint.MdAndDown, 1280, false)]
        [TestCase(Breakpoint.MdAndDown, 1279, true)]
        [TestCase(Breakpoint.MdAndDown, 0, true)]

        // < 1920
        [TestCase(Breakpoint.LgAndDown, 1920, false)]
        [TestCase(Breakpoint.LgAndDown, 1919, true)]
        [TestCase(Breakpoint.LgAndDown, 0, true)]
        public async Task IsMediaSizeReturnsCorrectValue(Breakpoint breakpoint, int browserWidth, bool expectedValue)
        {
            // Arrange
            _browserWindowSizeProvider
                .Setup(p => p.GetBrowserWindowSize())
                .ReturnsAsync(new BrowserWindowSize { Width = browserWidth });

            // Act
            var actual = await _service.IsMediaSize(breakpoint);

            // Assert
            actual.Should().Be(expectedValue);
        }
    }
}
