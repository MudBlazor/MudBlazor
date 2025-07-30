// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Data;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor.State;
using MudBlazor.Utilities;
#nullable enable
namespace MudBlazor;

/// <summary>
/// A Material Design sheet component that slides in from an edge of the screen to display contextual content. 
/// Commonly used in mobile UIs for lists or selections (bottom sheet) or in desktop layouts for menus or side panels (side sheet). 
/// Supports positioning at the top, bottom, left, right, or center.
/// </summary>
public partial class MudSheet : MudComponentBase, IAsyncDisposable
{
    /// <summary>
    /// The id for the sheet container element, used for accessibility and styling purposes.
    /// </summary>
    public string ElementId { get; private set; } = Identifier.Create("sheet-");
    internal bool _dragging;
    private bool _isDisposing = false;
    private bool _updateState;

    internal record struct DragPoints(double XDown, double YDown, int StartSize);
    internal DragPoints? _points;
    internal DateTime _lastPointerMove = DateTime.MinValue;
    internal readonly TimeSpan PointerMoveThrottle = TimeSpan.FromMilliseconds(16);

    internal record struct ViewPortSize(double Width, double Height);
    internal ViewPortSize? _viewportSize;

    internal bool JSRuntimeReady => !_isDisposing && IsJSRuntimeAvailable;
    internal ElementReference _handleRef = default!;

    private ParameterState<bool> _openSheetState;
    private ParameterState<int> _currentSizeState;

    /// <summary>
    /// Initializes a new instance of the <see cref="MudSheet"/> class.
    /// </summary>
    public MudSheet()
    {
        using var registerScope = CreateRegisterScope();
        _openSheetState = registerScope.RegisterParameter<bool>(nameof(Open))
            .WithParameter(() => Open)
            .WithEventCallback(() => OpenChanged)
            .WithChangeHandler(OnOpenChanged);

        _currentSizeState = registerScope.RegisterParameter<int>(nameof(CurrentSize))
            .WithParameter(() => CurrentSize)
            .WithEventCallback(() => CurrentSizeChanged)
            .WithChangeHandler(OnCurrentSizeChanged);
    }

    /// <summary>
    /// Gets the CSS class names for the sheet element based on its state and configuration.
    /// </summary>
    protected string Classname =>
        new CssBuilder("mud-sheet-container")
            .AddClass($"mud-sheet-position-{Positioning}")
            .AddClass($"mud-sheet-borderradius-{BorderRadius}", BorderRadius != null)
            .AddClass($"mud-elevation-{Elevation}", !_dragging && Elevation > 0)
            .AddClass("mud-sheet-dragging", _dragging)
            .AddClass(Class)
            .Build();

    /// <summary>
    /// Gets the computed styles for the sheet element based on the current style configuration.
    /// </summary>
    protected string Stylename =>
        new StyleBuilder()
            .AddStyle(Style, !string.IsNullOrEmpty(Style))
            .Build();

    /// <summary>
    /// Gets the CSS class name for the <see cref="MudPopover"/> element, including position-specific styling.
    /// </summary>
    protected string PopoverClassname =>
        new CssBuilder("mud-sheet-popover")
            .AddClass($"mud-sheet-position-{Positioning}")
            .AddClass("mud-popover-fixed")
            .AddClass($"mud-sheet-cover-appbar",
                // If value has been set then use it
                // else use M3 specifications (Modal covers)
                CoverAppBar ?? !Standard)
            .Build();

    /// <summary>
    /// Gets the computed styles for the popover element based on the current style configuration.
    /// </summary>
    protected string PopoverStylename =>
        new StyleBuilder()
            .AddStyle("width", $"{_currentSizeState.Value}vw", Positioning is "left" or "right" or "center")
            .AddStyle("height", $"{_currentSizeState.Value}vh", Positioning is "top" or "bottom" or "center")
            .AddStyle(Style, !string.IsNullOrEmpty(Style))
            .Build();

    /// <summary>
    /// Gets the origin point based on the current position and layout direction.
    /// </summary>
    /// <remarks>The returned origin is determined by the <see cref="Position"/> property and, if
    /// applicable,  the <see cref="RightToLeft"/> layout setting. This ensures the origin aligns correctly with 
    /// the specified position and text direction resulting in a BottomSheet, or SideSheet.</remarks>
    protected Origin Origin =>
        Positioning switch
        {
            "bottom" => Origin.BottomCenter,
            "left" => Origin.CenterLeft,
            "right" => Origin.CenterRight,
            "top" => Origin.TopCenter,
            _ => Origin.CenterCenter
        };

