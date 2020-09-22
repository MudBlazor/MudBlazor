// Copyright (c) Alessandro Ghidini. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

namespace MudBlazor
{
    internal enum ToastState
    {
        Init,
        Showing,
        Hiding,
        Visible,
        MouseOver
    }

    internal static class ToastStateExtensions
    {
        public static bool IsShowing(this ToastState state) => state == ToastState.Showing;
        public static bool IsVisible(this ToastState state) => state == ToastState.Visible;
        public static bool IsHiding(this ToastState state) => state == ToastState.Hiding;
    }
}