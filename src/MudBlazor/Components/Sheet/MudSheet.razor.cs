// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using MudBlazor.Utilities;
#nullable enable
namespace MudBlazor
{
    /// <summary>
    /// Represents a Sheet component in MudBlazor, which is used to display content in a modal or persistent overlay coming from an edge of the screen. 
    /// Typically used in mobile or responsive designs, the <see cref="MudSheet"/> can be positioned at the top, bottom, left, right, or center of the viewport.
    /// </summary>
    public partial class MudSheet : MudComponentBase
    {
        private string _elementId = Identifier.Create("sheet-");
        private int _currentSize { get; set; } = 25;

        private ParameterState<bool> _openSheetState;

        /// <summary>
        /// Initializes a new instance of the <see cref="MudSheet"/> class.
        /// </summary>
        public MudSheet()
        {
            using var registerScope = CreateRegisterScope();
            _openSheetState = registerScope.RegisterParameter<bool>(nameof(Open))
                .WithParameter(() => Open)
                .WithEventCallback(() => OpenChanged);
        }

        /// <summary>
        /// Gets the CSS class names for the sheet element based on its state and configuration.
        /// </summary>
        protected string Classname =>
            new CssBuilder("mud-sheet-container")
                .AddClass($"mud-sheet-position-{Positioning}")
                .AddClass("mud-sheet-orientation-vertical", Position is Position.Top or Position.Bottom)
                .AddClass("mud-sheet-orientation-horizontal", Position is not (Position.Top or Position.Bottom))
                .AddClass("mud-sheet-standard", Standard)
                .AddClass("mud-sheet-modal", !Standard)
                .AddClass($"mud-elevation-{Elevation}", Elevation > 0)
                .AddClass(Class)
                .Build();

        /// <summary>
        /// Gets the computed styles for the sheet element based on the current style configuration.
        /// </summary>
        protected string Stylename =>
            new StyleBuilder()
                .AddStyle("width", $"{CurrentSize}vw", CurrentSize > 0 && (Position is Position.Left or Position.Right or Position.Center))
                .AddStyle("height", $"{CurrentSize}vh", CurrentSize > 0 && (Position is Position.Top or Position.Bottom or Position.Center))
                .AddStyle(Style, !string.IsNullOrEmpty(Style))
                .Build();

        /// <summary>
        /// Gets the CSS class name for the <see cref="MudPopover"/> element, including position-specific styling.
        /// </summary>
        protected string PopoverClassname =>
            new CssBuilder("mud-sheet-popover")
                .AddClass($"mud-sheet-position-{Positioning}")
                .AddClass("mud-popover-position-override")
                .Build();

        /// <summary>
        /// Gets the origin point based on the current position and layout direction.
        /// </summary>
        /// <remarks>The returned origin is determined by the <see cref="Position"/> property and, if
        /// applicable,  the <see cref="RightToLeft"/> layout setting. This ensures the origin aligns correctly with 
        /// the specified position and text direction resulting in a BottomSheet, or SideSheet.</remarks>
        protected Origin Origin =>
            Position switch
            {
                Position.Bottom => Origin.BottomCenter,
                Position.Start => RightToLeft ? Origin.CenterRight : Origin.CenterLeft,
                Position.End => RightToLeft ? Origin.CenterLeft : Origin.CenterRight,
                Position.Left => Origin.CenterLeft,
                Position.Right => Origin.CenterRight,
                Position.Top => Origin.TopCenter,
                _ => Origin.CenterCenter
            };

        /// <summary>
        /// Gets a cascading value indicating whether the layout and text direction are rendered in a right-to-left format.
        /// </summary>
        [CascadingParameter(Name = "RightToLeft")]
        [Category(CategoryTypes.Sheet.Appearance)]
        public bool RightToLeft { get; set; }

