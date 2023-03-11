using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Services;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudRadio<T> : MudComponentBase, IDisposable
    {
        [CascadingParameter(Name = "RightToLeft")] public bool RightToLeft { get; set; }

        protected string Classname =>
        new CssBuilder("mud-radio")
            .AddClass($"mud-disabled", IsDisabled)
            .AddClass($"mud-readonly", MudRadioGroup?.ReadOnly)
            .AddClass($"mud-radio-content-placement-{ConvertPlacement(Placement).ToDescriptionString()}")
            .AddClass(Class)
            .Build();

        protected string ButtonClassname =>
        new CssBuilder("mud-button-root mud-icon-button")
            .AddClass($"mud-ripple mud-ripple-radio", !DisableRipple && !Disabled && !(MudRadioGroup?.Disabled ?? false) && !(MudRadioGroup?.ReadOnly ?? false))
            .AddClass($"mud-{Color.ToDescriptionString()}-text hover:mud-{Color.ToDescriptionString()}-hover", UnCheckedColor == null || (UnCheckedColor != null && Checked == true))
            .AddClass($"mud-{UnCheckedColor?.ToDescriptionString()}-text hover:mud-{UnCheckedColor?.ToDescriptionString()}-hover", UnCheckedColor != null && Checked == false)
            .AddClass($"mud-radio-dense", Dense)
            .AddClass($"mud-disabled", IsDisabled)
            .AddClass($"mud-readonly", MudRadioGroup?.ReadOnly)
            .AddClass($"mud-checked", Checked)
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
            .AddClass("mud-error-text", MudRadioGroup.HasErrors)
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
                Placement.Left => RightToLeft ? Placement.End : Placement.Start,
                Placement.Right => RightToLeft ? Placement.Start : Placement.End,
                _ => placement
            };
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
        public Color? UnCheckedColor { get; set; } = null;

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
        public T Option { get; set; }

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
        /// If true, disables ripple effect.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Radio.Appearance)]
        public bool DisableRipple { get; set; }

        /// <summary>
        /// If true, the button will be disabled.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Radio.Behavior)]
        public bool Disabled { get; set; }
        private bool IsDisabled => Disabled || (MudRadioGroup?.Disabled ?? false);

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Radio.Behavior)]
        public RenderFragment ChildContent { get; set; }

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

        internal Task OnClick()
        {
            if (IsDisabled || (MudRadioGroup?.ReadOnly ?? false)) return Task.CompletedTask;
            if (MudRadioGroup != null)
                return MudRadioGroup.SetSelectedRadioAsync(this);

            return Task.CompletedTask;
        }

        protected internal void HandleKeyDown(KeyboardEventArgs obj)
        {
            if (IsDisabled || (MudRadioGroup?.ReadOnly ?? false))
                return;
            switch (obj.Key)
            {
                case "Enter":
                case "NumpadEnter":
                case " ":
                    Select();
                    break;
                case "Backspace":
                    MudRadioGroup.Reset();
                    break;
            }
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
            _keyInterceptor?.Dispose();
        }

        private IKeyInterceptor _keyInterceptor;
        [Inject] private IKeyInterceptorFactory _keyInterceptorFactory { get; set; }

        private string _elementId = "radio" + Guid.NewGuid().ToString().Substring(0, 8);

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
            }
            await base.OnAfterRenderAsync(firstRender);
        }

    }
}
