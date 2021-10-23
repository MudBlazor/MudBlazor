﻿using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudChip : MudComponentBase, IDisposable
    {
        private bool _isSelected;
        [Inject] public NavigationManager UriHelper { get; set; }

        [Inject] public IJsApiService JsApiService { get; set; }

        protected string Classname =>
        new CssBuilder("mud-chip")
          .AddClass($"mud-chip-{GetVariant().ToDescriptionString()}")
          .AddClass($"mud-chip-size-{Size.ToDescriptionString()}")
          .AddClass($"mud-chip-color-{GetColor().ToDescriptionString()}")
          .AddClass("mud-clickable", !ChipSet?.ReadOnly ?? OnClick.HasDelegate)
          .AddClass("mud-ripple", !ChipSet?.ReadOnly ?? OnClick.HasDelegate && !DisableRipple)
          .AddClass("mud-chip-label", Label)
          .AddClass("mud-disabled", Disabled)
          .AddClass("mud-chip-selected", IsSelected)
          .AddClass(Class)
        .Build();

        private Variant GetVariant()
        {
            return Variant switch
            {
                Variant.Text => IsSelected ? Variant.Filled : Variant.Text,
                Variant.Filled => IsSelected ? Variant.Text : Variant.Filled,
                Variant.Outlined => Variant.Outlined,
                _ => Variant
            };
        }

        private Color GetColor()
        {
            if (IsSelected && SelectedColor != Color.Inherit)
            {
                return SelectedColor;
            }
            else if(IsSelected && SelectedColor == Color.Inherit)
            {
                return Color;
            }
            else
            {
                return Color;
            }
        }

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
        /// The selected color to use when selected, only works togheter with ChipSet, Color.Inherit for default value.
        /// </summary>
        [Parameter] public Color SelectedColor { get; set; } = Color.Inherit;

        /// <summary>
        /// Avatar Icon, Overrides the regular Icon if set.
        /// </summary>
        [Parameter] public string Avatar { get; set; }

        /// <summary>
        /// Avatar CSS Class, appends to Chips default avatar classes.
        /// </summary>
        [Parameter] public string AvatarClass { get; set; }

        /// <summary>
        /// Removes circle edges and applies theme default.
        /// </summary>
        [Parameter] public bool Label { get; set; }

        /// <summary>
        /// If true, the chip will be displayed in disabled state and no events possible.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }

        /// <summary>
        /// Sets the Icon to use.
        /// </summary>
        [Parameter] public string Icon { get; set; }

        /// <summary>
        /// Custom checked icon.
        /// </summary>
        [Parameter] public string CheckedIcon { get; set; } = Icons.Material.Filled.Check;

        /// <summary>
        /// The color of the icon.
        /// </summary>
        [Parameter] public Color IconColor { get; set; } = Color.Inherit;

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
        /// A value that should be managed in the SelectedValues collection.
        /// Note: do not change the value during the chip's lifetime
        /// </summary>
        [Parameter] public object Value { get; set; }

        /// <summary>
        /// If true, force browser to redirect outside component router-space.
        /// </summary>
        [Parameter] public bool ForceLoad { get; set; }

        /// <summary>
        /// If true, this chip is selected by default if used in a ChipSet. 
        /// </summary>
        [Parameter] public bool? Default { get; set; }

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
            get => _isSelected && ChipSet?.Filter == true;
        }

        /// <summary>
        /// If false, this chip has not been seen before
        /// </summary>
        public bool DefaultProcessed { get; set; }

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

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (Value == null)
                Value = this;
        }

        protected async Task OnClickHandler(MouseEventArgs ev)
        {
            if (ChipSet?.ReadOnly == true)
            {
                return;
            }
            if (ChipSet != null)
            {
                _ = ChipSet.OnChipClicked(this);
            }
            if (Link != null)
            {
                // TODO: use MudElement to render <a> and this code can be removed. we know that it has potential problems on iOS
                if (string.IsNullOrWhiteSpace(Target))
                    UriHelper.NavigateTo(Link, ForceLoad);
                else
                    await JsApiService.Open(Link, Target);
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
            if (ChipSet?.ReadOnly == true)
            {
                return;
            }
            await OnClose.InvokeAsync(this);
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
            }
            catch (Exception)
            {
                /* ignore! */
            }
        }

    }
}
