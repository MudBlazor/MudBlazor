using System.Diagnostics.CodeAnalysis;

namespace MudBlazor
{
    /// <summary>
    /// Represents an arbitrary SVG path.
    /// </summary>
    public class SvgPath : IEquatable<SvgPath>
    {
        /// <summary>
        /// The position of this path within a list.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// The SVG path to draw.
        /// </summary>
        public string? Data { get; set; }

        /// <summary>
        /// The label text for on hover.
        /// </summary>
        public string LabelXValue { get; set; } = string.Empty;

        /// <summary>
        /// The label text for on hover.
        /// </summary>
        public string LabelYValue { get; set; } = string.Empty;

        /// <summary>
        /// The label X position for on hover.
        /// </summary>
        public double LabelX { get; set; }

        /// <summary>
        /// The label Y position for on hover.
        /// </summary>
        public double LabelY { get; set; }

        ///<inheritdoc />
        public virtual bool Equals(SvgPath? other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Index == other.Index &&
                   string.Equals(Data, other.Data) &&
                   string.Equals(LabelXValue, other.LabelXValue) &&
                   string.Equals(LabelYValue, other.LabelYValue) &&
                   LabelX.Equals(other.LabelX) &&
                   LabelY.Equals(other.LabelY);
        }

        ///<inheritdoc />
        public override bool Equals(object? obj)
        {
            return Equals(obj as SvgPath);
        }

        ///<inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(Index, Data, LabelXValue, LabelYValue, LabelX, LabelY);
        }
    }
}