        /// <summary>
        /// The size of the drop shadow.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>16</c>.  A higher number creates a heavier drop shadow.  Use a value of <c>0</c> for no shadow.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Paper.Appearance)]
        public int Elevation { set; get; } = 16;

        /// <summary>
        /// The icon used as the drag handle when <see cref="Position"/> is <c>Top</c> or <c>Bottom</c>.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Sheet.Appearance)]
        public string VerticalHandle { get; set; } = Icons.Material.Filled.DragHandle;

        /// <summary>
        /// The icon used as the drag handle when <see cref="Position"/> is horizontal.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Sheet.Appearance)]
        public string HorizontalHandle { get; set; } = Icons.Material.Filled.DragIndicator;

        /// <summary>
        /// The position of the component relative to its reference point.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Position.Bottom"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Sheet.Appearance)]
        public Position Position { get; set; } = Position.Bottom;

        /// <summary>
        /// Gets or sets a value indicating whether the sheet is open or closed.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Sheet.Behavior)]
        public bool Open { get; set; } // TODO: See if a change handler is needed to update state if set outside of the component

        /// <summary>
        /// The callback that is invoked when the open state of the sheet changes.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Sheet.Behavior)]
        public EventCallback<bool> OpenChanged { get; set; }

        /// <summary>
        /// The callback that is invoked when the bottom sheet is closed.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Sheet.Behavior)]
        public EventCallback OnDismissed { get; set; }

        /// <summary>
        /// The content to be rendered inside the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Sheet.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// ARIA label on the sheet container for accessibility.
        /// </summary>
        /// <remarks>
        /// If not overridden, defaults to a string that includes the position of the sheet. e.g. Bottom Sheet, Left Sheet, Start Sheet, etc.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Sheet.Appearance)]
        public string AriaLabel { get; set; } = string.Empty;

