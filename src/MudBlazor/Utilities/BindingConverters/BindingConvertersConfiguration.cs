namespace MudBlazor
{

    internal enum BindingConvertersErrorMessageEnum
    {
        NOT_A_VALID_BOOL,
        NOT_A_VALID_NUMBER,
        NOT_A_VALID_GUID,
        NOT_A_VALID_ENUM,
        NOT_A_VALID_DATETIME,
        NOT_A_VALID_TIMESPAN,
        CONVERSION_ERROR,
        CONVERSION_NOT_IMPLEMENTED,
        CONVERSION_FAILED,
        CONVERSION_TO_BOOL_FAILED
    }

    internal static class BindingConvertersErrorMessages
    {
        private static string[] ErrorMessages = {
            "Not a valid boolean",
            "Not a valid number",
            "Not a valid GUID",
            "Not a value of {0}",
            "Not a valid date time",
            "Not a valid time span",
            "Conversion error",
            "Conversion to type {0} not implemented",
            "Conversion from {0} to {1} failed",
            "Unable to convert to bool? from type {0}"
        };

        internal static string GetErrorMessage(BindingConvertersErrorMessageEnum bindingConvertersErrorMessage)
        {
            return ErrorMessages[(int)bindingConvertersErrorMessage];
        }

        public static string[] GetErrorMessages()
        {
            return ErrorMessages;
        }

        public static void SetErrorMessages(string[] messages)
        {
            if (messages == null)
                return;
            for (var i = 0; i < messages.Length; i++)
            {
                if (i > ErrorMessages.Length)
                    break;
                if (messages[i] != null)
                    ErrorMessages[i] = messages[i];
            }
        }
    }

}
