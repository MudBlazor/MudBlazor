using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using MudBlazor.Extensions;
using System.Windows.Input;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;
using System;

namespace MudBlazor
{
    public partial class MudChip : MudComponentBase, IDisposable
    {
        private bool _isSelected;
        [Inject] public Microsoft.AspNetCore.Components.NavigationManager UriHelper { get; set; }

        [Inject] public IJSRuntime JsRuntime { get; set; }

        protected string Classname =>
        new CssBuilder("mud-chip")
          .AddClass($"mud-chip-{Variant.ToDescriptionString()}")
          .AddClass($"mud-chip-size-{Size.ToDescriptionString()}")
          .AddClass($"mud-chip-color-{Color.ToDescriptionString()}")
          .AddClass("mud-clickable", (OnClick.HasDelegate || ChipSet != null))
          .AddClass($"mud-ripple", !DisableRipple && (OnClick.HasDelegate || ChipSet!=null))
          .AddClass("mud-chip-label", Label)
          .AddClass("mud-disabled", Disabled)
          .AddClass("mud-chip-selected", IsSelected)
          .AddClass(Class)
        .Build();

        [CascadingParameter] MudChipSet ChipSet { get; set; }

        /// <summary>
        /// The color of the component.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The size of the button. small is equivalent to the dense button styling.
        /// </summary>
        [Parameter] public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// The variant to use.
        /// </summary>
        [Parameter] public Variant Variant { get; set; } = Variant.Filled;

        /// <summary>
        /// Avatar Icon, Overrides the regular Icon if set.
        /// </summary>
        [Parameter] public string Avatar { get; set; }

        /// <summary>
        /// Avatar CSS Class, appends to Chips default avatar classes.
        /// </summary>
        [Parameter] public string AvatarClass { get; set; }

        /// <summary>
        /// Removes circle edges and applys theme default.
        /// </summary>
        [Parameter] public bool Label { get; set; }

        /// <summary>
        /// If true, the chip will not be rendered.
        /// </summary>
        [Parameter] public bool Deleted { get; set; }

        /// <summary>
        /// If true, the chip will be displayed in disabled state and no events possible.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }

        /// <summary>
        /// Sets the Icon to use.
        /// </summary>
        [Parameter] public string Icon { get; set; }

        /// <summary>
        /// Overrides the default close icon, only shown if OnClose is set.
        /// </summary>
        [Parameter] public string CloseIcon { get; set; }

        /// <summary>
        /// If true, disables ripple effect, ripple effect is only applied to clickable chips.
        /// </summary>
        [Parameter] public bool DisableRipple { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// If set to a URL, clicking the button will open the referenced document. Use Target to specify where
        /// </summary>
        [Parameter] public string Link { get; set; }

        /// <summary>
        /// The target attribute specifies where to open the link, if Link is specified. Possible values: _blank | _self | _parent | _top | <i>framename</i>
        /// </summary>
        [Parameter] public string Target { get; set; }

        /// <summary>
        /// A string you want to associate with the chip. If the ChildContent is not set this will be shown as chip text.
        /// </summary>
        [Parameter] public string Text { get; set; }

        /// <summary>
        /// If true, force browser to redirect outside component router-space.
        /// </summary>
        [Parameter] public bool ForceLoad { get; set; }

       /// <summary>
       /// If true, remove the chip from the render tree on close.
       /// </summary>
       [Parameter]
       public bool RemoveOnClose { get; set; } = true;

        /// <summary>
        /// Command executed when the user clicks on an element.
        /// </summary>
        [Parameter] public ICommand Command { get; set; }

        /// <summary>
        /// Command parameter.
        /// </summary>
        [Parameter] public object CommandParameter { get; set; }

        /// <summary>
        /// Chip click event, if set the chip focus, hover and click effects are applied.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

        /// <summary>
        /// Chip delete event, if set the delete icon will be visible.
        /// </summary>
        [Parameter] public EventCallback<MudChip> OnClose { get; set; }

        /// <summary>
        /// Set by MudChipSet
        /// </summary>
        public bool IsChecked
        {
            get => _isSelected && ChipSet?.Filter==true;
        }

        /// <summary>
        /// Set by MudChipSet
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value)
                    return;
                _isSelected = value;
                StateHasChanged();
            }
        }

        protected async Task OnClickHandler(MouseEventArgs ev)
        {
            if (ChipSet != null)
            {
                ChipSet.OnChipClicked(this);
            }
            if (Link != null)
            {
                if (string.IsNullOrWhiteSpace(Target))
                    UriHelper.NavigateTo(Link, ForceLoad);
                else
                    await JsRuntime.InvokeAsync<object>("open", Link, Target);
            }
            else
            {
                await OnClick.InvokeAsync(ev);
                if (Command?.CanExecute(CommandParameter) ?? false)
                {
                    Command.Execute(CommandParameter);
                }
            }
        }

        protected async Task OnCloseHandler(MouseEventArgs ev)
        {
            await OnClose.InvokeAsync(this);
            if (RemoveOnClose)
                Deleted = true;
            ChipSet?.OnChipDeleted(this);
            StateHasChanged();
        }

        protected override Task OnInitializedAsync()
        {
            ChipSet?.Add(this);
            return base.OnInitializedAsync();
        }

        internal void ForceRerender() => StateHasChanged();
             

        public void Dispose()
        {
            try
            {
                ChipSet?.Remove(this);
            }catch(Exception){}
        }

    }
}
