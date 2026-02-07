namespace MudBlazor;

/// <summary>
/// A range of values.
/// </summary>
/// <typeparam name="T">The type of value for the range.</typeparam>
public class Range<T> : IEquatable<Range<T>?>
{
    /// <summary>
    /// The minimum value.
    /// </summary>
    public T? Start { get; }

    /// <summary>
    /// The maximum value.
    /// </summary>
    public T? End { get; }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public Range() : this(default, default)
    {
    }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="start">The minimum value.</param>
    /// <param name="end">The maximum value.</param>
    public Range(T? start, T? end)
    {
        Start = start;
        End = end;
    }

    /// <inheritdoc />
    public bool Equals(Range<T>? other) => other is not null && other.Start is not null && other.End is not null && other.Start.Equals(Start) && other.End.Equals(End);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Range<T> range && Equals(range);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Start, End);
}
