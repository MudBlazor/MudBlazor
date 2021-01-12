using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Extensions;
using MudBlazor.Interop;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudPicker : MudBasePicker
    {
        enum PickerVerticalPosition
        {
            Unknown,
            Below,
            Above,
            Top,
            Bottom
        }

        enum PickerHorizontalPosition
        {
            Unknown,
            Left,
            Right
        }

        [Inject]
        private IBrowserWindowSizeProvider WindowSizeListener { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        protected string PickerClass =>
        new CssBuilder("mud-picker")
            .AddClass($"mud-picker-inline", PickerVariant != PickerVariant.Static)
            .AddClass($"mud-picker-static", PickerVariant == PickerVariant.Static)
            .AddClass($"mud-rounded", PickerVariant == PickerVariant.Static && !PickerSquare)
            .AddClass($"mud-elevation-{PickerElevation}", PickerVariant == PickerVariant.Static)
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
            .AddClass("mud-picker-hidden", pickerVerticalPosition == PickerVerticalPosition.Unknown && PickerVariant == PickerVariant.Inline)
            .AddClass("mud-picker-pos-top", pickerVerticalPosition == PickerVerticalPosition.Top)
            .AddClass("mud-picker-pos-above", pickerVerticalPosition == PickerVerticalPosition.Above)
            .AddClass("mud-picker-pos-bottom", pickerVerticalPosition == PickerVerticalPosition.Bottom)
            .AddClass("mud-picker-pos-below", pickerVerticalPosition == PickerVerticalPosition.Below)
            .AddClass("mud-picker-pos-left", pickerHorizontalPosition == PickerHorizontalPosition.Left)
            .AddClass("mud-picker-pos-right", pickerHorizontalPosition == PickerHorizontalPosition.Right)
            .Build();

        protected string PickerContainerClass =>
        new CssBuilder("mud-picker-container")
        .AddClass("mud-paper-square", PickerSquare)
        .AddClass("mud-picker-container-landscape", Orientation == Orientation.Landscape && PickerVariant == PickerVariant.Static)
        .Build();
        
        protected string PickerInputClass =>
        new CssBuilder("mud-input-input-control").AddClass(Class)
        .Build();

        [Parameter] public bool IsRange { get; set; } = false;
        [Parameter] public string InputIcon { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public EventCallback PickerOpened { get; set; }

        private bool PickerSquare { get; set; }
        private int PickerElevation { get; set; }
        private bool isRendered = false;
        private ElementReference PickerContainerRef { get; set; }

        private PickerVerticalPosition pickerVerticalPosition = PickerVerticalPosition.Unknown;
        private PickerHorizontalPosition pickerHorizontalPosition = PickerHorizontalPosition.Unknown;

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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (PickerVariant == PickerVariant.Inline)
            {
                if (!isRendered && IsOpen)
                {
                    isRendered = true;
                    await DeterminePosition();
                    StateHasChanged();
                }
                else if (isRendered && !IsOpen)
                {
                    isRendered = false;
                    pickerVerticalPosition = PickerVerticalPosition.Unknown;
                }
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task DeterminePosition()
        {
            if (WindowSizeListener == null || JSRuntime == null)
            {
                pickerVerticalPosition = PickerVerticalPosition.Below;
                return;
            }

            var size = await WindowSizeListener.GetBrowserWindowSize();
            var clientRect = await JSRuntime.InvokeAsync<BoundingClientRect>("getMudBoundingClientRect", PickerContainerRef);

            if (size == null || clientRect == null)
            {
                pickerVerticalPosition = PickerVerticalPosition.Below;
                return;
            }

            if (size.Width < clientRect.Right)
            {
                pickerHorizontalPosition = size.Width > clientRect.Width ?
                    PickerHorizontalPosition.Right : PickerHorizontalPosition.Left;
            }

            if (size.Height < clientRect.Height)
            {
                pickerVerticalPosition = PickerVerticalPosition.Top;
            }
            else if (size.Height < clientRect.Bottom)
            {
                if (clientRect.Top > clientRect.Height)
                {
                    pickerVerticalPosition = PickerVerticalPosition.Above;
                }
                else if (clientRect.Top > size.Height / 2)
                {
                    pickerVerticalPosition = PickerVerticalPosition.Bottom;
                }
                else
                {
                    pickerVerticalPosition = PickerVerticalPosition.Top;
                }
            }
            else if (clientRect.Top < 0)
            {
                pickerVerticalPosition = PickerVerticalPosition.Top;
            }
            else
            {
                pickerVerticalPosition = PickerVerticalPosition.Below;
            }
        }
    }
}