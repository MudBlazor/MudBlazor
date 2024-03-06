using System.Globalization;
using System.Threading;
using FluentAssertions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MudBlazor.Resources;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services.Localization;

[TestFixture]
public class InternalMudLocalizerTests
{
    private InternalMudLocalizer _internalMudLocalizer;
    private InternalMudLocalizer _internalMudLocalizerWithoutMudLocalizer;
    private InternalMudLocalizer _internalMudLocalizerWithCustomInterceptor;

    [SetUp]
    public void SetUp()
    {
        var mudLocalizer = new Mock<MudLocalizer> { CallBase = true };
        mudLocalizer.Setup(s => s["MudDataGrid.is empty"]).Returns(new LocalizedString("MudDataGrid.is empty", "XXX", false));
        mudLocalizer.Setup(s => s["MudDataGrid.is not empty"]).Returns(new LocalizedString("MudDataGrid.is not empty", "MudDataGrid.is not empty", true));


        _internalMudLocalizer = new InternalMudLocalizer(new DefaultLocalizationInterceptor(NullLoggerFactory.Instance, mudLocalizer.Object));
        _internalMudLocalizerWithoutMudLocalizer = new InternalMudLocalizer(new DefaultLocalizationInterceptor(NullLoggerFactory.Instance));

        var interceptor = new Mock<ILocalizationInterceptor>();
        interceptor.Setup(s => s.Handle("MudDataGrid.Clear")).Returns(new LocalizedString("MudDataGrid.Clear", "Reset", false));

        _internalMudLocalizerWithCustomInterceptor = new InternalMudLocalizer(interceptor.Object);
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

    [Test]
    public void TestWithCustomLocalizationInterceptor()
    {
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
        _internalMudLocalizerWithCustomInterceptor["MudDataGrid.Clear"].Should().BeOfType(typeof(LocalizedString)).And.BeEquivalentTo(new LocalizedString("MudDataGrid.Clear", "Reset", false));

        Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-DE");
        _internalMudLocalizerWithCustomInterceptor["MudDataGrid.Clear"].Should().BeOfType(typeof(LocalizedString)).And.BeEquivalentTo(new LocalizedString("MudDataGrid.Clear", "Reset", false));
    }
}
