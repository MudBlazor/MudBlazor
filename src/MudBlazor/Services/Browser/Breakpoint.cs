namespace MudBlazor;

#nullable enable
/// <summary>
/// Breakpoints describe certain user interfaces sizes or ranges. Use them in conjunction with MudHidden or ResizeListenerService
/// </summary>
public enum Breakpoint
{
    Xs, Sm, Md, Lg, Xl, Xxl,
    SmAndDown, MdAndDown, LgAndDown, XlAndDown,
    SmAndUp, MdAndUp, LgAndUp, XlAndUp,
    None,
    Always
}
