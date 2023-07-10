using System.Globalization;
using System.Threading;
using FluentAssertions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MudBlazor.Resources;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services.Localization
{
    [TestFixture]
    public class InternalMudLocalizerTests
    {
        private Mock<MudLocalizer> _mudLocalizer;
        private InternalMudLocalizer _internalMudLocalizer;
        private InternalMudLocalizer _internalMudLocalizerWithoutMudLocalizer;

        [SetUp]
        public void SetUp()
        {
            _mudLocalizer = new Mock<MudLocalizer> { CallBase = true};
            _mudLocalizer.Setup(s => s["MudDataGrid.is empty"]).Returns(new LocalizedString("MudDataGrid.is empty", "XXX", false));
            _mudLocalizer.Setup(s => s["MudDataGrid.is not empty"]).Returns(new LocalizedString("MudDataGrid.is not empty", "MudDataGrid.is not empty", true));

            _internalMudLocalizer = new InternalMudLocalizer(NullLoggerFactory.Instance, _mudLocalizer.Object);
            _internalMudLocalizerWithoutMudLocalizer = new InternalMudLocalizer(NullLoggerFactory.Instance);
        }

        [Test]
        public void TestWithCustomTranslation()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            _internalMudLocalizer["MudDataGrid.contains"].Should().BeOfType(typeof(LocalizedString)).And.BeEquivalentTo(new LocalizedString("MudDataGrid.contains", "contains", false, typeof(LanguageResource).FullName));
            _internalMudLocalizer["MudDataGrid.is empty"].Should().BeOfType(typeof(LocalizedString)).And.BeEquivalentTo(new LocalizedString("MudDataGrid.is empty", "is empty", false, typeof(LanguageResource).FullName));
            _internalMudLocalizer["MudDataGrid.is not empty"].Should().BeOfType(typeof(LocalizedString)).And.BeEquivalentTo(new LocalizedString("MudDataGrid.is not empty", "is not empty", false, typeof(LanguageResource).FullName));

            Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-DE");

            _internalMudLocalizer["MudDataGrid.contains"].Should().BeOfType(typeof(LocalizedString)).And.BeEquivalentTo(new LocalizedString("MudDataGrid.contains", "contains", false, typeof(LanguageResource).FullName));
            _internalMudLocalizer["MudDataGrid.is empty"].Should().BeOfType(typeof(LocalizedString)).And.BeEquivalentTo(new LocalizedString("MudDataGrid.is empty", "XXX", false));
            _internalMudLocalizer["MudDataGrid.is not empty"].Should().BeOfType(typeof(LocalizedString)).And.BeEquivalentTo(new LocalizedString("MudDataGrid.is not empty", "is not empty", false, typeof(LanguageResource).FullName));
        }

        [Test]
        public void TestWithoutCustomTranslation()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            _internalMudLocalizerWithoutMudLocalizer["MudDataGrid.contains"].Should().BeOfType(typeof(LocalizedString)).And.BeEquivalentTo(new LocalizedString("MudDataGrid.contains", "contains", false, typeof(LanguageResource).FullName));
            _internalMudLocalizerWithoutMudLocalizer["MudDataGrid.is empty"].Should().BeOfType(typeof(LocalizedString)).And.BeEquivalentTo(new LocalizedString("MudDataGrid.is empty", "is empty", false, typeof(LanguageResource).FullName));
            _internalMudLocalizerWithoutMudLocalizer["MudDataGrid.is not empty"].Should().BeOfType(typeof(LocalizedString)).And.BeEquivalentTo(new LocalizedString("MudDataGrid.is not empty", "is not empty", false, typeof(LanguageResource).FullName));

            Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-DE");

            _internalMudLocalizerWithoutMudLocalizer["MudDataGrid.contains"].Should().BeOfType(typeof(LocalizedString)).And.BeEquivalentTo(new LocalizedString("MudDataGrid.contains", "contains", false, typeof(LanguageResource).FullName));
            _internalMudLocalizerWithoutMudLocalizer["MudDataGrid.is empty"].Should().BeOfType(typeof(LocalizedString)).And.BeEquivalentTo(new LocalizedString("MudDataGrid.is empty", "is empty", false, typeof(LanguageResource).FullName));
            _internalMudLocalizerWithoutMudLocalizer["MudDataGrid.is not empty"].Should().BeOfType(typeof(LocalizedString)).And.BeEquivalentTo(new LocalizedString("MudDataGrid.is not empty", "is not empty", false, typeof(LanguageResource).FullName));
        }
    }
}
