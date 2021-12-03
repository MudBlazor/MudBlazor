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
        new CssBuilder("mud-checkbox")
            .AddClass($"mud-disabled", Disabled)
            .AddClass($"mud-readonly", ReadOnly)
          .AddClass(Class)
        .Build();

        protected string CheckBoxClassname =>
        new CssBuilder("mud-button-root mud-icon-button")
            .AddClass($"mud-icon-button-color-{Color.ToDescriptionString()}")
            .AddClass($"mud-checkbox-dense", Dense)
            .AddClass($"mud-ripple mud-ripple-checkbox", !DisableRipple)
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
        /// If applied the text will be added to the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string Label { get; set; }

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
            if (Disabled || ReadOnly)
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

        [Inject] private IKeyInterceptor _keyInterceptor { get; set; }

        private string _elementId = "checkbox" + Guid.NewGuid().ToString().Substring(0, 8);

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
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
    }
}
