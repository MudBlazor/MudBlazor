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
        protected string Classname => new CssBuilder("mud-toggle-item")
            .AddClass($"mud-theme-{Parent?.Color.ToDescriptionString()}", Selected && string.IsNullOrEmpty(Parent?.SelectedClass))
            .AddClass(Parent?.SelectedClass, Selected && !string.IsNullOrEmpty(Parent?.SelectedClass))
            .AddClass("mud-toggle-item-selected", Selected)
            .AddClass($"mud-toggle-item-{Parent?.Color.ToDescriptionString()}")
            .AddClass("mud-toggle-item-vertical", Parent?.Vertical == true)
            .AddClass("mud-toggle-item-delimiter", Parent?.Delimiters == true)
            .AddClass("mud-ripple", Parent?.Ripple == true)
            .AddClass($"mud-border-{Parent?.Color.ToDescriptionString()} border-solid")
            .AddClass("mud-toggle-delimiter-alternative", Parent?.SelectionMode == SelectionMode.MultiSelection && Selected && Parent?.Color != Color.Default)
            .AddClass("mud-toggle-item-fixed", Parent?.CheckMark == true && Parent?.FixedContent == true)
            .AddClass("mud-disabled", GetDisabledState())
            .AddClass(Class)
            .Build();

        protected string TextClassname => new CssBuilder("mud-toggle-item-text")
            .AddClass("mud-typography mud-typography-align-center")
            .AddClass(Parent?.TextClass)
            .Build();

        protected string CheckMarkClassname => new CssBuilder("mud-toggle-item-check-icon")
            .AddClass(Parent?.CheckMarkClass)
            .Build();

        [CascadingParameter]
        public MudToggleGroup<T>? Parent { get; set; }

        /// <summary>
        /// If true, the item will be disabled.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool Disabled { get; set; }

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

        private string? CurrentIcon => Selected ? SelectedIcon ?? UnselectedIcon : UnselectedIcon;

        /// <summary>
        /// The text to show. You need to set this only if you want a text that differs from the Value. If null,
        /// show Value?.ToString().
        /// Note: the Text is only shown if you haven't defined your own child content.
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
            Selected = selected;
            StateHasChanged();
        }

        protected internal bool Selected { get; private set; }

        protected async Task HandleOnClickAsync()
        {
            if (Parent is not null)
            {
                await Parent.ToggleItemAsync(this);
            }
        }

        protected internal bool IsEmpty => string.IsNullOrEmpty(Text) && Value is null;

        protected bool GetDisabledState() => Disabled || (Parent?.Disabled ?? false);
    }
}
