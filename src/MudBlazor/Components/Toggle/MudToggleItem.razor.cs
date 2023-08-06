// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudToggleItem<T> : MudComponentBase
    {
        protected string Classname => new CssBuilder("mud-toggle-item")
            .AddClass($"mud-theme-{Parent?.Color.ToDescriptionString()}", _selected && string.IsNullOrEmpty(Parent?.SelectedClass))
            .AddClass(Parent?.SelectedClass, _selected && string.IsNullOrEmpty(Parent?.SelectedClass) == false)
            .AddClass($"mud-toggle-item-{Parent?.Color.ToDescriptionString()}")
            .AddClass("mud-ripple", Parent?.DisableRipple == false)
            .AddClass($"mud-border-{Parent?.Color.ToDescriptionString()} border-solid")
            .AddClass("border-r-2 border-b-2")
            .AddClass("border-l-2", Parent?.Vertical == true || Parent?.IsFirstItem(this) == true)
            .AddClass("border-t-2", Parent?.Vertical == false || Parent?.IsFirstItem(this) == true)
            .AddClass("rounded-l-xl", Parent?.Rounded == true && Parent?.Vertical == false && Parent?.IsFirstItem(this) == true)
            .AddClass("rounded-t-xl", Parent?.Rounded == true && Parent?.Vertical == true && Parent?.IsFirstItem(this) == true)
            .AddClass("rounded-r-xl", Parent?.Rounded == true && Parent?.Vertical == false && Parent?.IsLastItem(this) == true)
            .AddClass("rounded-b-xl", Parent?.Rounded == true && Parent?.Vertical == true && Parent?.IsLastItem(this) == true)
            .AddClass("mud-toggle-item-dense", Parent?.Dense == true)
            .AddClass("mud-toggle-item-vertical", Parent?.Vertical == true)
            .AddClass(Class)
            .Build();

        protected string TextClassname => new CssBuilder()
            .AddClass("me-2", _selected == true && IsEmpty() == false && Parent?.ShowSelectedIcon == true)
            .AddClass(Parent?.TextClass)
            .Build();

        protected string Stylename => new StyleBuilder()
            .AddStyle("min-width", $"{Parent?.GetItemWidth(this).ToInvariantString()}%", Parent?.Vertical == false && IsEmpty() == false)
            .AddStyle("width", "fit-content", Parent?.Vertical == true || IsEmpty())
            .AddStyle("height", "fit-content", Parent?.Vertical == true || IsEmpty())
            .AddStyle(Style)
            .Build();

        bool _selected;

        [CascadingParameter]
        public MudToggleGroup<T> Parent { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public T Value { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public string Icon { get; set; } = Icons.Material.Filled.Done;

        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public string Text { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Parent?.Register(this);
        }

        protected internal void ForceRender()
        {
            StateHasChanged();
        }

        public void SetSelected(bool selected)
        {
            _selected = selected;
            StateHasChanged();
        }

        protected internal bool IsSelected() => _selected;

        protected async Task HandleOnClick()
        {
            await Parent.ToggleItem(this);
        }

        private bool IsEmpty()
        {
            if (string.IsNullOrEmpty(Text) && Value == null)
            {
                return true;
            }
            return false;
        }

    }
}
