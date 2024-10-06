using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudRadio<T> : MudComponentBase, IDisposable
    {
        private IMudRadioGroup? _parent;
        private string _elementId = Identifier.Create("radio");

        protected string Classname =>
            new CssBuilder("mud-radio")
                .AddClass("mud-disabled", GetDisabledState())
                .AddClass("mud-readonly", GetReadOnlyState())
                .AddClass($"mud-radio-content-placement-{ConvertPlacement(Placement).ToDescriptionString()}")
                .AddClass("mud-radio-with-content", ChildContent is not null)
                .AddClass(Class)
                .Build();

        protected string ButtonClassname =>
            new CssBuilder("mud-button-root mud-icon-button")
                .AddClass("mud-ripple mud-ripple-radio", Ripple && !GetDisabledState() && !GetReadOnlyState())
                .AddClass($"mud-{Color.ToDescriptionString()}-text", !GetDisabledState() && (UncheckedColor == null || (UncheckedColor != null && Checked)))
                .AddClass($"mud-{UncheckedColor?.ToDescriptionString()}-text", !GetDisabledState() && UncheckedColor != null && Checked == false)
                .AddClass($"hover:mud-{UncheckedColor?.ToDescriptionString()}-hover", !GetReadOnlyState() && !GetDisabledState() && (UncheckedColor == null || (UncheckedColor != null && Checked)))
                .AddClass($"hover:mud-{UncheckedColor?.ToDescriptionString()}-hover", !GetReadOnlyState() && !GetDisabledState() && UncheckedColor != null && Checked == false)
                .AddClass("mud-radio-dense", Dense)
                .AddClass("mud-disabled", GetDisabledState())
                .AddClass("mud-readonly", GetReadOnlyState())
                .AddClass("mud-checked", Checked)
                .AddClass("mud-error-text", MudRadioGroup?.HasErrors)
                .Build();

        protected string RadioIconsClassNames =>
            new CssBuilder("mud-radio-icons")
                .AddClass($"mud-checked", Checked)
                .Build();

        protected string IconClassName =>
            new CssBuilder("mud-icon-root mud-svg-icon")
                .AddClass($"mud-icon-size-{Size.ToDescriptionString()}")
                .Build();

        protected string CheckedIconClassName =>
            new CssBuilder("mud-icon-root mud-svg-icon mud-radio-icon-checked")
                .AddClass($"mud-icon-size-{Size.ToDescriptionString()}")
                .Build();

        protected string ChildSpanClassName =>
            new CssBuilder("mud-radio-content mud-typography mud-typography-body1")
                .AddClass("mud-error-text", MudRadioGroup?.HasErrors)
                .Build();

        [Inject]
        private IKeyInterceptorService KeyInterceptorService { get; set; } = null!;

        [CascadingParameter(Name = "RightToLeft")]
        public bool RightToLeft { get; set; }

        /// <summary>
        /// The parent Radio Group
        /// </summary>
        [CascadingParameter]
        internal IMudRadioGroup? IMudRadioGroup
        {
            get => _parent;
            set
            {
                _parent = value;
                _parent?.CheckGenericTypeMatch(this);
            }
        }

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Radio.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The base color of the component in its none active/unchecked state. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Radio.Appearance)]
        public Color? UncheckedColor { get; set; } = null;

        /// <summary>
        /// The position of the child content.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Radio.Behavior)]
        public Placement Placement { get; set; } = Placement.End;

        /// <summary>
        /// The value to associate to the button.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Radio.Behavior)]
        public T? Value { get; set; }

        /// <summary>
        /// If true, compact padding will be applied.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Radio.Appearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// The Size of the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Radio.Appearance)]
        public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// Gets or sets whether to show a ripple effect when the user clicks the button. Default is true.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Radio.Appearance)]
        public bool Ripple { get; set; } = true;

        /// <summary>
        /// If true, the button will be disabled.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Radio.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Radio.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        private bool GetDisabledState() => Disabled || MudRadioGroup?.GetDisabledState() == true;

        private bool GetReadOnlyState() => MudRadioGroup?.GetReadOnlyState() == true;

        internal bool Checked { get; private set; }

        internal MudRadioGroup<T>? MudRadioGroup => (MudRadioGroup<T>?)IMudRadioGroup;

        private Placement ConvertPlacement(Placement placement)
        {
            return placement switch
            {
                Placement.Left => RightToLeft ? Placement.End : Placement.Start,
                Placement.Right => RightToLeft ? Placement.Start : Placement.End,
                _ => placement
            };
        }

        internal void SetChecked(bool value)
        {
            if (Checked != value)
            {
                Checked = value;
                StateHasChanged();
            }
        }

        public Task SelectAsync()
        {
            if (MudRadioGroup is not null)
            {
                return MudRadioGroup.SetSelectedRadioAsync(this);
            }

            return Task.CompletedTask;
        }

        internal Task OnClickAsync()
        {
            if (GetDisabledState() || (GetReadOnlyState()))
            {
                return Task.CompletedTask;
            }

            if (MudRadioGroup != null)
            {
                return MudRadioGroup.SetSelectedRadioAsync(this);
            }

            return Task.CompletedTask;
        }

        protected internal async Task HandleKeyDownAsync(KeyboardEventArgs keyboardEventArgs)
        {
            if (GetDisabledState() || GetReadOnlyState())
            {
                return;
            }

            switch (keyboardEventArgs.Key)
            {
                case "Enter" or "NumpadEnter" or " ":
                    await SelectAsync();
                    break;
                case "Backspace":
                    {
                        if (MudRadioGroup is not null)
                        {
                            await MudRadioGroup.ResetAsync();
                        }

                        break;
                    }
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (MudRadioGroup is not null)
            {
                await MudRadioGroup.RegisterRadioAsync(this);
            }
        }

        public void Dispose()
        {
            MudRadioGroup?.UnregisterRadio(this);
            if (IsJSRuntimeAvailable)
            {
                // TODO: Replace with IAsyncDisposable
                KeyInterceptorService.UnsubscribeAsync(_elementId).CatchAndLog();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var options = new KeyInterceptorOptions(
                    "mud-button-root",
                    [
                        // prevent scrolling page
                        new(" ", preventDown: "key+none", preventUp: "key+none"),
                        new("Enter", preventDown: "key+none"),
                        new("NumpadEnter", preventDown: "key+none"),
                        new("Backspace", preventDown: "key+none")
                    ]);

                await KeyInterceptorService.SubscribeAsync(_elementId, options, KeyObserver.KeyDownIgnore(), KeyObserver.KeyUpIgnore());
            }

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
