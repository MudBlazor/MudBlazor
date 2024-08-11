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
        private string _elementId = Identifier.Create("switch");
        private IKeyInterceptor? _keyInterceptor;

        [Inject]
        private IKeyInterceptorFactory KeyInterceptorFactory { get; set; } = null!;

        protected string Classname =>
            new CssBuilder("mud-input-control-boolean-input")
                .AddClass(Class)
                .Build();

        protected string LabelClassname =>
            new CssBuilder("mud-switch")
                .AddClass("mud-disabled", GetDisabledState())
                .AddClass("mud-readonly", GetReadOnlyState())
                .AddClass(LabelPosition == LabelPosition.End ? "" : "flex-row-reverse", true)
                .Build();

        protected string SwitchLabelClassname =>
            new CssBuilder($"mud-switch-label-{Size.ToDescriptionString()}")
                .Build();

        protected string SwitchClassname =>
            new CssBuilder("mud-button-root mud-icon-button mud-switch-base")
                .AddClass($"mud-ripple mud-ripple-switch", Ripple && !GetReadOnlyState() && !GetDisabledState())
                .AddClass($"mud-{Color.ToDescriptionString()}-text hover:mud-{Color.ToDescriptionString()}-hover", !GetReadOnlyState() && !GetDisabledState() && BoolValue == true)
                .AddClass($"mud-{UncheckedColor.ToDescriptionString()}-text hover:mud-{UncheckedColor.ToDescriptionString()}-hover", !GetReadOnlyState() && !GetDisabledState() && BoolValue == false)
                .AddClass($"mud-switch-disabled", GetDisabledState())
                .AddClass($"mud-readonly", GetReadOnlyState())
                .AddClass($"mud-checked", BoolValue)
                .AddClass($"mud-switch-base-{Size.ToDescriptionString()}")
                .Build();

        protected string TrackClassname =>
            new CssBuilder("mud-switch-track")
                .AddClass($"mud-{Color.ToDescriptionString()}", BoolValue == true)
                .AddClass($"mud-{UncheckedColor.ToDescriptionString()}", BoolValue == false)
                .Build();

        protected string ThumbClassname =>
            new CssBuilder($"mud-switch-thumb-{Size.ToDescriptionString()}")
                .AddClass("d-flex align-center justify-center")
                .Build();

        protected string SpanClassname =>
            new CssBuilder("mud-switch-span mud-flip-x-rtl")
                .AddClass($"mud-switch-span-{Size.ToDescriptionString()}")
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
        public Color UncheckedColor { get; set; } = Color.Default;

        /// <summary>
        /// The text/label will be displayed next to the switch if set.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public string? Label { get; set; }

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
        public string? ThumbIcon { get; set; }

        /// <summary>
        /// The color of the thumb icon. Supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Color ThumbIconColor { get; set; } = Color.Default;

        /// <summary>
        /// Gets or sets whether to show a ripple effect when the user clicks the button. Default is true.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public bool Ripple { get; set; } = true;

        /// <summary>
        /// The Size of the switch.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Size Size { get; set; } = Size.Medium;

        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public RenderFragment? ChildContent { get; set; }

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
