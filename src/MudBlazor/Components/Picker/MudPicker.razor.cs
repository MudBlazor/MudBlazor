using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudPicker<T> : MudFormComponent<T, string>
    {
        protected IKeyInterceptor _keyInterceptor;

        public MudPicker() : base(new Converter<T, string>()) { }
        protected MudPicker(Converter<T, string> converter) : base(converter) { }

        [Inject]
        private IKeyInterceptorFactory KeyInterceptorFactory { get; set; }

        private string _elementId = "picker" + Guid.NewGuid().ToString().Substring(0, 8);

        protected string PickerClassname =>
            new CssBuilder("mud-picker")
                .AddClass("mud-picker-inline", PickerVariant != PickerVariant.Static)
                .AddClass("mud-picker-static", PickerVariant == PickerVariant.Static)
                .AddClass("mud-rounded", PickerVariant == PickerVariant.Static && !_pickerSquare)
                .AddClass($"mud-elevation-{_pickerElevation}", PickerVariant == PickerVariant.Static)
                .AddClass("mud-picker-input-button", !Editable && PickerVariant != PickerVariant.Static)
                .AddClass("mud-picker-input-text", Editable && PickerVariant != PickerVariant.Static)
                .AddClass("mud-disabled", GetDisabledState() && PickerVariant != PickerVariant.Static)
                .AddClass(Class)
                .Build();

        protected string PickerPaperClassname =>
            new CssBuilder("mud-picker")
                .AddClass("mud-picker-paper")
                .AddClass("mud-picker-view", PickerVariant == PickerVariant.Inline)
                .AddClass("mud-picker-open", Open && PickerVariant == PickerVariant.Inline)
                .AddClass("mud-picker-popover-paper", PickerVariant == PickerVariant.Inline)
                .AddClass("mud-dialog", PickerVariant == PickerVariant.Dialog)
                .Build();

        protected string PickerPaperStylename =>
            new StyleBuilder()
                .AddStyle("transition-duration", $"{Math.Round(MudGlobal.TransitionDefaults.Duration.TotalMilliseconds)}ms")
                .AddStyle("transition-delay", $"{Math.Round(MudGlobal.TransitionDefaults.Delay.TotalMilliseconds)}ms")
                .AddStyle(Style)
                .Build();

        protected string PickerInlineClassname =>
            new CssBuilder("mud-picker-inline-paper")
                .Build();

        protected string PickerContainerClassname =>
            new CssBuilder("mud-picker-container")
                .AddClass("mud-paper-square", _pickerSquare)
                .AddClass("mud-picker-container-landscape",
                    Orientation == Orientation.Landscape && PickerVariant == PickerVariant.Static)
                .Build();

        protected string PickerInputClassname =>
            new CssBuilder("mud-input-input-control")
                .AddClass(Class)
                .Build();

        protected string ActionsClassname =>
            new CssBuilder("mud-picker-actions")
                .AddClass(ActionsClass)
                .Build();

        /// <summary>
        /// The color of the adornment if used. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Color AdornmentColor { get; set; } = Color.Default;

        /// <summary>
        /// Sets the icon of the input text field
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string AdornmentIcon { get; set; } = Icons.Material.Filled.Event;

        /// <summary>
        /// Sets the aria-label of the input text field icon
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string AdornmentAriaLabel { get; set; } = string.Empty;

        /// <summary>
        /// The short hint displayed in the input before the user enters a value.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string Placeholder { get; set; }

        /// <summary>
        /// Fired when the dropdown / dialog opens
        /// </summary>
        [Parameter]
        public EventCallback PickerOpened { get; set; }

        /// <summary>
        /// Fired when the dropdown / dialog closes
        /// </summary>
        [Parameter]
        public EventCallback PickerClosed { get; set; }

        /// <summary>
        /// The higher the number, the heavier the drop-shadow. 0 for no shadow set to 8 by default in inline mode and 0 in static mode.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public int Elevation { set; get; } = 8;

        /// <summary>
        /// If true, border-radius is set to 0 this is set to true automatically in static mode but can be overridden with Rounded bool.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public bool Square { get; set; }

        /// <summary>
        /// If true, border-radius is set to theme default when in Static Mode.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public bool Rounded { get; set; }

        /// <summary>
        /// If string has value, HelperText will be applied.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string HelperText { get; set; }

        /// <summary>
        /// If true, the helper text will only be visible on focus.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool HelperTextOnFocus { get; set; }

        /// <summary>
        /// If string has value the label text will be displayed in the input, and scaled down at the top if the input has value.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string Label { get; set; }

        /// <summary>
        /// Show clear button.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Clearable { get; set; } = false;

        /// <summary>
        /// If true, the picker will be disabled.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Disabled { get; set; }

        [CascadingParameter(Name = "ParentDisabled")]
        private bool ParentDisabled { get; set; }
        protected bool GetDisabledState() => Disabled || ParentDisabled;

        /// <summary>
        /// Determines whether the input has an underline. Default is true
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool Underline { get; set; } = true;

        /// <summary>
        /// If true, no date or time can be defined.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool ReadOnly { get; set; }

        [CascadingParameter(Name = "ParentReadOnly")]
        private bool ParentReadOnly { get; set; }
        protected bool GetReadOnlyState() => ReadOnly || ParentReadOnly;

        /// <summary>
        /// If true, the picker will be editable.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Editable { get; set; } = false;

        /// <summary>
        /// If true, show toolbar. If false, show only date/time views.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public bool ShowToolbar { get; set; } = true;

        /// <summary>
        /// User class names for picker's Toolbar, separated by space
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public string ToolbarClass { get; set; }

        /// <summary>
        /// Picker container option
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public PickerVariant PickerVariant { get; set; } = PickerVariant.Inline;

        /// <summary>
        /// Variant of the text input
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        /// Sets if the icon will be att start or end, set to None to disable.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public Adornment Adornment { get; set; } = Adornment.End;

        /// <summary>
        /// What orientation to render in when in PickerVariant Static Mode.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public Orientation Orientation { get; set; } = Orientation.Portrait;

        /// <summary>
        /// Sets the Icon Size.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Size IconSize { get; set; } = Size.Medium;

        /// <summary>
        /// The color of the toolbar, selected and active. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// Fired when the text changes.
        /// </summary>
        [Parameter]
        public EventCallback<string> TextChanged { get; set; }

        /// <summary>
        /// If true and Editable is true, update Text immediately on typing.
        /// If false, Text is updated only on Enter or loss of focus.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool ImmediateText { get; set; }

        /// <summary>
        /// Fired when the text input is clicked.
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        /// <summary>
        /// The currently selected string value (two-way bindable)
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Data)]
        public string Text
        {
            get => _text;
            set => SetTextAsync(value, true).AndForget();
        }

        private string _text;

        /// <summary>
        /// CSS class that will be applied to the action buttons container
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public string ActionsClass { get; set; }

        /// <summary>
        /// Define the action buttons here
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerBehavior)]
        public RenderFragment<MudPicker<T>> PickerActions { get; set; }

        /// <summary>
        ///  Will adjust vertical spacing.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Margin Margin { get; set; } = Margin.None;

        /// <summary>
        /// A mask for structured input of the date (requires Editable to be true).
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public IMask Mask
        {
            get => _mask;
            set => _mask = value;
        }

        /// <summary>
        /// Gets or sets the origin of the popover's anchor. Defaults to <see cref="Origin.TopLeft"/>
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public Origin AnchorOrigin { get; set; } = Origin.TopLeft;

        /// <summary>
        /// Gets or sets the origin of the popover's transform. Defaults to <see cref="Origin.TopLeft"/>
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public Origin TransformOrigin { get; set; } = Origin.TopLeft;

        protected IMask _mask = null;

        protected async Task SetTextAsync(string value, bool callback)
        {
            if (_text != value)
            {
                _text = value;
                if (callback)
                    await StringValueChangedAsync(_text);
                await TextChanged.InvokeAsync(_text);
            }
        }

        /// <summary>
        /// Value change hook for descendants.
        /// </summary>
        protected virtual Task StringValueChangedAsync(string value)
        {
            return Task.CompletedTask;
        }

        protected bool Open { get; set; }

        public Task ToggleOpenAsync()
        {
            if (Open)
            {
                return CloseAsync();
            }
            else
            {
                return OpenAsync();
            }
        }

        public async Task CloseAsync(bool submit = true)
        {
            Open = false;

            if (submit)
            {
                await SubmitAsync();
            }

            await OnClosedAsync();
            StateHasChanged();
        }

        public Task OpenAsync()
        {
            Open = true;
            StateHasChanged();

            return OnOpenedAsync();
        }

        private Task CloseOverlayAsync() => CloseAsync(PickerActions == null);

        protected internal virtual Task SubmitAsync() => Task.CompletedTask;

        public virtual async Task ClearAsync(bool close = true)
        {
            if (close && PickerVariant != PickerVariant.Static)
            {
                await CloseAsync(false);
            }
        }

        protected override async Task ResetValueAsync()
        {
            if (_inputReference is not null)
            {
                await _inputReference.ResetAsync();
            }
            await base.ResetValueAsync();
        }

        protected internal MudTextField<string> _inputReference;

        public virtual ValueTask FocusAsync() => _inputReference?.FocusAsync() ?? ValueTask.CompletedTask;

        public virtual ValueTask BlurAsync() => _inputReference?.BlurAsync() ?? ValueTask.CompletedTask;

        public virtual ValueTask SelectAsync() => _inputReference?.SelectAsync() ?? ValueTask.CompletedTask;

        public virtual ValueTask SelectRangeAsync(int pos1, int pos2) =>
            _inputReference?.SelectRangeAsync(pos1, pos2) ?? ValueTask.CompletedTask;

        private bool _pickerSquare;
        private int _pickerElevation;
        private ElementReference _pickerInlineRef;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (PickerVariant == PickerVariant.Static)
            {
                Open = true;
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

            if (Label == null && For != null)
                Label = For.GetLabelString();
        }

        private async Task EnsureKeyInterceptorAsync()
        {
            if (_keyInterceptor == null)
            {
                _keyInterceptor = KeyInterceptorFactory.Create();

                await _keyInterceptor.Connect(_elementId, new KeyInterceptorOptions()
                {
                    //EnableLogging = true,
                    TargetClass = "mud-input-slot",
                    Keys =
                    {
                        new KeyOptions { Key = " ", PreventDown = "key+none" },
                        new KeyOptions { Key = "ArrowUp", PreventDown = "key+none" },
                        new KeyOptions { Key = "ArrowDown", PreventDown = "key+none" },
                        new KeyOptions { Key = "Enter", PreventDown = "key+none" },
                        new KeyOptions { Key = "NumpadEnter", PreventDown = "key+none" },
                        new KeyOptions { Key = "/./", SubscribeDown = true, SubscribeUp = true }, // for our users
                    },
                });
                _keyInterceptor.KeyDown += HandleKeyDown;
            }
        }

        private async Task OnClickAsync(MouseEventArgs args)
        {
            if (!Editable)
            {
                await ToggleStateAsync();
            }

            if (OnClick.HasDelegate)
            {
                await OnClick.InvokeAsync(args);
            }
        }

        /// <summary>
        /// 'HandleKeyDown' needed to be async in order to call other async methods. Because
        /// the HandleKeyDown is virtual and the base needs to be called from overriden methods
        /// we can't use 'async void'. This would break the synchronous behavior of those
        /// overriden methods. The KeyInterceptor does not support async behavior, so we have to
        /// add this hook method for handling the KeyDown event.
        /// This method can be removed when the KeyInterceptor supports async behavior.
        /// </summary>
        /// <param name="args"></param>
        private async void HandleKeyDown(KeyboardEventArgs args)
        {
            await OnHandleKeyDownAsync(args);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await EnsureKeyInterceptorAsync();
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        protected internal async Task ToggleStateAsync()
        {
            if (GetDisabledState() || GetReadOnlyState())
                return;
            if (Open)
            {
                Open = false;
                await OnClosedAsync();
            }
            else
            {
                Open = true;
                await OnOpenedAsync();
                await FocusAsync();
            }
        }

        protected virtual async Task OnOpenedAsync()
        {
            await OnPickerOpenedAsync();

            if (PickerVariant == PickerVariant.Inline)
            {
                await _pickerInlineRef.MudChangeCssAsync(PickerInlineClassname);
            }

            await EnsureKeyInterceptorAsync();
            await _keyInterceptor.UpdateKey(new() { Key = "Escape", StopDown = "key+none" });
        }

        protected virtual async Task OnClosedAsync()
        {
            await OnPickerClosedAsync();

            await EnsureKeyInterceptorAsync();
            await _keyInterceptor.UpdateKey(new() { Key = "Escape", StopDown = "none" });
        }

        protected virtual Task OnPickerOpenedAsync() => PickerOpened.InvokeAsync(this);

        protected virtual Task OnPickerClosedAsync() => PickerClosed.InvokeAsync(this);

        protected internal virtual async Task OnHandleKeyDownAsync(KeyboardEventArgs args)
        {
            if (GetDisabledState() || GetReadOnlyState())
                return;
            switch (args.Key)
            {
                case "Backspace":
                    if (args.CtrlKey && args.ShiftKey)
                    {
                        await ClearAsync();
                        _value = default;
                        await ResetAsync();
                    }

                    break;
                case "Escape":
                case "Tab":
                    await CloseAsync(false);
                    break;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (_keyInterceptor != null)
                {
                    _keyInterceptor.KeyDown -= HandleKeyDown;
                    if (IsJSRuntimeAvailable)
                    {
                        _keyInterceptor.Dispose();
                    }
                }
            }
        }
    }
}
