using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services;

[TestFixture]
public class ScrollSpyTests
{
    private Mock<IJSRuntime> _runtimeMock;
    private ScrollSpy _service;

    [SetUp]
    public void SetUp()
    {
        _runtimeMock = new Mock<IJSRuntime>(MockBehavior.Strict);
        _service = new ScrollSpy(_runtimeMock.Object);
    }

    [Test]
    public async Task StartSpying_CallsJsWithSelectors()
    {
        SetupVoidInvocation("mudScrollSpy.spying", args =>
            args.Length == 3 &&
            (args[1] as string) == "#container" &&
            (args[2] as string) == "section-class");

        await _service.StartSpying("#container", "section-class");

        _runtimeMock.VerifyAll();
    }

    [Test]
    public async Task ScrollToSection_TrimsFragmentAndUpdatesCenteredSection()
    {
        SetupVoidInvocation("mudScrollSpy.scrollToSection", args =>
            args.Length == 1 &&
            (args[0] as string) == "details");

        await _service.ScrollToSection("#details");

        _service.CenteredSection.Should().Be("#details");
        _runtimeMock.VerifyAll();
    }

    [Test]
    public async Task ScrollToSection_WithUri_UsesFragment()
    {
        SetupVoidInvocation("mudScrollSpy.scrollToSection", args =>
            args.Length == 1 &&
            (args[0] as string) == "api");

        await _service.ScrollToSection(new Uri("https://mudblazor.test/docs#api"));

        _service.CenteredSection.Should().Be("#api");
        _runtimeMock.VerifyAll();
    }

    [Test]
    public async Task ScrollToSection_WithUriWithoutFragment_UsesEmptyId()
    {
        SetupVoidInvocation("mudScrollSpy.scrollToSection", args =>
            args.Length == 1 &&
            (args[0] as string) == string.Empty);

        await _service.ScrollToSection(new Uri("https://mudblazor.test/docs"));

        _service.CenteredSection.Should().Be(string.Empty);
        _runtimeMock.VerifyAll();
    }

    [Test]
    public async Task SetSectionAsActive_TrimsFragmentAndUpdatesCenteredSection()
    {
        SetupVoidInvocation("mudScrollSpy.activateSection", args =>
            args.Length == 1 &&
            (args[0] as string) == "examples");

        await _service.SetSectionAsActive("#examples");

        _service.CenteredSection.Should().Be("#examples");
        _runtimeMock.VerifyAll();
    }

    [Test]
    public void SectionChangeOccured_UpdatesCenteredSectionAndRaisesEvent()
    {
        ScrollSectionCenteredEventArgs capturedEventArgs = null;
        _service.ScrollSectionSectionCentered += (_, args) => capturedEventArgs = args;

        _service.SectionChangeOccured("usage");

        _service.CenteredSection.Should().Be("usage");
        capturedEventArgs.Should().NotBeNull();
        capturedEventArgs.Id.Should().Be("usage");
    }

    [Test]
    public async Task DisposeAsync_UnspiesOnlyOnce()
    {
        SetupVoidInvocation("mudScrollSpy.unspy", args => args.Length == 0);

        await _service.DisposeAsync();
        await _service.DisposeAsync();

        _runtimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudScrollSpy.unspy", It.IsAny<object[]>()), Times.Once);
    }

    [Test]
    public void ScrollSpyFactory_Create_ReturnsScrollSpy()
    {
        var services = new ServiceCollection();
        services.AddSingleton(_runtimeMock.Object);
        var factory = new ScrollSpyFactory(services.BuildServiceProvider());

        var scrollSpy = factory.Create();

        scrollSpy.Should().BeOfType<ScrollSpy>();
    }

    private void SetupVoidInvocation(string identifier, Func<object[], bool> argumentMatcher)
    {
        _runtimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>(identifier, It.Is<object[]>(args => argumentMatcher(args))))
            .ReturnsAsync(Mock.Of<IJSVoidResult>())
            .Verifiable();
    }
}
