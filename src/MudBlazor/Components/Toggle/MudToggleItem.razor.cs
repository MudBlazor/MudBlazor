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

        protected string Classname => new CssBuilder("mud-toggle-item")
            .AddClass($"mud-theme-{Parent?.Color.ToDescriptionString()}", _selected && string.IsNullOrEmpty(Parent?.SelectedClass))
            .AddClass(Parent?.SelectedClass, _selected && !string.IsNullOrEmpty(Parent?.SelectedClass))
            .AddClass($"mud-toggle-item-{Parent?.Color.ToDescriptionString()}")
            .AddClass("mud-ripple", Parent?.DisableRipple == false)
            .AddClass($"mud-border-{Parent?.Color.ToDescriptionString()} border-solid")
            .AddClass("border-r border-b", Parent?.Bordered == true)
            .AddClass("border-l", Parent?.Bordered == true && (Parent?.Vertical == true || Parent?.IsFirstItem(this) == true || Parent?.RightToLeft == true))
            .AddClass("border-t", Parent?.Bordered == true && (Parent?.Vertical == false || Parent?.IsFirstItem(this) == true))
            .AddClass("rounded-l-xl", Parent is { Rounded: true, Vertical: false } && Parent?.IsFirstItem(this) == true)
            .AddClass("rounded-t-xl", Parent is { Rounded: true, Vertical: true } && Parent?.IsFirstItem(this) == true)
            .AddClass("rounded-r-xl", Parent is { Rounded: true, Vertical: false } && Parent?.IsLastItem(this) == true)
            .AddClass("rounded-b-xl", Parent is { Rounded: true, Vertical: true } && Parent?.IsLastItem(this) == true)
            .AddClass("mud-toggle-item-dense", Parent?.Dense == true)
            .AddClass("mud-toggle-item-vertical", Parent?.Vertical == true)
            .AddClass(Class)
            .Build();

        protected string TextClassname => new CssBuilder()
            .AddClass(Parent?.TextClass)
            .Build();

        [CascadingParameter]
        public MudToggleGroup<T>? Parent { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public T? Value { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public string Icon { get; set; } = Icons.Material.Filled.Done;

        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public string? Text { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public RenderFragment? ChildContent { get; set; }

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

        protected internal bool IsSelected() => _selected;

        protected async Task HandleOnClickAsync()
        {
            if (Parent is not null)
            {
                await Parent.ToggleItemAsync(this);
            }
        }

        private bool IsEmpty()
        {
            return string.IsNullOrEmpty(Text) && Value is null;
        }
    }
}