        /// <summary>
        /// Whether this is a persistent (standard) sheet. If false, a MudOverlay is shown.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// <para>
        /// Standard sheets are persistent and do not close when clicking outside of them.<br/>
        /// When set to <c>false</c>, the sheet behaves like a modal popover, closing when clicking outside of it.<br/>
        /// </para>
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Sheet.Appearance)]
        public bool Standard { get; set; } = true;

        /// <summary>
        /// The starting size in vh or vw percentage starting from the edge of the screen.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>25</c>.<br/>
        /// Material Design specification starting size should not exceed 50%
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Sheet.Appearance)]
        public int OpeningSize { get; set; } = 25;

        /// <summary>
        /// List of snap point heights (in vh%) to toggle or drag
        /// </summary>
        /// <remarks>
        /// Defaults to <c>20, 40, 50, 70, 90, 100</c>.<br/>
        /// Valid values are between 10 and 100, inclusive.<br/>
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Sheet.Appearance)]
        public int[] PresetSizes { get; set; } = new[] { 20, 40, 50, 70, 90, 100 };

        /// <summary>
        /// Indicates whether the drag or toggle methods can roam outside of <see cref="PresetSizes"/> or not.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. 
        /// <para>
        /// When <c>true</c>, the user can drag or cycle only through the preset sizes.<br/>
        /// When <c>false</c>, the user can drag or cycle through any size.
        /// </para>
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Sheet.Behavior)]
        public bool SnapMode { get; set; }

        /// <summary>
        /// Returns the Current Drag Handle Icon based on the Position of the sheet.
        /// </summary>
        protected string DragHandle => Position is Position.Top or Position.Bottom ? VerticalHandle : HorizontalHandle;

        /// <summary>
        /// Returns the Positioning string for the popover using RightToLeft logic.
        /// </summary>
        protected string Positioning => Position switch
        {
            Position.Bottom => "bottom",
            Position.Start => RightToLeft ? "right" : "left",
            Position.End => RightToLeft ? "left" : "right",
            Position.Left => "left",
            Position.Right => "right",
            Position.Top => "top",
            _ => "center"
        };

        /// <summary>
        /// Returns the Current Size of the sheet as a percentage of the viewport height (vh) or width (vw).
        /// </summary>
        public int CurrentSize => _openSheetState.Value ? _currentSize : 0;

        /// <summary>
        /// Opens the sheet if it is not already open.
        /// </summary>
        /// <remarks>If the sheet is already open, this method does nothing. Otherwise, it sets the
        /// sheet's state to open and triggers the <see cref="OpenChanged"/> event with a value of <see
        /// langword="true"/>.</remarks>
        public async Task OpenSheetAsync()
        {
            var open = _openSheetState.Value;
            if (!open)
            {
                // calling the open event shouldn't trigger a callback if open did not change
                await _openSheetState.SetValueAsync(true);
                await OpenChanged.InvokeAsync(true);
            }
        }

        /// <summary>
        /// Closes the currently open sheet, if it is open.
        /// </summary>
        /// <remarks>This method updates the sheet's state to closed and triggers the appropriate events
        /// to notify listeners of the state change. If the sheet is already closed, no action is taken.</remarks>
        public async Task CloseSheetAsync()
        {
            var open = _openSheetState.Value;
            if (open)
            {
                // calling the close event shouldn't trigger a callback if open did not change
                await _openSheetState.SetValueAsync(false);
                await OpenChanged.InvokeAsync(false);
                await OnDismissed.InvokeAsync();
            }
        }

        /// <summary>
        /// This method cycles through the <see cref="PresetSizes"/> array, moving to the next larger size in the 
        /// list and closes once it reaches the max.
        /// </summary>
        /// <returns>
        /// The new size after toggling. Returns 0 if the sheet is closed.
        /// </returns>
        public async Task<int> ToggleSize()
        {
            var nextIndex = (Array.IndexOf(PresetSizes, _currentSize) + 1);
            if (nextIndex >= PresetSizes.Length)
            {
                await CloseSheetAsync();
                _currentSize = 0;
            }
            else
            {
                _currentSize = PresetSizes[nextIndex];
            }
            return _currentSize;
        }

        /// <summary>
        /// Changes the current size to the specified value.
        /// </summary>
        /// <param name="size">The new size value. Must be between 0 and 100, inclusive.</param>
        /// <param name="ignoreSnapMode">If <c>true</c>, the size change will ignore the snap mode and set the size directly.</param>
        public async Task ChangeSize(int size, bool ignoreSnapMode = false)
        {
            if (size < 0)
                size = 0;
            else if (size > 100)
                size = 100;

            if (!ignoreSnapMode && SnapMode)
            {
                var nearestPresetIndex = Array.FindIndex(PresetSizes, s => s >= size);
                _currentSize = nearestPresetIndex >= 0 ? PresetSizes[nearestPresetIndex] : size;
            }
            else
                _currentSize = size;

            await Task.CompletedTask;
        }

        /// <summary>
        /// Override SetParametersAsync to update any needed changes
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            // track changes
            var ariaChanged = parameters.TryGetValue<string>(nameof(AriaLabel), out var ariaLabel) &&
                ariaLabel != AriaLabel || string.IsNullOrEmpty(AriaLabel);
            var startSizeChanged = parameters.TryGetValue<int>(nameof(OpeningSize), out var openingSize) &&
                openingSize != OpeningSize || openingSize != _currentSize;

            // make relative updates, the out var will be set to the value of the parameter if it exists,
            // but if it doesn't exist it will be the default value of the type.
            if (ariaChanged)
            {
                if (parameters.)
                    AriaLabel = $"{CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Positioning.ToString().ToLower())} Sheet";
            }
            if (startSizeChanged)
            {
                _currentSize = openingSize;
                // Check if OpeningSize was actually passed
                var openingSizeWasPassed = parameters.TryGetValue<int>(nameof(OpeningSize), out var oopeningSize);

            }
            await base.SetParametersAsync(parameters);
        }
    }
}
