// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;

#nullable enable

/// <summary>
/// A component for aligning child items horizontally or vertically.
/// </summary>
public partial class MudStack : MudComponentBase
{
    protected string Classname =>
        new CssBuilder("d-flex")
            .AddClass(getFlexDirection())
            .AddClass($"justify-{Justify?.ToDescriptionString()}", Justify is not null)
            .AddClass($"align-{AlignItems?.ToDescriptionString()}", AlignItems is not null)
            .AddClass($"flex-{Wrap?.ToDescriptionString()}", Wrap is not null)
            .AddClass($"gap-{Spacing}", Spacing >= 0)
            .AddClass($"flex-grow-{StretchItems?.ToDescriptionString()}", StretchItems is not null and not MudBlazor.StretchItems.None)
            .AddClass(Class)
            .Build();

    /// <summary>
    /// Gets the CSS flex direction based on the current breakpoint and row settings.
    /// </summary>
    /// <remarks>
    /// This property generates a CSS flex direction string based on the following conditions:
    /// - If the breakpoint is not set or is "none" or "always", it returns the default direction based on the Row and Reverse properties.
    /// - If the breakpoint is one of the predefined sizes ("xs", "sm", "md", "lg", "xl", "xxl"), it generates a flex direction string that changes at the specified breakpoint.
    /// - If the breakpoint ends with "anddown", it generates a flex direction string that applies up to and including the specified breakpoint.
    /// - If the breakpoint ends with "andup", it generates a flex direction string that applies from the specified breakpoint and up.
    /// </remarks>
    /// <returns>
    /// A string representing the CSS flex direction based on the current breakpoint and row settings.
    /// </returns>  
    private string getFlexDirection()
    {
        // Sets the inital direction based on the Row Parameter and Appends the reverse if needed
        string defaultState = (Row ? "row" : "column") + (Reverse ? "-reverse" : string.Empty);
        // Sets the reverse direction based on the Row Parameter and Appends the reverse if needed
        string reverseState = (Row ? "column" : "row") + (Reverse ? "-reverse" : string.Empty);

        // Switch statement to determine the breakpoint and return the appropriate flex direction
        switch (Breakpoint)
        {
            // If the Breakpoint is None or Always, return the default direction
            case Breakpoint.None: // If breakpoint is None, return the default direction 
                return $"flex-{defaultState}";
            case Breakpoint.Always: // If breakpoint is Always, return the reverse direction, honestly the user should just use the Row Property
                return $"flex-{reverseState}";
            case Breakpoint.Xs: // Xs is Reverse Direction, Sm and Up is Default Direction
                return $"flex-{reverseState} flex-sm-{defaultState}";
            case Breakpoint.Sm: // Xs is Default Direction, Sm is Reverse Direction, Md and Up is Default Direction
                return $"flex-{defaultState} flex-sm-{reverseState} flex-md-{defaultState}";
            case Breakpoint.Md: // Xs to Sm is Default Direction, Md is Reverse Direction, Lg and Up is Default Direction
                return $"flex-{defaultState} flex-md-{reverseState}  flex-lg-{defaultState}";
            case Breakpoint.Lg: // Xs to Md is Default Direction, Lg is Reverse Direction, Xl and Up is Default Direction
                return $"flex-{defaultState} flex-lg-{reverseState}  flex-xl-{defaultState}";
            case Breakpoint.Xl: // Xs to Lg is Default Direction, Xl is Reverse Direction, Xxl is Default Direction
                return $"flex-{defaultState} flex-xl-{reverseState}  flex-xxl-{defaultState}";
            case Breakpoint.Xxl: // Xs to Xl is Default Direction, Xxl is Reverse Direction
                return $"flex-{defaultState} flex-xxl-{reverseState}";
            case Breakpoint.SmAndDown: // Sm and Down is Reverse Direction, Md and Up is Default Direction
                return $"flex-{reverseState} flex-md-{defaultState}";
            case Breakpoint.MdAndDown: // Md and Down is Reverse Direction, Lg and Up is Default Direction
                return $"flex-{reverseState} flex-lg-{defaultState}";
            case Breakpoint.LgAndDown: // Lg and Down is Reverse Direction, Xl and Up is Default Direction
                return $"flex-{reverseState} flex-xl-{defaultState}";
            case Breakpoint.XlAndDown: // Xl and Down is Reverse Direction, Xxl and Up is Default Direction
                return $"flex-{reverseState} flex-xxl-{defaultState}";
            case Breakpoint.SmAndUp: // Xs is Default Direction, Sm and Up is Reverse Direction
                return $"flex-{defaultState} flex-sm-{reverseState}";
            case Breakpoint.MdAndUp: // Xs to Sm is Default Direction, Md and Up is Reverse Direction
                return $"flex-{defaultState} flex-md-{reverseState}";
            case Breakpoint.LgAndUp: // Xs to Md is Default Direction, Lg and Up is Reverse Direction
                return $"flex-{defaultState} flex-lg-{reverseState}";
            case Breakpoint.XlAndUp: // Xs to Lg is Default Direction, Xl and Up is Reverse Direction
                return $"flex-{defaultState} flex-xl-{reverseState}";
            default: // Return the default direction if no Breakpoint is Matched
                return $"flex-{defaultState}";
        }
    }

    /// <summary>
    /// Displays items horizontally.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.  
    /// When <c>true</c>, items will be displayed horizontally.  When <c>false</c>, items are displayed vertically.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Stack.Behavior)]
    public bool Row { get; set; }

    /// <summary>
    /// Reverses the order of items.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.  
    /// When <c>true</c>, items will be reversed.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Stack.Behavior)]
    public bool Reverse { get; set; }

    /// <summary>
    /// The gap between items in increments of <c>4px</c>.
    /// </summary>
    /// <remarks>
    /// <para>Defaults to 3 in <see cref="MudGlobal.StackDefaults.Spacing"/>.</para>
    /// <para>Maximum is 20 (<c>80px</c>).</para>
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Stack.Behavior)]
    public int Spacing { get; set; } = MudGlobal.StackDefaults.Spacing;

    /// <summary>
    /// Defines the distribution of child items.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Stack.Behavior)]
    public Justify? Justify { get; set; }

    /// <summary>
    /// Defines the vertical alignment of child items.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Stack.Behavior)]
    public AlignItems? AlignItems { get; set; }

    /// <summary>
    /// Defines the stretching behaviour of children along the main axis within a <see cref="MudStack"/> component.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Stack.Behavior)]
    public StretchItems? StretchItems { get; set; }

    /// <summary>
    /// Controls how items are wrapped.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Stack.Behavior)]
    public Wrap? Wrap { get; set; }

    /// <summary>
    /// The breakpoint at which the Stack should switch to a row or column layout.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Stack.Behavior)]
    public Breakpoint Breakpoint { get; set; } = Breakpoint.None;

    /// <summary>
    /// The content within this component.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Stack.Behavior)]
    public RenderFragment? ChildContent { get; set; }
}
