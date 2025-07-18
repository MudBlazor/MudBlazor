namespace MudBlazor
{
    /// <summary>
    /// The type of filter ComboBox uses. 
    /// </summary>
    public enum ComboBoxFilterType
    {
        /// <summary>
        /// Server will use <see cref="MudComboBox{T}.SearchFunc" />.
        /// </summary>
        Server = 0,
        /// <summary>
        /// Client will use Items and filter them based on the input.
        /// </summary>
        Client = 1,
    }
}
