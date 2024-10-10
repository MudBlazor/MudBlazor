using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudSwitch<T> : MudBooleanInput<T>
    {
        private string _elementId = "switch_" + Guid.NewGuid().ToString().Substring(0, 8);
        private IKeyInterceptor? _keyInterceptor;

        [Inject]
        private IKeyInterceptorFactory KeyInterceptorFactory { get; set; } = null!;

        protected override string Classname => new CssBuilder("mud-input-control-boolean-input")
            .AddClass(Class)
            .Build();

        protected override string LabelClassname => new CssBuilder("mud-switch")
            .AddClass("mud-disabled", GetDisabledState())
            .AddClass("mud-readonly", GetReadOnlyState())
            .AddClass($"mud-switch-label-{Size.ToDescriptionString()}")
            .AddClass($"mud-input-content-placement-{ConvertPlacement(Placement).ToDescriptionString()}")
            .Build();

        protected string SwitchClassname => new CssBuilder("mud-button-root mud-icon-button mud-switch-base")
            .AddClass($"mud-ripple mud-ripple-switch", Ripple && !GetReadOnlyState() && !GetDisabledState())
            .AddClass($"mud-{Color.ToDescriptionString()}-text hover:mud-{Color.ToDescriptionString()}-hover", !GetReadOnlyState() && !GetDisabledState() && BoolValue == true)
            .AddClass($"mud-{UncheckedColor.ToDescriptionString()}-text hover:mud-{UncheckedColor.ToDescriptionString()}-hover", !GetReadOnlyState() && !GetDisabledState() && BoolValue == false)
            .AddClass($"mud-switch-disabled", GetDisabledState())
            .AddClass($"mud-readonly", GetReadOnlyState())
            .AddClass($"mud-checked", BoolValue)
            .AddClass($"mud-switch-base-{Size.ToDescriptionString()}")
            .Build();

        protected string TrackClassname => new CssBuilder("mud-switch-track")
            .AddClass($"mud-{Color.ToDescriptionString()}", BoolValue == true)
            .AddClass($"mud-{UncheckedColor.ToDescriptionString()}", BoolValue == false)
            .Build();

        protected string ThumbClassname => new CssBuilder($"mud-switch-thumb-{Size.ToDescriptionString()}")
            .AddClass("d-flex align-center justify-center")
            .Build();

        protected string SpanClassname => new CssBuilder("mud-switch-span")
            .AddClass($"mud-switch-span-{Size.ToDescriptionString()}")
            .Build();

        /// <summary>
        /// The base color of the component in its none active/unchecked state. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Radio.Appearance)]
        public Color UncheckedColor { get; set; } = Color.Default;

        /// <summary>
        /// Shows an icon on Switch's thumb.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string? ThumbIcon { get; set; }

        /// <summary>
        /// The color of the thumb icon. Supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Color ThumbIconColor { get; set; } = Color.Default;

        protected internal void HandleKeyDown(KeyboardEventArgs obj)
        {
            if (GetDisabledState() || GetReadOnlyState())
            {
                return;
            }

            switch (obj.Key)
            {
                case "ArrowLeft" or "Delete":
                    SetBoolValueAsync(false, true);
                    break;
                case "ArrowRight" or "Enter" or "NumpadEnter":
                    SetBoolValueAsync(true, true);
                    break;
                case " ":
                    switch (BoolValue)
                    {
                        case true:
                            SetBoolValueAsync(false, true);
                            break;
                        default:
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