    /// <summary>
    /// JSRuntime to use for swipe effects
    /// </summary>
    [Inject]
    IJSRuntime JSRuntime { get; set; } = default!;

    /// <summary>
    /// Gets a cascading value indicating whether the layout and text direction are rendered in a right-to-left format.
    /// </summary>
    [CascadingParameter(Name = "RightToLeft")]
    [Category(CategoryTypes.Sheet.Appearance)]
    public bool RightToLeft { get; set; }

    /// <summary>
    /// Displays content within a <see cref="MudPaper"/>.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.<br/>
    /// When true it spans 100% width, 100% height, and overflow: auto
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Popover.Appearance)]
    public bool Paper { get; set; } = true;

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
    /// The border radius of the sheet. Does not apply to the connecting edge.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>16</c>.<br/>
    /// Can be set to <c>null</c> to default to MudTheme border radius.<br/>
    /// Any value above 24 is ignored and set to <c>null</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Sheet.Appearance)]
    public int? BorderRadius { get; set; } = 16;

    /// <summary>
    /// Whether the sheet should visually cover the AppBar.
    /// </summary>
    /// <remarks>
    /// Default is <c> null</c>, behavior follows Material defaults:<br/>
    /// - Standard: false, Appbar is never covered and clicking outside does not close the sheet.<br/>
    /// - Modal: true, Appbar is covered by default and clicking outside the sheet closes it.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Sheet.Behavior)]
    public bool? CoverAppBar { get; set; }

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
    [Category(CategoryTypes.Sheet.Behavior)]
    public Position Position { get; set; } = Position.Bottom;

    /// <summary>
    /// Gets or sets a value indicating whether the sheet is open or closed.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Sheet.Behavior)]
    public bool Open { get; set; }

    /// <summary>
    /// The callback that is invoked when the open state of the sheet changes.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Sheet.Behavior)]
    public EventCallback<bool> OpenChanged { get; set; }

    /// <summary>
    /// The Current Size of the sheet as a percentage of the viewport height (vh) or width (vw).
    /// </summary>
    /// <remarks>
    /// Defaults to 50% of the viewport height or width, depending on the position.<br/>
    /// If the sheet is closed and reopened, it will retain its last size. 
    /// </remarks>
    /// <para>Typically Side Sheets should only start at 25%</para>
    [Parameter]
    [Category(CategoryTypes.Sheet.Behavior)]
    public int CurrentSize { get; set; } = 50;

    /// <summary>
    /// The callback that is invoked when the current size of the sheet changes.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Sheet.Behavior)]
    public EventCallback<int> CurrentSizeChanged { get; set; }

    /// <summary>
    /// The callback that is invoked when the bottom sheet is closed.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Sheet.Behavior)]
    public EventCallback OnDismissed { get; set; }

    /// <summary>
    /// The content to be rendered inside the content area of the sheet.
    /// </summary>
    [Parameter, EditorRequired]
    [Category(CategoryTypes.Sheet.Behavior)]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The content to be rendered inside the mud-sheet-handle area of the sheet.
    /// </summary>
    /// <remarks>
    /// Defaults to:
    /// <c>@&lt;MudIconButton Icon=&quot;@DragHandle&quot; Class=&quot;mud-sheet-handle-button&quot; OnClick=&quot;@ToggleSizeAsync&quot; @onpointerdown:stopPropagation aria-controls="@ElementId" /&gt;</c>
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Sheet.Behavior)]
    public RenderFragment? SheetHandleFragment { get; set; }

    /// <summary>
    /// ARIA label on the sheet container for accessibility.
    /// </summary>
    /// <remarks>
    /// If not overridden, defaults to a string that includes the position of the sheet. 
    /// e.g. Bottom Sheet, Left Sheet, etc.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Sheet.Appearance)]
    public string? AriaLabel { get; set; }

    /// <summary>
    /// Whether this is a persistent (standard) sheet. If false, it is shown as a modal.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// <para>
    /// Standard sheets are persistent and do not close when clicking outside of them.<br/>
    /// Non Standard sheets behaves like a modal, closing when clicking outside of it.<br/>
    /// </para>
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Sheet.Behavior)]
    public bool Standard { get; set; } = true;

    /// <summary>
    /// A value indicating whether the user can resize the sheet by dragging.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.<br/>
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Sheet.Behavior)]
    public bool EnableDragToSize { get; set; } = true;

    /// <summary>
    /// List of snap point heights (in viewport height or width) to toggle or drag
    /// </summary>
    /// <remarks>
    /// Defaults to <c>[20, 40, 50, 70, 90, 100]</c>.<br/>
    /// Valid values are between 10 and 100, inclusive.<br/>
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Sheet.Appearance)]
    public int[] PresetSizes { get; set; } = [25, 50, 75, 100];

    /// <summary>
    /// Indicates whether the drag methods can roam outside of <see cref="PresetSizes"/> or not.
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
    /// Determines if a sheet should close when the Escape key is pressed while 
    /// anywhere inside the container has focus.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Sheet.Behavior)]
    public bool CloseOnEscapeKey { get; set; } = true;

    /// <summary>
    /// Dictionary for ARIA attributes for accessibility. You can override any of the attributes by setting a matching 
    /// key in <see cref="MudComponentBase.UserAttributes" />
    /// </summary>
    /// <remarks>
    /// Standard (Non-Modal) sheet containers will have the following attributes:
    /// <para>
    /// role="<c>region</c>"<br/>
    /// tabindex="-1"
    /// </para>
    /// Modal sheets will have the following attributes:
    /// <para>
    /// role="dialog"<br/>
    /// aria-modal="true"<br/>
    /// tabindex="-1"
    /// </para>
    /// </remarks>
    public Dictionary<string, object?> AriaAttributes { get; private set; } = [];

    /// <summary>
    /// Return a combined list of attributes used on the MudSheet Container element.
    /// </summary>
    /// <returns></returns>
    internal Dictionary<string, object?> UpdatedAttributes
    {
        get
        {
            var ariaLabel = $"{CultureInfo.InvariantCulture.TextInfo.ToTitleCase(Positioning)} Sheet";
            // Update AriaAttributes based on the sheet's properties
            AriaAttributes.Clear();
            // Ensure the container is outside of the tab order
            AriaAttributes.Add("tabindex", -1);
            AriaAttributes["aria-label"] = AriaLabel ?? ariaLabel;
            if (Standard)
            {
                // sets region role for standard sheets
                AriaAttributes["role"] = "region";
            }
            else
            {
                // lets screen readers know this is a modal dialog and must be interacted with or dismissed
                AriaAttributes["role"] = "dialog";
                AriaAttributes["aria-modal"] = "true";
            }
            var mergedAttributes = new Dictionary<string, object?>(AriaAttributes);
            foreach (var attr in base.UserAttributes)
            {
                mergedAttributes[attr.Key] = attr.Value;
            }
            return mergedAttributes;
        }
    }

    /// <summary>
    /// Returns the Current Drag Handle Icon based on the Position of the sheet.
    /// </summary>
    protected string DragHandle => Positioning is not "left" or "right" ? VerticalHandle : HorizontalHandle;

    /// <summary>
    /// Gets a value indicating whether dragging is currently allowed.
    /// </summary>
    private bool CanDrag => _dragging && _openSheetState.Value && _points is not null && _viewportSize is not null;

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
    /// Opens the sheet if it is not already open.
    /// </summary>
    /// <remarks>
    /// If the sheet is already open, this method does nothing. Otherwise, it sets the
    /// sheet's state to open and triggers the <see cref="OpenChanged"/> event with a value of <see
    /// langword="true"/>.
    /// </remarks>
    public Task OpenSheetAsync()
    {
        return OpenSheetAsync(updateState: true);
    }

    /// <summary>
    /// Internal method to open the sheet asynchronously without updating the state.
    /// </summary>
    /// <param name="updateState">Whether to call StateHasChanged after opening the sheet</param>
    private async Task OpenSheetAsync(bool updateState)
    {
        var open = _openSheetState.Value;
        if (!open)
        {
            // calling the open event shouldn't trigger a callback if open did not change
            await _openSheetState.SetValueAsync(true);
            await OpenChanged.InvokeAsync(true);
            if (updateState)
            {
                StateHasChanged();
            }
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
            StateHasChanged();
        }
    }

    /// <summary>
    /// Selects the next larger size from the <see cref="PresetSizes"/> array based on the current sheet size.
    /// </summary>
    /// <remarks>
    /// If there are no preset sizes defined, or if the current size is already the largest preset size,
    /// the sheet will be closed.
    /// </remarks>
    public async Task ToggleSizeAsync()
    {
        if (PresetSizes is null || PresetSizes.Length == 0)
        {
            await CloseSheetAsync();
            return;
        }

        var nearestPresetIndex = Array.FindIndex(PresetSizes, s => s > _currentSizeState.Value);
        if (nearestPresetIndex == -1)
        {
            // If no larger preset size is found, close the sheet
            // and start over
            nearestPresetIndex = 0;
            await CloseSheetAsync();
        }
        await ChangeSize(PresetSizes[nearestPresetIndex]);
    }

    /// <summary>
    /// Changes the current size to the specified value.
    /// </summary>
    /// <param name="newSize">The new size value. Must be between 0 and 100.</param>
    internal async Task ChangeSize(int newSize)
    {
        Math.Clamp(newSize, 0, 100);
        await _currentSizeState.SetValueAsync(newSize);
        await CurrentSizeChanged.InvokeAsync(newSize);
    }

    /// <summary>
    /// Handles the key down event for when the sheet has focus.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    private Task HandleKeyDownAsync(KeyboardEventArgs args)
    {
        // If the Escape key is pressed, close the sheet
        if (args.Key == "Escape" && _openSheetState.Value && CloseOnEscapeKey)
        {
            return CloseSheetAsync();
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Handles what happens when the pointer is pressed down on the sheet handle area.
    /// </summary>
    /// <remarks>
    /// Start DragEvent, Set Starting Point, Javascript setPointerCapture
    /// </remarks>
    private async Task HandlePointerDownAsync(PointerEventArgs args)
    {
        // If the sheet is open, we set the dragging state to true
        if (_openSheetState.Value)
        {
            await SetDraggingState(true, args);
            // set starting point after extending pointer capture and starting drag
            _points = new DragPoints(args.ClientX, args.ClientY, _currentSizeState.Value);
        }
    }

    /// <summary>
    /// Handles what happens when the pointer is released on the sheet handle area.
    /// </summary>
    /// <remarks>
    /// Stop Drag, Adjust Snap Position to nearest, Javascript releasePointerCapture
    /// Optional add a sensitivity setting
    /// </remarks>
    private async Task HandlePointerUpAsync(PointerEventArgs args)
    {
        // perform a final "move" checking original points
        if (!CanDrag)
        {
            // If not dragging or points are not set, we do nothing
            return;
        }

        await PerformPointerDrag(_points!.Value.XDown, _points!.Value.YDown,
            args.ClientX, args.ClientY, _points!.Value.StartSize);

        // if SnapMode is enabled, adjust the size to the nearest preset size
        if (SnapMode)
        {
            var nearestPresetIndex = PresetSizes
                .Select((size, index) =>
                    new { size, index, diff = Math.Abs(size - _currentSizeState.Value) })
                .OrderBy(x => x.diff)
                .ThenByDescending(x => x.size) // Bias towards larger match if equal diff
                .First().index;
            await ChangeSize(PresetSizes[nearestPresetIndex]);
        }
        // Stop Dragging Events
        await SetDraggingState(false, args);
    }

    /// <summary>
    /// Updates the size of the sheet based on pointer movement while dragging.
    /// </summary>
    /// <remarks>
    /// Moving the pointer while dragging 
    /// - Inside Container via onpointermove or 
    /// - Outside Container via setPointerCapture 
    /// Update points if dragging
    /// </remarks>
    private Task HandlePointerMoveAsync(PointerEventArgs args)
    {
        // If the sheet is open and dragging and points set, we handle the pointer move
        if (!CanDrag) return Task.CompletedTask;

        // Throttle pointer move events to avoid excessive updates
        var now = DateTime.UtcNow;
        if (now - _lastPointerMove < PointerMoveThrottle)
            return Task.CompletedTask;

        _lastPointerMove = now;

        return PerformPointerDrag(_points!.Value.XDown, _points!.Value.YDown,
            args.ClientX, args.ClientY, _points!.Value.StartSize);
    }

    /// <summary>
    /// Handles what happens when the pointer is cancelled by the browser
    /// </summary>
    /// <remarks>
    /// Explicit releasePointerCapture
    /// </remarks>
    private Task HandlePointerCancelAsync(PointerEventArgs args)
    {
        return SetDraggingState(false, args);
    }

    /// <summary>
    /// Sets the dragging state of the sheet.
    /// </summary>
    /// <param name="isDragging">A boolean indicating whether the sheet is in a dragging operation.</param>
    /// <param name="args">The pointer event arguments containing details about the pointer interaction.</param>
    private async Task SetDraggingState(bool isDragging, PointerEventArgs args)
    {
        if (!EnableDragToSize)
        {
            // let the click focus
            await _handleRef.FocusAsync();
            return;
        }

        _dragging = isDragging;
        // Reset the swipe deltas when starting or stopping dragging
        _points = null;
        _viewportSize = null;
        if (JSRuntimeReady)
        {
            // Notify JS that dragging has started or stopped
            if (_dragging)
            {
                var sizeArray = await JSRuntime.InvokeAsync<double[]>("window.mudsheetHelper.startDrag", _handleRef, args.PointerId);
                if (sizeArray is not { Length: 2 })
                {
                    throw new InvalidOperationException("JSInterop did not return the expected MudSheet size array.");
                }
                _viewportSize = new ViewPortSize(sizeArray[0], sizeArray[1]);
            }
            else
            {
                await JSRuntime.InvokeVoidAsync("window.mudsheetHelper.cancelDrag", _handleRef, args.PointerId);
            }
        }
    }

    /// <summary>
    /// Set's the new size during a drag operation based on the pointer movement.
    /// </summary>
    /// <param name="baseSize">Size of the sheet at the starting point.</param>
    /// <param name="startX">x coordinate of the starting point.</param>
    /// <param name="startY">y coordinate of the starting point.</param>
    /// <param name="currentX">x coordinate of the current point.</param>
    /// <param name="currentY">y coordinate of the current point.</param>
    private Task PerformPointerDrag(double startX, double startY, double currentX, double currentY, int baseSize)
    {
        // Get pixel movement in the appropriate direction
        var delta = Positioning switch
        {
            "top" => currentY - startY,
            "bottom" or "center" => startY - currentY,
            "left" => currentX - startX,
            "right" => startX - currentX,
            _ => 0
        };

        // Get the relevant viewport dimension
        var viewportPixels = Positioning switch
        {
            "top" or "bottom" => _viewportSize!.Value.Height,
            "left" or "right" => _viewportSize!.Value.Width,
            "center" => _viewportSize!.Value.Height / 2,
            _ => 1
        };

        // Convert pixel movement into percentage of viewport
        var newSize = baseSize + (delta / viewportPixels * 100);

        return ChangeSize((int)newSize);
    }

    /// <summary>
    /// Fires when the open state of the sheet changes from outside of the component.
    /// </summary>
    private Task OnOpenChanged(ParameterChangedEventArgs<bool> args)
    {
        if (args.Value)
        {
            return OpenSheetAsync(updateState: false);
        }
        return CloseSheetAsync();
    }

    /// <summary>
    /// Fires when the current size of the sheet changes from outside of the component.
    /// </summary>
    private Task OnCurrentSizeChanged(ParameterChangedEventArgs<int> args)
    {
        return ChangeSize(args.Value);
    }

    /// <summary>
    /// Override OnAfterRender
    /// </summary>
    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        if (_updateState)
        {
            // If the state has been updated, we need to re-render the component
            _updateState = false;
            StateHasChanged();
        }
    }

    /// <summary>
    /// Override the setting of Parameters
    /// </summary>
    public override async Task SetParametersAsync(ParameterView parameters)
    {
        // var currentSize is true if the parameters contain a value
        var currentSize = parameters.TryGetValue<int>(nameof(CurrentSize), out var newCurrentSize);
        // if it's different from the current state and out of range clamp it
        if (currentSize && newCurrentSize != _currentSizeState.Value &&
                (newCurrentSize < 0 || newCurrentSize > 100))
        {
            newCurrentSize = Math.Clamp(newCurrentSize, 0, 100);
            await _currentSizeState.SetValueAsync(newCurrentSize);
            _updateState = true;
        }

        _updateState = AriaAttributes.Count == 0;

        // Check Aria attributes for override
        AriaAttributes.TryGetValue("aria-label", out var ariaLabelold);
        _updateState = _updateState ||
            (parameters.TryGetValue<string>(nameof(AriaLabel), out var newAriaLabel) &&
                newAriaLabel != (string?)ariaLabelold);

        // Position updated?
        _updateState = _updateState ||
            (parameters.TryGetValue<Position>(nameof(Position), out var newPosition) &&
                newPosition != Position);

        // Standard updated?
        _updateState = _updateState ||
            (parameters.TryGetValue<bool>(nameof(Standard), out var newStandard) &&
                newStandard != Standard);

        // have any other parameters changed that require a re-render?
        foreach (var param in parameters)
        {
            if (_updateState)
            {
                break;
            }
            if (param.Name is nameof(Open) or nameof(CurrentSize) or nameof(AriaLabel))
            {
                // Parameters Open and CurrentSize are handled by the ParameterState
                // AriaLabel is handled by above
                continue;
            }
            else
            {
                _updateState = true;
            }
        }

        await base.SetParametersAsync(parameters);
    }

    /// <summary>
    /// Override DisposeAsync
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (JSRuntimeReady && _dragging)
        {
            _isDisposing = true;
            // Notify JS that the component is being disposed and to stop a drag
            await JSRuntime.InvokeVoidAsync("window.mudsheetHelper.cancelDrag", _handleRef);
        }
        // Dispose of any resources if necessary
        IsJSRuntimeAvailable = false;
        GC.SuppressFinalize(this);
    }
}
