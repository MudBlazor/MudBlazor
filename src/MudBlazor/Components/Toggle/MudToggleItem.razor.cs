// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// An item as part of a <see cref="MudToggleGroup{T}"/>
    /// </summary>
    /// <typeparam name="T">The type of item being toggled.</typeparam>
    /// <seealso cref="MudToggleGroup{T}"/>
    /// <seealso cref="MudRadioGroup{T}"/>
    /// <seealso cref="MudRadio{T}"/>
    public partial class MudToggleItem<T> : MudComponentBase, IDisposable
    {
        protected string Classname => new CssBuilder("mud-toggle-item")
            .AddClass(AssertedParent.SelectedClass, Selected && !string.IsNullOrEmpty(AssertedParent.SelectedClass))
            .AddClass("mud-toggle-item-selected", Selected)
            .AddClass("mud-toggle-item-vertical", AssertedParent.Vertical)
            .AddClass("mud-toggle-item-delimiter", AssertedParent.Delimiters)
            .AddClass("mud-toggle-item-fixed", AssertedParent.CheckMark && AssertedParent.FixedContent)
            .AddClass($"mud-toggle-item-size-{AssertedParent.Size.ToDescriptionString()}")
            .AddClass("mud-ripple", AssertedParent.Ripple)
            .AddClass("mud-typography-input")
            .AddClass(Class)
            .Build();

        protected string CheckMarkClassname => new CssBuilder("mud-toggle-item-check-icon")
            .AddClass(AssertedParent.CheckMarkClass)
            .Build();

        /// <summary>
        /// The <see cref="MudToggleGroup{T}"/> hosting this item if one exists.
        /// </summary>
        [CascadingParameter]
        public MudToggleGroup<T>? Parent { get; set; }

        /// <summary>
        /// The <see cref="MudToggleGroup{T}"/> hosting this item, but validated to be non-null.
        /// </summary>
        private MudToggleGroup<T> AssertedParent => Parent ?? throw new InvalidOperationException($"{nameof(MudToggleItem<T>)} must be used within a {nameof(MudToggleGroup<T>)}.");

        /// <summary>
        /// Prevents the user from interacting with this item.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// The value associated with this item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.List.Behavior)]
        public T? Value { get; set; }

        /// <summary>
        /// The icon shown for the unselected checkmark.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c> (no icon). Applies when <see cref="MudToggleGroup{T}.CheckMark"/> is <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public string? UnselectedIcon { get; set; }

        /// <summary>
        /// The icon shown for the selected checkmark.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.Check" />. Applies when <see cref="MudToggleGroup{T}.CheckMark"/> is <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public string? SelectedIcon { get; set; } = Icons.Material.Filled.Check;

        /// <summary>
        /// The text shown for this item.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>. Should be set if the displayed text differs from <c>Value?.ToString()</c>.<br />
        /// Only shows if <see cref="ChildContent"/> is not set.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public string? Text { get; set; }

        /// <summary>
        /// The custom content shown for this item.
        /// </summary>
        /// <remarks>
        /// The provided <c>boolean</c> parameter is <c>true</c> when this item is selected. When set, <see cref="Text"/> will not be displayed.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.List.Appearance)]
        public RenderFragment<bool>? ChildContent { get; set; }

        /// <summary>
        /// Whether this item is currently selected.
        /// </summary>
        protected internal bool Selected { get; private set; }

        private string? GetCurrentIcon()
        {
            if (!AssertedParent.CheckMark)
            {
                return null;
            }

            if (Selected)
            {
                return SelectedIcon;
            }

            if (UnselectedIcon is null && AssertedParent.FixedContent)
            {
                return Icons.Custom.Uncategorized.Empty;
            }

            return UnselectedIcon;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            AssertedParent.Register(this);
        }

        /// <summary>
        /// Releases resources used by this component.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Don't assume we have a parent during disposal.
                Parent?.Unregister(this);
            }
        }

        /// <summary>
        /// Releases resources used by this component.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Sets the selection state of this item.
        /// </summary>
        /// <param name="selected">When <c>true</c>, this item is selected.</param>
        public void SetSelected(bool selected)
        {
            Selected = selected;
            StateHasChanged();
        }

        protected async Task HandleOnClickAsync()
        {
            await AssertedParent.ToggleItemAsync(this);
        }
    }
}
