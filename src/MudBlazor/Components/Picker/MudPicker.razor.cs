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
            .AddClass($"mud-rounded", PickerVariant == PickerVariant.Static && !_pickerSquare)
            .AddClass($"mud-elevation-{_pickerElevation}", PickerVariant == PickerVariant.Static)
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
            .AddClass("mud-picker-hidden", _pickerVerticalPosition == PickerVerticalPosition.Unknown && PickerVariant == PickerVariant.Inline)
            .AddClass("mud-picker-pos-top", _pickerVerticalPosition == PickerVerticalPosition.Top)
            .AddClass("mud-picker-pos-above", _pickerVerticalPosition == PickerVerticalPosition.Above)
            .AddClass("mud-picker-pos-bottom", _pickerVerticalPosition == PickerVerticalPosition.Bottom)
            .AddClass("mud-picker-pos-below", _pickerVerticalPosition == PickerVerticalPosition.Below)
            .AddClass("mud-picker-pos-left", _pickerHorizontalPosition == PickerHorizontalPosition.Left)
            .AddClass("mud-picker-pos-right", _pickerHorizontalPosition == PickerHorizontalPosition.Right)
            .Build();

        protected string PickerContainerClass =>
        new CssBuilder("mud-picker-container")
        .AddClass("mud-paper-square", _pickerSquare)
        .AddClass("mud-picker-container-landscape", Orientation == Orientation.Landscape && PickerVariant == PickerVariant.Static)
        .Build();

        protected string PickerInputClass =>
        new CssBuilder("mud-input-input-control").AddClass(Class)
        .Build();

        [Parameter] public bool IsRange { get; set; } = false;
        [Parameter] public string InputIcon { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public EventCallback PickerOpened { get; set; }

        private bool _pickerSquare;
        private int _pickerElevation;
        private bool _isRendered = false;
        private ElementReference _pickerContainerRef;

        private PickerVerticalPosition _pickerVerticalPosition = PickerVerticalPosition.Unknown;
        private PickerHorizontalPosition _pickerHorizontalPosition = PickerHorizontalPosition.Unknown;

        protected override void OnInitialized()
        {
            if (PickerVariant == PickerVariant.Static)
            {
                IsOpen = true;

                if (Elevation == 8)
                {
                    _pickerElevation = 0;
                }
                else
                {
                    _pickerElevation = Elevation;
                }

                if (!Rounded)
                {
                    _pickerSquare = true;
                }
            }
            else
            {
                _pickerSquare = Square;
                _pickerElevation = Elevation;
            }
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
                if (!_isRendered && IsOpen)
                {
                    _isRendered = true;
                    await DeterminePosition();
                    StateHasChanged();
                }
                else if (_isRendered && !IsOpen)
                {
                    _isRendered = false;
                    _pickerVerticalPosition = PickerVerticalPosition.Unknown;
                }
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task DeterminePosition()
        {
            if (WindowSizeListener == null || JSRuntime == null)
            {
                _pickerVerticalPosition = PickerVerticalPosition.Below;
                return;
            }

            var size = await WindowSizeListener.GetBrowserWindowSize();
            var clientRect = await JSRuntime.InvokeAsync<BoundingClientRect>("getMudBoundingClientRect", _pickerContainerRef);

            if (size == null || clientRect == null)
            {
                _pickerVerticalPosition = PickerVerticalPosition.Below;
                return;
            }

            if (size.Width < clientRect.Right)
            {
                _pickerHorizontalPosition = size.Width > clientRect.Width ?
                    PickerHorizontalPosition.Right : PickerHorizontalPosition.Left;
            }

            if (size.Height < clientRect.Height)
            {
                _pickerVerticalPosition = PickerVerticalPosition.Top;
            }
            else if (size.Height < clientRect.Bottom)
            {
                if (clientRect.Top > clientRect.Height)
                {
                    _pickerVerticalPosition = PickerVerticalPosition.Above;
                }
                else if (clientRect.Top > size.Height / 2)
                {
                    _pickerVerticalPosition = PickerVerticalPosition.Bottom;
                }
                else
                {
                    _pickerVerticalPosition = PickerVerticalPosition.Top;
                }
            }
            else if (clientRect.Top < 0)
            {
                _pickerVerticalPosition = PickerVerticalPosition.Top;
            }
            else
            {
                _pickerVerticalPosition = PickerVerticalPosition.Below;
            }
        }
    }
}
