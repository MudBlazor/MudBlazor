using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudRadio<T> : MudComponentBase, IDisposable
    {
        [CascadingParameter] public bool RightToLeft { get; set; }

        protected string Classname =>
        new CssBuilder("mud-radio")
            .AddClass($"mud-disabled", Disabled)
            .AddClass($"mud-radio-content-placement-{ConvertPlacement(Placement).ToDescriptionString()}")
            .AddClass(Class)
            .Build();

        protected string ButtonClassname =>
        new CssBuilder("mud-button-root mud-icon-button")
            .AddClass($"mud-ripple mud-ripple-radio", !DisableRipple)
            .AddClass($"mud-radio-color-{Color.ToDescriptionString()}")
            .AddClass($"mud-radio-dense", Dense)
            .AddClass($"mud-disabled", Disabled)
            .AddClass($"mud-checked", Checked)
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

        private IMudRadioGroup _parent;

        /// <summary>
        /// The parent Radio Group
        /// </summary>
        [CascadingParameter]
        internal IMudRadioGroup IMudRadioGroup
        {
            get => _parent;
            set
            {
                _parent = value;
                if (_parent == null)
                    return;
                _parent.CheckGenericTypeMatch(this);
                //MudRadioGroup<T>?.Add(this);
            }
        }

        internal MudRadioGroup<T> MudRadioGroup => (MudRadioGroup<T>)IMudRadioGroup;

        private Placement ConvertPlacement(Placement placement)
        {
            return placement switch
            {
                Placement.Left => RightToLeft ? Placement.Right : Placement.Left,
                Placement.Right => RightToLeft ? Placement.Left : Placement.Right,
                _ => placement
            };
        }

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The position of the child content.
        /// </summary>
        [Parameter] public Placement Placement { get; set; } = Placement.Right;

        /// <summary>
        /// The value to associate to the button.
        /// </summary>
        [Parameter] public T Option { get; set; }

        /// <summary>
        /// If true, compact padding will be applied.
        /// </summary>
        [Parameter] public bool Dense { get; set; }

        /// <summary>
        /// The Size of the component.
        /// </summary>
        [Parameter] public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// If true, disables ripple effect.
        /// </summary>
        [Parameter] public bool DisableRipple { get; set; }

        /// <summary>
        /// If true, the button will be disabled.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        internal bool Checked { get; private set; }

        internal void SetChecked(bool value)
        {
            if (Checked != value)
            {
                Checked = value;
                StateHasChanged();
            }
        }

        public void Select()
        {
            MudRadioGroup?.SetSelectedRadioAsync(this).AndForget();
        }

        private Task OnClick()
        {
            if (MudRadioGroup != null)
                return MudRadioGroup.SetSelectedRadioAsync(this);

            return Task.CompletedTask;
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (MudRadioGroup != null)
                await MudRadioGroup.RegisterRadioAsync(this);
        }

        public void Dispose()
        {
            MudRadioGroup?.UnregisterRadio(this);
        }
    }
}
