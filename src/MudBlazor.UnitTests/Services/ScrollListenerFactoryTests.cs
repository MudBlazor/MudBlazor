using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Moq;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services;

[TestFixture]
public class ScrollListenerFactoryTests
{
    [Test]
    public void Create_UsesDefaultReportRate()
    {
        var factory = CreateFactory();

        var listener = factory.Create("#main");

        listener.Should().BeOfType<ScrollListener>();
        listener.Selector.Should().Be("#main");
        listener.ReportRateMs.Should().Be(10);
    }

    [Test]
    public void Create_UsesExplicitReportRate()
    {
        var factory = CreateFactory();

        var listener = factory.Create("#main", 25);

        listener.Should().BeOfType<ScrollListener>();
        listener.Selector.Should().Be("#main");
        listener.ReportRateMs.Should().Be(25);
    }

    private static ScrollListenerFactory CreateFactory()
    {
        var services = new ServiceCollection();
        services.AddSingleton(Mock.Of<IJSRuntime>());
        return new ScrollListenerFactory(services.BuildServiceProvider());
    }
}
