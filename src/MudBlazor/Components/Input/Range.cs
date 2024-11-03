namespace MudBlazor
{
    /// <summary>
    /// A range of values.
    /// </summary>
    /// <typeparam name="T">The type of value for the range.</typeparam>
    public class Range<T>
    {
        /// <summary>
        /// The minimum value.
        /// </summary>
        public T Start { get; set; }

        /// <summary>
        /// The maximum value.
        /// </summary>
        public T End { get; set; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public Range()
        {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="start">The minimum value.</param>
        /// <param name="end">The maximum value.</param>
        public Range(T start, T end)
        {
            Start = start;
            End = end;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is Range<T> r
                && null != r.Start
                && r.Start.Equals(Start)
                && null != r.End
                && r.End.Equals(End);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
