namespace MudBlazor
{
    /// <summary>
    /// The type of filter AutoComplete uses. 
    /// Default is Client.
    /// </summary>
    public enum ComboBoxFilterType
    {
        /// <summary>
        /// Server will use AutoCompleteItems regardless of what's in it and allow client to do the filtering.
        /// </summary>
        Server = 0,
        /// <summary>
        /// Client will use AutoCompleteItems and filter them based on the input.
        /// </summary>
        Client = 1,
    }
}
