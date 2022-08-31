using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudCheckBox<T> : MudBooleanInput<T>
    {
        protected string Classname =>
        new CssBuilder("mud-input-control-boolean-input")
            .AddClass(Class)
            .Build();

        protected string LabelClassname =>
        new CssBuilder("mud-checkbox")
            .AddClass($"mud-disabled", Disabled)
            .AddClass($"mud-readonly", ReadOnly)
            .AddClass(LabelPosition == LabelPosition.End ? "mud-ltr" : "mud-rtl", true)
        .Build();

        protected string CheckBoxClassname =>
        new CssBuilder("mud-button-root mud-icon-button")
            .AddClass($"mud-{Color.ToDescriptionString()}-text hover:mud-{Color.ToDescriptionString()}-hover", UnCheckedColor == null || (UnCheckedColor != null && BoolValue == true))
            .AddClass($"mud-{UnCheckedColor?.ToDescriptionString()}-text hover:mud-{UnCheckedColor?.ToDescriptionString()}-hover", UnCheckedColor != null && BoolValue == false)
            .AddClass($"mud-checkbox-dense", Dense)
            .AddClass($"mud-ripple mud-ripple-checkbox", !DisableRipple && !ReadOnly && !Disabled)
            .AddClass($"mud-disabled", Disabled)
            .AddClass($"mud-readonly", ReadOnly)
        .Build();

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The base color of the component in its none active/unchecked state. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Radio.Appearance)]
        public Color? UnCheckedColor { get; set; } = null;

        /// <summary>
        /// The text/label will be displayed next to the checkbox if set.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string Label { get; set; }

        /// <summary>
        /// The position of the text/label.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public LabelPosition LabelPosition { get; set; } = LabelPosition.End;

        /// <summary>
        /// If true, the checkbox can be controlled with the keyboard.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool KeyboardEnabled { get; set; } = true;

        /// <summary>
        /// If true, disables ripple effect.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool DisableRipple { get; set; }

        /// <summary>
        /// If true, compact padding will be applied.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// The Size of the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Custom checked icon, leave null for default.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string CheckedIcon { get; set; } = Icons.Material.Filled.CheckBox;

        /// <summary>
        /// Custom unchecked icon, leave null for default.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string UncheckedIcon { get; set; } = Icons.Material.Filled.CheckBoxOutlineBlank;

        /// <summary>
        /// Custom indeterminate icon, leave null for default.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string IndeterminateIcon { get; set; } = Icons.Material.Filled.IndeterminateCheckBox;

        /// <summary>
        /// Define if the checkbox can cycle again through indeterminate status.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Validation)]
        public bool TriState { get; set; }

        private string GetIcon()
        {
            if (BoolValue == true)
            {
                return CheckedIcon;
            }

            if (BoolValue == false)
            {
                return UncheckedIcon;
            }

            return IndeterminateIcon;
        }

        protected override Task OnChange(ChangeEventArgs args)
        {
            Touched = true;

            // Apply only when TriState parameter is set to true and T is bool?
            if (TriState && typeof(T) == typeof(bool?))
            {
                // The cycle is forced with the following steps: true, false, indeterminate, true, false, indeterminate...
                if (!((bool?)(object)_value).HasValue)
                {
                    return SetBoolValueAsync(true);
                }
                else
                {
                    return ((bool?)(object)_value).Value ? SetBoolValueAsync(false) : SetBoolValueAsync(default);
                }
            }
            else
            {
                return SetBoolValueAsync((bool?)args.Value);
            }
        }

        protected void HandleKeyDown(KeyboardEventArgs obj)
        {
            if (Disabled || ReadOnly || !KeyboardEnabled)
                return;
            switch (obj.Key)
            {
                case "Delete":
                    SetBoolValueAsync(false);
                    break;
                case "Enter":
                case "NumpadEnter":
                    SetBoolValueAsync(true);
                    break;
                case "Backspace":
                    if (TriState)
                    {
                        SetBoolValueAsync(null);
                    }
                    break;
                case " ":
                    if (BoolValue == null)
                    {
                        SetBoolValueAsync(true);
                    }
                    else if (BoolValue == true)
                    {
                        SetBoolValueAsync(false);
                    }
                    else if (BoolValue == false)
                    {
                        if (TriState == true)
                        {
                            SetBoolValueAsync(null);
                        }
                        else
                        {
                            SetBoolValueAsync(true);
                        }
                    }
                    break;
            }
        }

        private IKeyInterceptor _keyInterceptor;
        [Inject] private IKeyInterceptorFactory _keyInterceptorFactory { get; set; }

        private string _elementId = "checkbox" + Guid.NewGuid().ToString().Substring(0, 8);

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _keyInterceptor = _keyInterceptorFactory.Create();

                await _keyInterceptor.Connect(_elementId, new KeyInterceptorOptions()
                {
                    //EnableLogging = true,
                    TargetClass = "mud-button-root",
                    Keys = {
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

            if (disposing == true)
            {
                if(_keyInterceptor != null)
                {
                    _keyInterceptor.KeyDown -= HandleKeyDown;
                    _keyInterceptor.Dispose();
                }
            }
        }
    }
}
