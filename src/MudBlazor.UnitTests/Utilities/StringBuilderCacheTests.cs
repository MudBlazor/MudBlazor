using System.Text;
using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities;

[TestFixture]
public class StringBuilderCacheTests
{
    [SetUp]
    public void SetUp()
    {
        DrainCache();
    }

    [Test]
    public void Acquire_ReturnsReleasedBuilderAndClearsItsContents()
    {
        var builder = StringBuilderCache.Acquire();
        builder.Append("cached");
        StringBuilderCache.Release(builder);

        var reused = StringBuilderCache.Acquire();

        reused.Should().BeSameAs(builder);
        reused.Length.Should().Be(0);
    }

    [Test]
    public void Acquire_WithBiggerCapacityThanCachedBuilder_LeavesCachedBuilderAvailable()
    {
        var cached = StringBuilderCache.Acquire(16);
        cached.Append('x');
        StringBuilderCache.Release(cached);

        var larger = StringBuilderCache.Acquire(cached.Capacity + 1);
        var reused = StringBuilderCache.Acquire(1);

        larger.Should().NotBeSameAs(cached);
        larger.Capacity.Should().BeGreaterThanOrEqualTo(cached.Capacity + 1);
        reused.Should().BeSameAs(cached);
        reused.Length.Should().Be(0);
    }

    [Test]
    public void Release_DoesNotCacheBuildersLargerThanMaxSize()
    {
        var largeBuilder = new StringBuilder(361);
        StringBuilderCache.Release(largeBuilder);

        var acquired = StringBuilderCache.Acquire();

        acquired.Should().NotBeSameAs(largeBuilder);
    }

    [Test]
    public void GetStringAndRelease_ReturnsTextAndCachesBuilder()
    {
        var builder = StringBuilderCache.Acquire();
        builder.Append("mud");

        var result = StringBuilderCache.GetStringAndRelease(builder);
        var reused = StringBuilderCache.Acquire();

        result.Should().Be("mud");
        reused.Should().BeSameAs(builder);
        reused.Length.Should().Be(0);
    }

    private static void DrainCache()
    {
        _ = StringBuilderCache.Acquire();
    }
}
