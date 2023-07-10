using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public partial class MudDynamicTabs : MudTabs
    {
        /// <summary>
        /// The icon used for the add button
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public string AddTabIcon { get; set; } = Icons.Material.Filled.Add;

        /// <summary>
        /// The icon used for the close button
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public string CloseTabIcon { get; set; } = Icons.Material.Filled.Close;

        /// <summary>
        /// The callback, when the add button has been clicked
        /// </summary>
        [Parameter] public EventCallback AddTab { get; set; }

        /// <summary>
        /// The callback, when the close button has been clicked
        /// </summary>
        [Parameter] public EventCallback<MudTabPanel> CloseTab { get; set; }

        /// <summary>
        /// Classes that are applied to the icon button of the add button
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public string AddIconClass { get; set; } = string.Empty;

        /// <summary>
        /// Styles that are applied to the icon button of the add button
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public string AddIconStyle { get; set; } = string.Empty;

        /// <summary>
        /// Classes that are applied to the icon button of the close button
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public string CloseIconClass { get; set; } = string.Empty;

        /// <summary>
        /// Styles that are applied to the icon button of the close button
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Tabs.Appearance)]
        public string CloseIconStyle { get; set; } = string.Empty;

        /// <summary>
        /// Tooltip that shown when a user hovers of the add button. Empty or null, if no tooltip should be visible
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Tabs.Behavior)]
        public string AddIconToolTip { get; set; } = string.Empty;

        /// <summary>
        /// Tooltip that shown when a user hovers of the close button. Empty or null, if no tooltip should be visible
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Tabs.Behavior)]
        public string CloseIconToolTip { get; set; } = string.Empty;

        protected override string InternalClassName { get; } = "mud-dynamic-tabs";
    }
}
