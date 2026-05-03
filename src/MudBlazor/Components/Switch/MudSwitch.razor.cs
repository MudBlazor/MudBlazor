using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{

    /// <summary>
    /// Toggles between two values with the tap of a button, visually distinct from checkboxes. Use switches (not radio buttons) if the items in a list can be independently controlled.
    /// </summary>
    /// <typeparam name="T">The kind of value being switched, typically a <see cref="bool"/>.</typeparam>
    /// <seealso cref="MudCheckBox{T}"/>
    /// <seealso cref="MudRadio{T}"/>
    public partial class MudSwitch<T> : MudBooleanInput<T>
    {
        private readonly string _ariaId = Identifier.Create("switch-aria-");
        internal string ElementId { get; } = Identifier.Create("switch");

        [Inject]
        private IKeyInterceptorService KeyInterceptorService { get; set; } = null!;

        protected override string Classname => new CssBuilder("mud-input-control-boolean-input")
            .AddClass(Class)
            .Build();

        protected override string LabelClassname => new CssBuilder("mud-switch")
            .AddClass("mud-disabled", GetDisabledState())
            .AddClass("mud-readonly", GetReadOnlyState())
            .AddClass($"mud-input-content-placement-{ConvertPlacement(LabelPlacement).ToStringFast(true)}")
            .Build();

        protected string SwitchClassname => new CssBuilder("mud-button-root mud-icon-button mud-switch-base")
            .AddClass($"mud-ripple mud-ripple-switch", Ripple && !GetReadOnlyState() && !GetDisabledState())
            .AddClass($"mud-{Color.ToStringFast(true)}-text hover:mud-{Color.ToStringFast(true)}-hover", !GetReadOnlyState() && !GetDisabledState() && BoolValue == true)
            .AddClass($"mud-{UncheckedColor.ToStringFast(true)}-text hover:mud-{UncheckedColor.ToStringFast(true)}-hover", !GetReadOnlyState() && !GetDisabledState() && BoolValue == false)
            .AddClass($"mud-switch-disabled", GetDisabledState())
            .AddClass($"mud-readonly", GetReadOnlyState())
            .AddClass($"mud-checked", BoolValue)
            .AddClass($"mud-switch-base-{Size.ToStringFast(true)}")
            .Build();

        protected string TrackClassname => new CssBuilder("mud-switch-track")
            .AddClass($"mud-{Color.ToStringFast(true)}", BoolValue == true)
            .AddClass($"mud-{UncheckedColor.ToStringFast(true)}", BoolValue == false)
            .Build();

        protected string ThumbClassname => new CssBuilder($"mud-switch-thumb-{Size.ToStringFast(true)}")
            .AddClass("d-flex align-center justify-center")
            .Build();

        protected string SpanClassname => new CssBuilder("mud-switch-span")
            .AddClass($"mud-switch-span-{Size.ToStringFast(true)}")
            .Build();

        protected string AriaCheckedState => BoolValue switch
        {
            true => "true",
            false => "false",
            null => "mixed"
        };

        /// <summary>
        /// The color of this switch when in an unchecked state.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Radio.Appearance)]
        public Color UncheckedColor { get; set; } = Color.Default;

        /// <summary>
        /// The Aria Label to be assigned to the switch.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>. Used to improve accessibility for screen readers. Adds an <c>aria-labelledby</c> to the <c>input</c> element.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Radio.Appearance)]
        public string? AriaLabel { get; set; }

        /// <summary>
        /// The icon to display for the switch thumb.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public string? ThumbIcon { get; set; }

        /// <summary>
        /// The color of the thumb icon.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>. Only applies when <see cref="ThumbIcon"/> is set.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Appearance)]
        public Color ThumbIconColor { get; set; } = Color.Default;

        /// <inheritdoc />
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var options = new KeyInterceptorOptions(
                    "mud-switch-base",
                    [
                        // prevent scrolling page, instead increment
                        new("ArrowUp", preventDown: "key+none"),
                        // prevent scrolling page, instead decrement
                        new("ArrowDown", preventDown: "key+none"),
                        new(" ", preventDown: "key+none", preventUp: "key+none")
                    ]);

                await KeyInterceptorService.SubscribeAsync(ElementId, options, keys => keys
                    .When(CanHandleKeys, builder => builder
                        .OnKeyDownAny(["ArrowLeft", "Delete"], () => SetBoolValueAsync(false, true))
                        .OnKeyDownAny(["ArrowRight", "Enter", "NumpadEnter"], () => SetBoolValueAsync(true, true))
                        .OnKeyDown(" ", () => SetBoolValueAsync(!BoolValue, true))));
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        protected Task HandleKeyDownAsync(KeyboardEventArgs obj) => KeyInterceptorService.DispatchAsync(ElementId, KeyEventKind.Down, obj);

        private bool CanHandleKeys() => !GetDisabledState() && !GetReadOnlyState();

        /// <inheritdoc />
        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            if (IsJSRuntimeAvailable)
            {
                await KeyInterceptorService.UnsubscribeAsync(ElementId);
            }
        }
    }
}
