using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudSwitch<T> : MudBooleanInput<T>
    {
        protected string Classname =>
        new CssBuilder("mud-switch")
            .AddClass($"mud-disabled", Disabled)
            .AddClass($"mud-readonly", ReadOnly)
            .AddClass(LabelPosition == LabelPosition.End ? "mud-ltr" : "mud-rtl", true)
            .AddClass(Class)
        .Build();

        protected string SwitchClassname =>
        new CssBuilder("mud-button-root mud-icon-button mud-switch-base")
            .AddClass($"mud-ripple mud-ripple-switch", !DisableRipple && !ReadOnly && !Disabled)
            .AddClass($"mud-{Color.ToDescriptionString()}-text hover:mud-{Color.ToDescriptionString()}-hover", BoolValue == true)
            .AddClass($"mud-{UnCheckedColor.ToDescriptionString()}-text hover:mud-{UnCheckedColor.ToDescriptionString()}-hover", BoolValue == false)
            .AddClass($"mud-switch-disabled", Disabled)
            .AddClass($"mud-readonly", ReadOnly)
            .AddClass($"mud-checked", BoolValue)
        .Build();

        protected string TrackClassname =>
        new CssBuilder("mud-switch-track")
            .AddClass($"mud-{Color.ToDescriptionString()}", BoolValue == true)
            .AddClass($"mud-{UnCheckedColor.ToDescriptionString()}", BoolValue == false)
        .Build();

        //Excluded because not used
        [ExcludeFromCodeCoverage]
        protected string SpanClassname =>
        new CssBuilder("mud-switch-span mud-flip-x-rtl")
        .Build();

        private IKeyInterceptor _keyInterceptor;
        [Inject] private IKeyInterceptorFactory KeyInterceptorFactory { get; set; }

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
        public Color UnCheckedColor { get; set; } = Color.Default;

        /// <summary>
        /// The text/label will be displayed next to the switch if set.
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
        /// Shows an icon on Switch's thumb.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string ThumbIcon { get; set; }

        /// <summary>
        /// The color of the thumb icon. Supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Color ThumbIconColor { get; set; } = Color.Default;

        /// <summary>
        /// If true, disables ripple effect.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool DisableRipple { get; set; }

        protected internal void HandleKeyDown(KeyboardEventArgs obj)
        {
            if (Disabled || ReadOnly)
                return;
            switch (obj.Key)
            {
                case "ArrowLeft":
                case "Delete":
                    SetBoolValueAsync(false);
                    break;
                case "ArrowRight":
                case "Enter":
                case "NumpadEnter":
                    SetBoolValueAsync(true);
                    break;
                case " ":
                    if (BoolValue == true)
                    {
                        SetBoolValueAsync(false);
                    }
                    else
                    {
                        SetBoolValueAsync(true);
                    }
                    break;
            }
        }

        private string _elementId = "switch_" + Guid.NewGuid().ToString().Substring(0, 8);

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _keyInterceptor = KeyInterceptorFactory.Create();

                await _keyInterceptor.Connect(_elementId, new KeyInterceptorOptions()
                {
                    //EnableLogging = true,
                    TargetClass = "mud-switch-base",
                    Keys = {
                        new KeyOptions { Key="ArrowUp", PreventDown = "key+none" }, // prevent scrolling page, instead increment
                        new KeyOptions { Key="ArrowDown", PreventDown = "key+none" }, // prevent scrolling page, instead decrement
                        new KeyOptions { Key=" ", PreventDown = "key+none", PreventUp = "key+none" },
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
