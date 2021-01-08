using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudPicker : MudBasePicker
    {
        protected string PickerClass =>
            new CssBuilder("mud-picker")
            .AddClass($"mud-picker-inline", PickerVariant != PickerVariant.Static)
            .AddClass($"mud-picker-static", PickerVariant == PickerVariant.Static)
            .AddClass($"mud-rounded", PickerVariant == PickerVariant.Static && !PickerSquare)
            .AddClass($"mud-elevation-{PickerElevation.ToString()}", PickerVariant == PickerVariant.Static)
            .AddClass($"mud-picker-input-button", !AllowKeyboardInput && PickerVariant != PickerVariant.Static)
            .AddClass($"mud-picker-input-text", AllowKeyboardInput && PickerVariant != PickerVariant.Static)
            .AddClass($"mud-disabled", Disabled && PickerVariant != PickerVariant.Static)
            .AddClass(Class)
            .Build();

        protected string PickerPaperClass =>
        new CssBuilder("mud-picker-paper")
        .AddClass("mud-picker-view", PickerVariant == PickerVariant.Inline)
        .AddClass("mud-picker-open", IsOpen && PickerVariant == PickerVariant.Inline)
        .AddClass("mud-picker-popover-paper", PickerVariant == PickerVariant.Inline)
        .AddClass("mud-dialog", PickerVariant == PickerVariant.Dialog)
        .Build();

        protected string PickerContainerClass =>
        new CssBuilder("mud-picker-container")
        .AddClass("mud-paper-square", PickerSquare)
        .AddClass("mud-picker-container-landscape", Orientation == Orientation.Landscape && PickerVariant == PickerVariant.Static)
        .Build();

        [Parameter] public string InputIcon { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public EventCallback PickerOpened { get; set; }

        private bool PickerSquare { get; set; }
        private int PickerElevation { get; set; }

        protected override void OnInitialized()
        {
            if (PickerVariant == PickerVariant.Static)
            {
                IsOpen = true;
                if (Elevation == 8)
                {
                    PickerElevation = 0;
                }
                else
                {
                    PickerElevation = Elevation;
                }

                if (!Rounded)
                {
                    PickerSquare = true;
                }
            }
            else
            {
                PickerSquare = Square;
                PickerElevation = Elevation;
            }
        }

        public override void ToggleOpen()
        {
            base.ToggleOpen();
            if(IsOpen)
                OnPickerOpened();
            else
                StateHasChanged();
        }

        public override void Open()
        {
            base.Open();
            OnPickerOpened();
        }

        protected virtual void OnPickerOpened()
        {
            PickerOpened.InvokeAsync(this);
        }
    }
}