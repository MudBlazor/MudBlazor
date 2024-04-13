﻿// Copyright (c) MudBlazor 2021
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

        protected string Classname => new CssBuilder("mud-toggle-item")
            .AddClass($"mud-theme-{Parent?.Color.ToDescriptionString()}", _selected && string.IsNullOrEmpty(Parent?.SelectedClass))
            .AddClass(Parent?.SelectedClass, _selected && !string.IsNullOrEmpty(Parent?.SelectedClass))
            .AddClass($"mud-toggle-item-size-{Size.ToDescriptionString()}")
            .AddClass($"mud-toggle-item-{Parent?.Color.ToDescriptionString()}")
            .AddClass("mud-toggle-item-vertical", Parent?.Vertical == true)
            .AddClass("mud-toggle-item-delimiter", Parent?.Delimiters == true)
            .AddClass("mud-ripple", Parent?.Ripple == true)
            .AddClass($"mud-border-{Parent?.Color.ToDescriptionString()} border-solid")
            .AddClass("mud-toggle-delimiter-alternative", Parent?.SelectionMode == SelectionMode.MultiSelection && IsSelected && Parent?.Color != Color.Default)
            .AddClass("mud-disabled", GetDisabledState())
            .AddClass(Class)
            .Build();

        protected string TextClassname => new CssBuilder()
            .AddClass(Parent?.TextClass)
            .Build();

        protected string CheckMarkClassname => new CssBuilder()
            .AddClass(Parent?.CheckMarkClass)
            .AddClass("me-2")
            .Build();

        private Size Size => Parent?.Size ?? Size.Medium;

        private bool CounterBalanceCheckMark => Parent?.CheckMark == true && Parent?.FixedContent == true;

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

        private string? CurrentIcon => IsSelected ? SelectedIcon ?? UnselectedIcon : UnselectedIcon;

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

        protected bool GetDisabledState() => Disabled || (Parent?.Disabled ?? false);
    }
}
