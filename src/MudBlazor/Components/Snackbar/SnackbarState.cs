// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace MudBlazor
{
    internal enum SnackbarState
    {
        Init,
        Showing,
        Hiding,
        Visible
    }

    internal static class SnackbarStateExtensions
    {
        public static bool IsShowing(this SnackbarState state) => state == SnackbarState.Showing;
        public static bool IsVisible(this SnackbarState state) => state == SnackbarState.Visible;
        public static bool IsHiding(this SnackbarState state) => state == SnackbarState.Hiding;
    }
}