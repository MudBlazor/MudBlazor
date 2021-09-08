namespace MudBlazor
{
    /// <summary>
    /// Breakpoints describe certain user interfaces sizes or ranges. Use them in conjunction with MudHidden or ResizeListenerService
    /// </summary>
    public enum Breakpoint
    {
        Xs, Sm, Md, Lg, Xl,
        SmAndDown, MdAndDown, LgAndDown,
        SmAndUp, MdAndUp, LgAndUp,
        None,
        Always
    }
}
