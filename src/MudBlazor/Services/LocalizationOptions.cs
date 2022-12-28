namespace MudBlazor.Services
{

    /// <summary>
    /// Provides localization configuration used by MudBlazor components
    /// </summary>
    public class LocalizationOptions
    {
        /// <summary>
        /// Localize binding converters error messages
        /// </summary>
        public BindingConvertersLocalizationOptions BindingConverters { get; } = new BindingConvertersLocalizationOptions();

        public class BindingConvertersLocalizationOptions
        {
            /// <summary>
            /// Gets error messages used by binding converters
            /// </summary>
            /// <returns>Array of messages</returns>
            public string[] GetErrorMessages() => BindingConvertersErrorMessages.GetErrorMessages();

            /// <summary>
            /// Sets localized error messages to be used by binding converters
            /// </summary>
            /// <param name="messages">Localized messages array (same order as GetErrorMessages)</param>
            public void SetErrorMessages(string[] messages) => BindingConvertersErrorMessages.SetErrorMessages(messages);
        }
    }
}
