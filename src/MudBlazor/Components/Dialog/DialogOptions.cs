// Copyright (c) 2020 Jonny Larsson
// License: MIT
// See https://github.com/Garderoben/MudBlazor
// Modified version of Blazored Modal
// Copyright (c) 2019 Blazored
// License: MIT
// See https://github.com/Blazored


using System.ComponentModel;

namespace MudBlazor.Dialog
{
    public class DialogOptions
    {
        public DialogPosition? Position { get; set; }

        public MaxWidth? MaxWidth { get; set; }

        public bool? DisableBackdropClick { get; set; }
        public bool? NoHeader { get; set; }
        public bool? CloseButton { get; set; }
        public bool? FullScreen { get; set; }
        public bool? FullWidth { get; set; }
    }

    public enum DialogPosition
    {
        [Description("center")]
        Center,
        [Description("topleft")]
        TopLeft,
        [Description("topright")]
        TopRight,
        [Description("bottomleft")]
        BottomLeft,
        [Description("bottomright")]
        BottomRight,
        [Description("center")]
        Custom
    }
}
