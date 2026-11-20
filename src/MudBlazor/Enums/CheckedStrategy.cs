namespace MudBlazor
{
    /// <summary>
    /// Which checked items of a nested <see cref="MudSelect{T}"/> are reported in its selected values.
    /// </summary>
    /// <remarks>
    /// <see cref="Child"/> and <see cref="Parent"/> also make checking a parent check its enabled descendants.
    /// </remarks>
    /// <seealso cref="MudSelect{T}.CheckedStrategy"/>
    public enum CheckedStrategy
    {
        /// <summary>
        /// Every checked item is reported, and items are checked independently of each other.
        /// </summary>
        All,

        /// <summary>
        /// Only items without nested items are reported.
        /// Checking a parent checks its descendants.
        /// </summary>
        Child,

        /// <summary>
        /// Only the topmost parents whose enabled descendants are all checked are reported, along with checked items that have no such parent.
        /// Checking a parent checks its descendants.
        /// </summary>
        Parent
    }
}
