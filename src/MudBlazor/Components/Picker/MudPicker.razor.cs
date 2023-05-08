using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interfaces;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudPicker<T> : MudFormComponent<T, string>
    {
        protected IKeyInterceptor _keyInterceptor;

        public MudPicker() : base(new Converter<T, string>()) { }
        protected MudPicker(Converter<T, string> converter) : base(converter) { }

        [Inject] private IKeyInterceptorFactory _keyInterceptorFactory { get; set; }

        private string _elementId = "picker" + Guid.NewGuid().ToString().Substring(0, 8);

        [Inject] private IBrowserWindowSizeProvider WindowSizeListener { get; set; }

        protected string PickerClass =>
            new CssBuilder("mud-picker")
                .AddClass($"mud-picker-inline", PickerVariant != PickerVariant.Static)
                .AddClass($"mud-picker-static", PickerVariant == PickerVariant.Static)
                .AddClass($"mud-rounded", PickerVariant == PickerVariant.Static && !_pickerSquare)
                .AddClass($"mud-elevation-{_pickerElevation}", PickerVariant == PickerVariant.Static)
                .AddClass($"mud-picker-input-button", !Editable && PickerVariant != PickerVariant.Static)
                .AddClass($"mud-picker-input-text", Editable && PickerVariant != PickerVariant.Static)
                .AddClass($"mud-disabled", GetDisabledState() && PickerVariant != PickerVariant.Static)
                .AddClass(Class)
                .Build();

        protected string PickerPaperClass =>
            new CssBuilder("mud-picker")
                .AddClass("mud-picker-paper")
                .AddClass("mud-picker-view", PickerVariant == PickerVariant.Inline)
                .AddClass("mud-picker-open", IsOpen && PickerVariant == PickerVariant.Inline)
                .AddClass("mud-picker-popover-paper", PickerVariant == PickerVariant.Inline)
                .AddClass("mud-dialog", PickerVariant == PickerVariant.Dialog)
                .Build();

        protected string PickerInlineClass =>
            new CssBuilder("mud-picker-inline-paper")
                .Build();

        protected string PickerContainerClass =>
            new CssBuilder("mud-picker-container")
                .AddClass("mud-paper-square", _pickerSquare)
                .AddClass("mud-picker-container-landscape",
                    Orientation == Orientation.Landscape && PickerVariant == PickerVariant.Static)
                .Build();

        protected string PickerInputClass =>
            new CssBuilder("mud-input-input-control").AddClass(Class)
                .Build();

        protected string ActionClass => new CssBuilder("mud-picker-actions")
            .AddClass(ClassActions)
            .Build();

        /// <summary>
        /// Sets the icon of the input text field
        /// </summary>
        [ExcludeFromCodeCoverage]
        [Parameter]
        [Obsolete("Use AdornmentIcon instead.", true)]
        public string InputIcon
        {
            get { return AdornmentIcon; }
            set { AdornmentIcon = value; }
        }

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
        [CascadingParameter(Name = "ParentDisabled")] private bool ParentDisabled { get; set; }
        protected bool GetDisabledState() => Disabled || ParentDisabled;

        /// <summary>
        /// If true, no date or time can be defined.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool ReadOnly { get; set; }
        [CascadingParameter(Name = "ParentReadOnly")] private bool ParentReadOnly { get; set; }
        protected bool GetReadOnlyState() => ReadOnly || ParentReadOnly;

        /// <summary>
        /// If true, the picker will be editable.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Editable { get; set; } = false;

        /// <summary>
        /// Hide toolbar and show only date/time views.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public bool DisableToolbar { get; set; }

        /// <summary>
        /// User class names for picker's ToolBar, separated by space
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.PickerAppearance)]
        public string ToolBarClass { get; set; }

        /// <summary>
        /// Picker container option
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public PickerVariant PickerVariant { get; set; } = PickerVariant.Inline;

        /// <summary>
        ///  Variant of the text input
        /// </summary>
        [ExcludeFromCodeCoverage]
        [Parameter]
        [Obsolete("Use Variant instead.", true)]
        public Variant InputVariant
        {
            get { return Variant; }
            set { Variant = value; }
        }

        /// <summary>
        /// Variant of the text input
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        /// Sets if the icon will be att start or end, set to false to disable.
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
        /// Changes the cursor appearance.
        /// </summary>
        [Obsolete("This is enabled now by default when you use Editable=true. You can remove the parameter.", false)]
        [Parameter]
        public bool AllowKeyboardInput { get; set; }

        /// <summary>
        /// Fired when the text changes.
        /// </summary>
        [Parameter]
        public EventCallback<string> TextChanged { get; set; }

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
        public string ClassActions { get; set; }

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
                    await StringValueChanged(_text);
                await TextChanged.InvokeAsync(_text);
            }
        }

        /// <summary>
        /// Value change hook for descendants.
        /// </summary>
        protected virtual Task StringValueChanged(string value)
        {
            return Task.CompletedTask;
        }

        protected bool IsOpen { get; set; }

        public void ToggleOpen()
        {
            if (IsOpen)
                Close();
            else
                Open();
        }

        public void Close(bool submit = true)
        {
            IsOpen = false;

            if (submit)
            {
                Submit();
            }

            OnClosed();
            StateHasChanged();
        }

        public void Open()
        {
            IsOpen = true;
            StateHasChanged();
            OnOpened();
        }

        private void CloseOverlay() => Close(PickerActions == null);

        protected internal virtual void Submit() { }

        public virtual void Clear(bool close = true)
        {
            if (close && PickerVariant != PickerVariant.Static)
            {
                Close(false);
            }
        }

        [Obsolete($"Use {nameof(ResetValueAsync)} instead. This will be removed in v7")]
        [ExcludeFromCodeCoverage]
        protected override void ResetValue()
        {
            _inputReference?.Reset();
            base.ResetValue();
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

            if (Label == null && For != null)
                Label = For.GetLabelString();
        }

        private async Task EnsureKeyInterceptor()
        {
            if (_keyInterceptor == null)
            {
                _keyInterceptor = _keyInterceptorFactory.Create();

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
                ToggleState();

            if (OnClick.HasDelegate)
                await OnClick.InvokeAsync(args);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender == true)
            {
                await EnsureKeyInterceptor();
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        protected internal void ToggleState()
        {
            if (GetDisabledState() || GetReadOnlyState())
                return;
            if (IsOpen)
            {
                IsOpen = false;
                OnClosed();
            }
            else
            {
                IsOpen = true;
                OnOpened();
                FocusAsync();
            }
        }

        protected virtual async void OnOpened()
        {
            OnPickerOpened();

            if (PickerVariant == PickerVariant.Inline)
            {
                await _pickerInlineRef.MudChangeCssAsync(PickerInlineClass);
            }

            await EnsureKeyInterceptor();
            await _keyInterceptor.UpdateKey(new() { Key = "Escape", StopDown = "key+none" });
        }

        protected virtual async void OnClosed()
        {
            OnPickerClosed();

            await EnsureKeyInterceptor();
            await _keyInterceptor.UpdateKey(new() { Key = "Escape", StopDown = "none" });
        }

        protected virtual void OnPickerOpened()
        {
            PickerOpened.InvokeAsync(this);
        }

        protected virtual void OnPickerClosed()
        {
            PickerClosed.InvokeAsync(this);
        }

        protected internal virtual void HandleKeyDown(KeyboardEventArgs obj)
        {
            if (GetDisabledState() || GetReadOnlyState())
                return;
            switch (obj.Key)
            {
                case "Backspace":
                    if (obj.CtrlKey == true && obj.ShiftKey == true)
                    {
                        Clear();
                        _value = default(T);
                        Reset();
                    }

                    break;
                case "Escape":
                case "Tab":
                    Close(false);
                    break;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing == true)
            {
                if (_keyInterceptor != null)
                {
                    _keyInterceptor.KeyDown -= HandleKeyDown;
                    _keyInterceptor.Dispose();
                }

            }
        }



    }
}
