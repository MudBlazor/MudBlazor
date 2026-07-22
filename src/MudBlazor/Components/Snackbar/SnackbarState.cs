// Copyright (c) A//Copyright(c) Alessandro Ghidini.All rights reserved.
//Changes and improvements Copyright (c) The MudBlazor Team.

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
