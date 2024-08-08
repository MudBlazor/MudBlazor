using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
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
        private string _elementId = Identifier.Create("checkbox");

        [Inject]
        private IKeyInterceptorFactory KeyInterceptorFactory { get; set; } = null!;

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
            .AddClass($"mud-checkbox-dense", Dense)
            .AddClass($"mud-ripple mud-ripple-checkbox", Ripple && !GetReadOnlyState() && !GetDisabledState())
            .AddClass($"mud-disabled", GetDisabledState())
            .AddClass($"mud-readonly", GetReadOnlyState())
            .AddClass($"mud-checkbox-true", BoolValue == true)
            .AddClass($"mud-checkbox-false", BoolValue == false)
            .AddClass($"mud-checkbox-null", BoolValue is null)
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
        public Color? UncheckedColor { get; set; } = null;

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
        /// Defaults to <c>false</c>.  When <c>true</c>, the checkbox can support an indeterminate state such as a <c>null</c> value, in addition to <c>true</c> and <c>false</c>.
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
            // Apply only when TriState parameter is set to true and T is bool?
            if (TriState && typeof(T) == typeof(bool?))
            {
                // The cycle is forced with the following steps: true, false, indeterminate, true, false, indeterminate...
                var boolValue = (bool?)(object?)_value;
                if (!boolValue.HasValue)
                {
                    return SetBoolValueAsync(true, true);
                }

                return boolValue.Value
                    ? SetBoolValueAsync(false, true)
                    : SetBoolValueAsync(default, true);
            }

            return SetBoolValueAsync((bool?)args.Value, true);
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
                    switch (BoolValue)
                    {
                        case null:
                            SetBoolValueAsync(true, true);
                            break;
                        case true:
                            SetBoolValueAsync(false, true);
                            break;
                        case false when TriState:
                            SetBoolValueAsync(null, true);
                            break;
                        case false:
                            SetBoolValueAsync(true, true);
                            break;
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
    }
}
