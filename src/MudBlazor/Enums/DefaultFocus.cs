namespace MudBlazor;

/// <summary>
/// Indicates the element which will receive focus.
/// </summary>
public enum DefaultFocus
{
    /// <summary>
    /// No focus will occur.
    /// </summary>
    None,

    /// <summary>
    /// This component will receive focus.
    /// </summary>
    /// <remarks>
    /// The user can press the <c>Tab</c> key to move to the next element.
    /// </remarks>
    Element,

    /// <summary>
    /// The first child within this component will receive focus.
    /// </summary>
    FirstChild,

    /// <summary>
    /// The last child within this component will receive focus.
    /// </summary>
    LastChild
}
