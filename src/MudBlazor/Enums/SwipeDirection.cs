namespace MudBlazor;

/// <summary>
/// The direction of a swipe event for a <see cref="MudSwipeArea"/> component.
/// </summary>
public enum SwipeDirection
{
    /// <summary>
    /// The swipe direction is unknown.
    /// </summary>
    None,

    /// <summary>
    /// The swipe is happening from left to right.
    /// </summary>
    LeftToRight,

    /// <summary>
    /// The swipe is happening from right to left.
    /// </summary>
    RightToLeft,

    /// <summary>
    /// The swipe is happening from top to bottom.
    /// </summary>
    TopToBottom,

    /// <summary>
    /// The swipe is happening from bottom to top.
    /// </summary>
    BottomToTop
}
