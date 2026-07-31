using MudBlazor.Utilities;

namespace MudBlazor;


/// <summary>
/// Arranges the top-level page structure of an app with an app bar, navigation drawer, and main content area.
/// </summary>
/// <remarks>
/// Layouts often contain <see cref="MudAppBar"/> and <see cref="MudDrawer"/> components.  The <see cref="MudMainContent"/> component is used to contain page content.
/// In your layout component, but above this component, add <see cref="MudThemeProvider"/>, <see cref="MudPopoverProvider"/>, <see cref="MudDialogProvider"/>, and <see cref="MudSnackbarProvider"/> components to enable all MudBlazor features.
/// </remarks>
/// <seealso cref="MudAppBar" />
/// <seealso cref="MudDrawer" />
/// <seealso cref="MudMainContent" />
public partial class MudLayout : MudDrawerContainer
{
    protected override string Classname =>
        new CssBuilder("mud-layout")
            .AddClass(base.Classname)
            .Build();

    public MudLayout()
    {
        Fixed = true;
    }
}
