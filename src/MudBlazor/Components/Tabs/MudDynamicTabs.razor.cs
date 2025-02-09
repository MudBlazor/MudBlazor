// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;

#nullable enable
namespace MudBlazor
{
    /// <summary>
    /// A tab component where the tabs are controlled dynamically, similar to browser tabs.
    /// </summary>
    public partial class MudDynamicTabs : MudTabs
    {
        /// <summary>
        /// The icon for the add button.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.Add"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public string AddTabIcon { get; set; } = Icons.Material.Filled.Add;

        /// <summary>
        /// The icon for the close button.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.Close"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public string CloseTabIcon { get; set; } = Icons.Material.Filled.Close;

        /// <summary>
        /// Occurs when the "Add" button has been clicked.
        /// </summary>
        [Parameter] public EventCallback AddTab { get; set; }

        /// <summary>
        /// Occurs when a tab has been closed.
        /// </summary>
        [Parameter] public EventCallback<MudTabPanel> CloseTab { get; set; }

        /// <summary>
        /// The CSS classes applied to the "Add" button.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>""</c>. Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public string AddIconClass { get; set; } = string.Empty;

        /// <summary>
        /// The CSS styles applied to the "Add" button.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>""</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public string AddIconStyle { get; set; } = string.Empty;

        /// <summary>
        /// The CSS classes applied to the "Close" button.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>""</c>. Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public string CloseIconClass { get; set; } = string.Empty;

        /// <summary>
        /// The CSS styles applied to the "Close" button.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>""</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public string CloseIconStyle { get; set; } = string.Empty;

        /// <summary>
        /// The text shown when the user hovers over the "Add" button.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>""</c>. When <c>null</c> or empty, no tooltip will be shown.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Behavior)]
        public string AddIconToolTip { get; set; } = string.Empty;

        /// <summary>
        /// The text shown when the user hovers over the "Close" button.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>""</c>. When <c>null</c> or empty, no tooltip will be shown.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Tabs.Behavior)]
        public string CloseIconToolTip { get; set; } = string.Empty;

        protected override string InternalClassName { get; } = "mud-dynamic-tabs";
    }
}
