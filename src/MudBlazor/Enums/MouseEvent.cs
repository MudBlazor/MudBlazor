namespace MudBlazor;

/// <summary>
/// The mouse buttons or events that open a <see cref="MudMenu"/>.
/// </summary>
[Flags]
public enum MouseEvent : long
{
    /// <summary>
    /// No button or uninitialized state.
    /// </summary>
    Auto = 0,

    /// <summary>
    /// Primary button (typically the left button).
    /// </summary>
    LeftClick = 1 << 0, // 1

    /// <summary>
    /// Secondary button (typically the right button).
    /// </summary>
    RightClick = 1 << 1, // 2

    /// <summary>
    /// Hovering over the element.
    /// </summary>
    MouseOver = 1 << 2, // 4

    /// <summary>
    /// Represents any button combination.
    /// </summary>
    Any = LeftClick | RightClick | MouseOver
}
