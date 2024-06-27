using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor.Services;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents a form input for boolean values or selecting multiple items in a list.
    /// </summary>
    /// <typeparam name="T">The type of item managed by this checkbox.</typeparam>
    public partial class MudCheckBox<T> : MudBooleanInput<T>
    {
        private IKeyInterceptor? _keyInterceptor;
        private string _elementId = "checkbox" + Guid.NewGuid().ToString().Substring(0, 8);

        [Inject]
        private IKeyInterceptorFactory KeyInterceptorFactory { get; set; } = null!;

        [Inject]
        private IJSRuntime JsRuntime { get; set; } = null!;

        protected string Classname => new CssBuilder("mud-input-control-boolean-input")
            .AddClass(Class)
            .Build();

        protected string LabelClassname => new CssBuilder("mud-checkbox")
            .AddClass($"mud-disabled", GetDisabledState())
            .AddClass($"mud-readonly", GetReadOnlyState())
            .AddClass("flex-row-reverse", LabelPosition == LabelPosition.Start)
            .Build();

        protected string CheckBoxClassname => new CssBuilder("mud-button-root mud-icon-button")
            .AddClass($"mud-{Color.ToDescriptionString()}-text hover:mud-{Color.ToDescriptionString()}-hover", !GetReadOnlyState() && !GetDisabledState() && UncheckedColor == null || (UncheckedColor != null && BoolValue == true))
            .AddClass($"mud-{UncheckedColor?.ToDescriptionString()}-text hover:mud-{UncheckedColor?.ToDescriptionString()}-hover", !GetReadOnlyState() && !GetDisabledState() && UncheckedColor != null && BoolValue == false)
            .AddClass("mud-checkbox-dense", Dense)
            .AddClass("mud-ripple mud-ripple-checkbox", Ripple && !GetReadOnlyState() && !GetDisabledState())
            .AddClass("mud-disabled", GetDisabledState())
            .AddClass("mud-readonly", GetReadOnlyState())
            .AddClass("mud-checkbox-true", BoolValue == true)
            .AddClass("mud-checkbox-false", BoolValue == false)
            .AddClass("mud-checkbox-null", BoolValue is null)
            .Build();

        /// <summary>
        /// The color of the checkbox.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>.  Theme colors are supported.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The color of the checkbox when its <c>Value</c> is <c>false</c> or <c>null</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Theme colors are supported.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Radio.Appearance)]
        public Color? UncheckedColor { get; set; }

        /// <summary>
        /// The text to display next to the checkbox.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string? Label { get; set; }

        /// <summary>
        /// The position of the <see cref="Label" /> text.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="LabelPosition.End"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public LabelPosition LabelPosition { get; set; } = LabelPosition.End;

        /// <summary>
        /// Allows this checkbox to be controlled via the keyboard.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.  The <c>Space</c> key cycles through true and false values (or true/false/null states if <see cref="TriState"/> is <c>true</c>). <c>Delete</c> will clear the checkbox. <c>Enter</c> (or <c>NumPadEnter</c>) will set the checkbox.  <c>Backspace</c> will set an indeterminate value.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool KeyboardEnabled { get; set; } = true;

        /// <summary>
        /// Shows a ripple effect when this checkbox is clicked.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool Ripple { get; set; } = true;

        /// <summary>
        /// Uses compact padding.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// The size of the checkbox.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Size.Medium"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// The content within this checkbox.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// The icon to display for a checked state.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.CheckBox"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string CheckedIcon { get; set; } = Icons.Material.Filled.CheckBox;

        /// <summary>
        /// The icon to display for an unchecked state.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.CheckBoxOutlineBlank"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string UncheckedIcon { get; set; } = Icons.Material.Filled.CheckBoxOutlineBlank;

        /// <summary>
        /// The icon to display for an indeterminate state.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.IndeterminateCheckBox"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string IndeterminateIcon { get; set; } = Icons.Material.Filled.IndeterminateCheckBox;

        /// <summary>
        /// Allows the checkbox to have an indeterminate state.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, the checkbox can support an indeterminate state such as a <c>null</c> value, in addition to <c>true</c> and <c>false</c>, but will not support
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public bool TriState { get; set; }

        private string GetIcon()
        {
            return BoolValue switch
            {
                true => CheckedIcon,
                false => UncheckedIcon,
                _ => IndeterminateIcon
            };
        }

        protected override Task OnChange(ChangeEventArgs args)
        {
            if (TriState is false || typeof(T) != typeof(bool?))
            {
                return SetBoolValueAsync((bool?)args.Value, true);
            }

            return ApplyTriStateLogicAsync();
        }

        protected void HandleKeyDown(KeyboardEventArgs obj)
        {
            if (GetDisabledState() || GetReadOnlyState() || !KeyboardEnabled)
            {
                return;
            }

            switch (obj.Key)
            {
                case "Delete":
                    SetBoolValueAsync(false, true);
                    break;
                case "Enter" or "NumpadEnter":
                    SetBoolValueAsync(true, true);
                    break;
                case "Backspace":
                    if (TriState)
                    {
                        SetBoolValueAsync(null, true);
                    }

                    break;
                case " ":
                    if (TriState)
                    {
                        ApplyTriStateLogicAsync();
                    }
                    else
                    {
                        SetBoolValueAsync(BoolValue is false, true);
                    }
                    break;
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (Label is null && For is not null)
            {
                Label = For.GetLabelString();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _keyInterceptor = KeyInterceptorFactory.Create();

                await _keyInterceptor.Connect(_elementId, new KeyInterceptorOptions
                {
                    //EnableLogging = true,
                    TargetClass = "mud-button-root",
                    Keys =
                    {
                        new KeyOptions { Key=" ", PreventDown = "key+none", PreventUp = "key+none" }, // prevent scrolling page
                        new KeyOptions { Key="Enter", PreventDown = "key+none" },
                        new KeyOptions { Key="NumpadEnter", PreventDown = "key+none" },
                        new KeyOptions { Key="Backspace", PreventDown = "key+none" },
                    },
                });
                _keyInterceptor.KeyDown += HandleKeyDown;

                await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudInput.setIndeterminateState", FieldId, BoolValue is null);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (_keyInterceptor is not null)
                {
                    _keyInterceptor.KeyDown -= HandleKeyDown;
                    if (IsJSRuntimeAvailable)
                    {
                        _keyInterceptor.Dispose();
                    }
                }
            }
        }

        protected override async Task OnValueChangedAsync(ParameterChangedEventArgs<T?> args)
        {
            await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudInput.setIndeterminateState", FieldId, BoolValue is null);
            await base.OnValueChangedAsync(args);
        }

        private Task ApplyTriStateLogicAsync()
        {
            // tri-state handling with the following repeating pattern: true -> false -> null
            if (BoolValue.HasValue is false)
            {
                return SetBoolValueAsync(true, true);
            }

            return BoolValue.Value
                ? SetBoolValueAsync(false, true)
                : SetBoolValueAsync(default, true);
        }
    }
}
