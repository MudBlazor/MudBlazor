using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudRadio<T> : MudComponentBase, IDisposable
    {
        private bool _checked;

        [CascadingParameter] protected MudRadioGroup<T> RadioGroup { get; set; }

        protected string Classname =>
        new CssBuilder("mud-radio")
            .AddClass($"mud-disabled", Disabled)
            .AddClass($"mud-radio-content-placement-{Placement.ToDescriptionString()}", when: () => Placement != Placement.End)
            .AddClass(Class)
            .Build();

        protected string ButtonClassname =>
        new CssBuilder("mud-button-root mud-icon-button")
            .AddClass($"mud-ripple mud-ripple-radio", !DisableRipple)
            .AddClass($"mud-radio-color-{Color.ToDescriptionString()}")
            .AddClass($"mud-disabled", Disabled)
            .AddClass($"mud-checked", Checked)
            .Build();

        protected string RadioIconsClassNames =>
        new CssBuilder("mud-radio-icons")
            .AddClass($"mud-checked", Checked)
            .Build();

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The position of the child content.
        /// </summary>
        [Parameter] public Placement Placement { get; set; } = Placement.End;

        /// <summary>
        /// The value to associate to the button.
        /// </summary>
        [Parameter] public T Option { get; set; }

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

        internal bool Checked => _checked;

        internal void SetChecked(bool value)
        {
            if (_checked != value)
            {
                _checked = value;
                StateHasChanged();
            }
        }

        public void Select()
        {
            if (RadioGroup != null)
                RadioGroup.SetSelectedRadioAsync(this).AndForget();
        }

        private Task OnClick()
        {
            if (RadioGroup != null)
                return RadioGroup.SetSelectedRadioAsync(this);

            return Task.CompletedTask;
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            if (RadioGroup != null)
                await RadioGroup.RegisterRadioAsync(this);
        }

        public void Dispose()
        {
            if (RadioGroup != null)
                RadioGroup.UnregisterRadio(this);
        }
    }
}
