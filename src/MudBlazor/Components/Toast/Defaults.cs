// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace MudBlazor
{
    public static class Defaults
    {
        public static class Classes
        {
            public const string Snackbar = "mud-toaster";
            public const string SnackbarTitle = "mud-toast-title";
            public const string SnackbarMessage = "mud-toast-message";
            public const string CloseIconClass = "mud-toast-close-button";
            public const string ProgressBarClass = "mud-toast-progress";

            public static class TextPosition
            {
                public const string Left = "text-left";
                public const string Center = "text-center";
                public const string Right = "text-right";
            }

            public static class Position
            {
                public const string TopCenter = "mud-toast-top-center";
                public const string BottomCenter = "mud-toast-bottom-center";
                public const string TopFullWidth = "mud-toast-top-full-width";
                public const string BottomFullWidth = "mud-toast-bottom-full-width";
                public const string TopLeft = "mud-toast-top-left";
                public const string TopRight = "mud-toast-top-right";
                public const string BottomRight = "mud-toast-bottom-right";
                public const string BottomLeft = "mud-toast-bottom-left";
            }

            public static class Icons
            {
                public const string Info = "mud-toast-info";
                public const string Success = "mud-toast-success";
                public const string Warning = "mud-toast-warning";
                public const string Error = "mud-toast-error";
            }
        }
    }
}