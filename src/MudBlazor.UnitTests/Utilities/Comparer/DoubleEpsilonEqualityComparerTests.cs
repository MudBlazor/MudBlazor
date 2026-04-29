using AwesomeAssertions;

namespace MudBlazor.UnitTests.Utilities.Comparer;
public class DoubleEpsilonEqualityComparerTests
{
    [Test]
    public void Constructor_ShouldThrowArgumentOutOfRangeException()
    {
        // Act & Arrange
        Action<double> construct = epsilon => _ = new DoubleEpsilonEqualityComparer(epsilon);

        // Assert
        construct.Invoking(ctor => ctor(2)).Should().Throw<ArgumentOutOfRangeException>();
        construct.Invoking(ctor => ctor(-2)).Should().Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    [Arguments(1000000f, 1000001f, true)]
    [Arguments(1000001f, 1000000f, true)]
    [Arguments(10000f, 10001f, false)]
    [Arguments(10001f, 10000f, false)]
    public void Equals_Big(double a, double b, bool expected)
    {
        // Arrange
        var comparer = new DoubleEpsilonEqualityComparer(0.00001f);

        // Act
        var result = comparer.Equals(a, b);

        // Assert
        result.Should().Be(expected);
    }

    [Test]
    [Arguments(-1000000f, -1000001f, true)]
    [Arguments(-1000001f, -1000000f, true)]
    [Arguments(-10000f, -10001f, false)]
    [Arguments(-10001f, -10000f, false)]
    public void Equals_BigNeg(double a, double b, bool expected)
    {
        // Arrange
        var comparer = new DoubleEpsilonEqualityComparer(0.00001f);

        // Act
        var result = comparer.Equals(a, b);

        // Assert
        result.Should().Be(expected);
    }

    [Test]
    [Arguments(1.0000001f, 1.0000002f, true)]
    [Arguments(1.0000002f, 1.0000001f, true)]
    [Arguments(1.0002f, 1.0001f, false)]
    [Arguments(1.0001f, 1.0002f, false)]
    public void Equals_Mid(double a, double b, bool expected)
    {
        // Arrange
        var comparer = new DoubleEpsilonEqualityComparer(0.00001f);

        // Act
        var result = comparer.Equals(a, b);

        // Assert
        result.Should().Be(expected);
    }

    [Test]
    [Arguments(-1.000001f, -1.000002f, true)]
    [Arguments(-1.000002f, -1.000001f, true)]
    [Arguments(-1.0001f, -1.0002f, false)]
    [Arguments(-1.0002f, -1.0001f, false)]
    public void Equals_MidNeg(double a, double b, bool expected)
    {
        // Arrange
        var comparer = new DoubleEpsilonEqualityComparer(0.00001f);

        // Act
        var result = comparer.Equals(a, b);

        // Assert
        result.Should().Be(expected);
    }

    [Test]
    [Arguments(0.000000001000001f, 0.000000001000002f, true)]
    [Arguments(0.000000001000002f, 0.000000001000001f, true)]
    [Arguments(0.000000000001002f, 0.000000000001001f, false)]
    [Arguments(0.000000000001001f, 0.000000000001002f, false)]
    public void Equals_Small(double a, double b, bool expected)
    {
        // Arrange
        var comparer = new DoubleEpsilonEqualityComparer(0.00001f);

        // Act
        var result = comparer.Equals(a, b);

        // Assert
        result.Should().Be(expected);
    }

    [Test]
    [Arguments(-0.000000001000001f, -0.000000001000002f, true)]
    [Arguments(-0.000000001000002f, -0.000000001000001f, true)]
    [Arguments(-0.000000000001002f, -0.000000000001001f, false)]
    [Arguments(-0.000000000001001f, -0.000000000001002f, false)]
    public void Equals_SmallNeg(double a, double b, bool expected)
    {
        // Arrange
        var comparer = new DoubleEpsilonEqualityComparer(0.00001f);

        // Act
        var result = comparer.Equals(a, b);

        // Assert
        result.Should().Be(expected);
    }

    [Test]
    [Arguments(0.3f, 0.30000003f, true)]
    [Arguments(-0.3f, -0.30000003f, true)]
    public void Equals_SmallDiffs(double a, double b, bool expected)
    {
        // Arrange
        var comparer = new DoubleEpsilonEqualityComparer(0.00001f);

        // Act
        var result = comparer.Equals(a, b);

        // Assert
        result.Should().Be(expected);
    }

    [Test]
    [Arguments(0.0f, 0.0f, 0.00001f, true)]
    [Arguments(0.0f, -0.0f, 0.00001f, true)]
    [Arguments(-0.0f, -0.0f, 0.00001f, true)]
    [Arguments(0.00000001f, 0.0f, 0.00001f, false)]
    [Arguments(0.0f, 0.00000001f, 0.00001f, false)]
    [Arguments(-0.00000001f, 0.0f, 0.00001f, false)]
    [Arguments(0.0f, -0.00000001f, 0.00001f, false)]
    //[TestCase(0.0f, 1e-40f, 0.01f, true)]
    //[TestCase(1e-40f, 0.0f, 0.01f, true)]
    [Arguments(1e-40f, 0.0f, 0.000001f, false)]
    [Arguments(0.0f, 1e-40f, 0.000001f, false)]
    //[TestCase(0.0f, -1e-40f, 0.1f, true)]
    //[TestCase(-1e-40f, 0.0f, 0.1f, true)]
    [Arguments(-1e-40f, 0.0f, 0.00000001f, false)]
    [Arguments(0.0f, -1e-40f, 0.00000001f, false)]
    public void Equals_Zero(double a, double b, double epsilon, bool expected)
    {
        // Arrange
        var comparer = new DoubleEpsilonEqualityComparer(epsilon);

        // Act
        var result = comparer.Equals(a, b);

        // Assert
        result.Should().Be(expected);
    }

    [Test]
    [Arguments(double.MaxValue, double.MaxValue, true)]
    [Arguments(double.MaxValue, -double.MaxValue, false)]
    [Arguments(-double.MaxValue, double.MaxValue, false)]
    [Arguments(double.MaxValue, double.MaxValue / 2, false)]
    [Arguments(double.MaxValue, -double.MaxValue / 2, false)]
    [Arguments(-double.MaxValue, double.MaxValue / 2, false)]
    public void Equals_ExtremeMax(double a, double b, bool expected)
    {
        // Arrange
        var comparer = new DoubleEpsilonEqualityComparer(0.00001f);

        // Act
        var result = comparer.Equals(a, b);

        // Assert
        result.Should().Be(expected);
    }

    [Test]
    [Arguments(double.PositiveInfinity, double.PositiveInfinity, true)]
    [Arguments(double.NegativeInfinity, double.NegativeInfinity, true)]
    [Arguments(double.NegativeInfinity, double.PositiveInfinity, false)]
    [Arguments(double.PositiveInfinity, double.MaxValue, false)]
    [Arguments(double.NegativeInfinity, -double.MaxValue, false)]
    public void Equals_Infinities(double a, double b, bool expected)
    {
        // Arrange
        var comparer = new DoubleEpsilonEqualityComparer(0.00001f);

        // Act
        var result = comparer.Equals(a, b);

        // Assert
        result.Should().Be(expected);
    }

    [Test]
    [Arguments(double.NaN, double.NaN, true)]
    [Arguments(double.NaN, 0.0f, false)]
    [Arguments(double.NaN, -0.0f, false)]
    [Arguments(double.NaN, -0.0f, false)]
    [Arguments(0.0f, double.NaN, false)]
    [Arguments(double.NaN, double.PositiveInfinity, false)]
    [Arguments(double.PositiveInfinity, double.NaN, false)]
    [Arguments(double.NaN, double.NegativeInfinity, false)]
    [Arguments(double.NegativeInfinity, double.NaN, false)]
    [Arguments(double.NaN, double.MaxValue, false)]
    [Arguments(double.MaxValue, double.NaN, false)]
    [Arguments(double.NaN, -double.MaxValue, false)]
    [Arguments(-double.MaxValue, double.NaN, false)]
    [Arguments(double.NaN, double.MinValue, false)]
    [Arguments(double.MinValue, double.NaN, false)]
    [Arguments(double.NaN, -double.MinValue, false)]
    [Arguments(-double.MinValue, double.NaN, false)]
    public void Equals_Nan(double a, double b, bool expected)
    {
        // Arrange
        var comparer = new DoubleEpsilonEqualityComparer(0.00001f);

        // Act
        var result = comparer.Equals(a, b);

        // Assert
        result.Should().Be(expected);
    }

    [Test]
    [Arguments(1.000000001f, -1.0f, false)]
    [Arguments(-1.0f, 1.000000001f, false)]
    [Arguments(-1.000000001f, 1.0f, false)]
    [Arguments(1.0f, -1.000000001f, false)]
    public void Equals_Opposite(double a, double b, bool expected)
    {
        // Arrange
        var comparer = new DoubleEpsilonEqualityComparer(0.00001f);

        // Act
        var result = comparer.Equals(a, b);

        // Assert
        result.Should().Be(expected);
    }

    [Test]
    public void GetHashCode_Same()
    {
        // Arrange
        var comparer = DoubleEpsilonEqualityComparer.Default;
        var double1 = 1.1f;
        var double2 = 1.1f;

        // Act
        var getHasCode1 = comparer.GetHashCode(double1);
        var getHasCode2 = comparer.GetHashCode(double2);
        var result = getHasCode1 == getHasCode2;

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void GetHashCode_NotSame()
    {
        // Arrange
        var comparer = DoubleEpsilonEqualityComparer.Default;
        var double1 = 1.1f;
        var double2 = 1.2f;

        // Act
        var getHasCode1 = comparer.GetHashCode(double1);
        var getHasCode2 = comparer.GetHashCode(double2);
        var result = getHasCode1 == getHasCode2;

        // Assert
        result.Should().BeFalse();
    }
}