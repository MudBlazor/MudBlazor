// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{

#nullable enable
    public partial class MudToggleItem<T> : MudComponentBase
    {
        private bool _selected;

        protected string Classes => new CssBuilder("mud-toggle-item")
            .AddClass($"mud-theme-{Parent?.Color.ToDescriptionString()}", _selected && string.IsNullOrEmpty(Parent?.SelectedClass))
            .AddClass("mud-toggle-item-selected-border", _selected && Parent?.Outline == true)
            .AddClass(Parent?.SelectedClass, _selected && !string.IsNullOrEmpty(Parent?.SelectedClass))
            .AddClass($"mud-toggle-item-{Parent?.Color.ToDescriptionString()}")
            .AddClass("mud-ripple", Parent?.DisableRipple == false)
            .AddClass($"mud-border-{Parent?.Color.ToDescriptionString()} border-solid")
            .AddClass("mud-toggle-delimiter-alternative", Parent?.SelectionMode == SelectionMode.MultiSelection && IsSelected && Parent?.Color != Color.Default)
            .AddClass("rounded-l-xl", Parent is { Rounded: true, Vertical: false } && Parent?.IsFirstItem(this) == true)
            .AddClass("rounded-t-xl", Parent is { Rounded: true, Vertical: true } && Parent?.IsFirstItem(this) == true)
            .AddClass("rounded-r-xl", Parent is { Rounded: true, Vertical: false } && Parent?.IsLastItem(this) == true)
            .AddClass("rounded-b-xl", Parent is { Rounded: true, Vertical: true } && Parent?.IsLastItem(this) == true)
            .AddClass(ItemPadding)
            .AddClass("mud-toggle-item-vertical", Parent?.Vertical == true)
            .AddClass("mud-toggle-item-delimiter", Parent?.Delimiters == true)
            .AddClass(Class)
            .Build();
        
        protected string TextClassName => new CssBuilder()
            .AddClass(Parent?.TextClass)
            .Build();
        
        protected string CheckMarkClasses => new CssBuilder()
            .AddClass(Parent?.CheckMarkClass)
            .AddClass("me-2")
            .Build();

        protected string ItemPadding
        {
            get
            {
                if (Parent?.Vertical == true)
                {
                    if (Parent?.Rounded == true)
                        if (Parent?.IsFirstItem(this) == true)
                            return Parent?.Dense==true ? "px-1 pt-2 pb-1" : "px-2 pt-3 pb-2";
                        else if (Parent?.IsLastItem(this) == true)
                            return Parent?.Dense==true ? "px-1 pt-1 pb-2" : "px-2 pt-2 pb-3";
                        else
                            return Parent?.Dense==true ? "px-1 py-1" : "px-2 py-2";
                    // not rounded 
                    return Parent?.Dense==true ? "px-1 py-1" : "px-2 py-2";
                }
                // horizontal
                if (Parent?.Rounded == true)
                    if (Parent?.IsFirstItem(this) == true)
                        return Parent?.Dense==true ? "ps-2 pe-1 py-1" : "ps-3 pe-2 py-2";
                    else if (Parent?.IsLastItem(this) == true)
                        return Parent?.Dense==true ? "ps-1 pe-2 py-1" : "ps-2 pe-3 py-2";
                    else
                        return Parent?.Dense==true ? "px-1 py-1" : "px-2 py-2";
                // not rounded 
                return Parent?.Dense==true ? "px-1 py-1" : "px-2 py-2";
            }
        }

        private bool CounterBalanceCheckmark => Parent?.CheckMark == true && Parent?.FixedContent == true;

        [CascadingParameter]
        public MudToggleGroup<T>? Parent { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public T? Value { get; set; }

        /// <summary>
        /// Icon to show if the item is not selected (if CheckMark is true on the parent group)
        /// Leave null to show no check mark (default).
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public string? UnselectedIcon { get; set; }

        /// <summary>
        /// Icon to show if the item is selected (if CheckMark is true on the parent group)
        /// By default this is set to a check mark icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public string? SelectedIcon { get; set; } = Icons.Material.Filled.Check;

        private string? CurrentIcon => IsSelected ? SelectedIcon ?? UnselectedIcon : UnselectedIcon;
        
        /// <summary>
        /// The text to show. You need to set this only if you want a text that differs from the Value. If null,
        /// show Value?.ToString().
        /// Note: the Text is only shown if you haven't defined your own child content
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public string? Text { get; set; }

        /// <summary>
        /// Custom child content which overrides the text. The boolean parameter conveys whether or not the item is selected. 
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public RenderFragment<bool>? ChildContent { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Parent?.Register(this);
        }

        public void SetSelected(bool selected)
        {
            _selected = selected;
            StateHasChanged();
        }

        protected internal bool IsSelected => _selected;

        protected async Task HandleOnClickAsync()
        {
            if (Parent is not null)
            {
                await Parent.ToggleItemAsync(this);
            }
        }

        protected internal bool IsEmpty => string.IsNullOrEmpty(Text) && Value is null;
        
    }
}
