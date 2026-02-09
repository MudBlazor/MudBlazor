using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using MudBlazor.Resources;

namespace MudBlazor;

/// <summary>
/// Shows a confirmation dialog when the user tries to navigate away.
/// </summary>
/// <remarks>
/// Due to browser restrictions the native browser exit prompt has to be used on browser navigation.
/// </remarks>
public partial class MudExitPrompt : MudComponentBase, IAsyncDisposable
{
    private bool _navigatedAway;
    private IDisposable? _locationChangingRegistration;

    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    [Inject]
    private IDialogService DialogService { get; set; } = null!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    private InternalMudLocalizer Localizer { get; set; } = null!;

    /// <summary>
    /// The title of the message box to show on exit.
    /// </summary>
    /// <remarks>
    /// Defaults to the localized version of <i>"Confirm navigation"</i>.
    /// </remarks>
    [Parameter, Category(CategoryTypes.ExitPrompt.Appearance)]
    public string? Title { get; set; }

    /// <summary>
    /// The text to show on exit.
    /// </summary>
    /// <remarks>
    /// Defaults to the localized version of <i>"Leave site? Changes you made may not be saved."</i>.
    /// </remarks>
    [Parameter, Category(CategoryTypes.ExitPrompt.Appearance)]
    public string? Text { get; set; }

    /// <summary>
    /// Disables the exit prompt.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter, Category(CategoryTypes.ExitPrompt.Behavior)]
    public bool Disabled { get; set; }

    /// <summary>
    /// Uses the browser's native prompt instead of the message box.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter, Category(CategoryTypes.ExitPrompt.Behavior)]
    public bool NativeOnly { get; set; }

    private string TitleToDisplay => Title ?? Localizer[LanguageResource.MudExitPrompt_Title];

    private string TextToDisplay => Text ?? Localizer[LanguageResource.MudExitPrompt_Text];

    public MudExitPrompt()
    {
        using var registerScope = CreateRegisterScope();
        registerScope.RegisterParameter<bool>(nameof(Disabled))
            .WithParameter(() => Disabled)
            .WithChangeHandler(args => !args.Value ? EnableAsync() : DisableAsync());
        registerScope.RegisterParameter<string?>(nameof(Text))
            .WithParameter(() => Text)
            .WithChangeHandler(SetTextAsync);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            _locationChangingRegistration = NavigationManager.RegisterLocationChangingHandler(OnLocationChanging);
            if (!Disabled)
            {
                await EnableAsync();
            }
        }
    }

    private async ValueTask OnLocationChanging(LocationChangingContext context)
    {
        if (Disabled)
        {
            return;
        }

        var allow = NativeOnly
            ? await JsRuntime.InvokeAsync<bool>("mudExitPrompt.handleBeforeNavigation")
            : await DialogService.ShowMessageBoxAsync(
                TitleToDisplay,
                TextToDisplay,
                Localizer[LanguageResource.MudExitPrompt_Exit],
                Localizer[LanguageResource.MudExitPrompt_Cancel]
            ) == true;
        if (!allow)
        {
            context.PreventNavigation();
        }
        else
        {
            _navigatedAway = true;
        }
    }

    private async Task EnableAsync()
    {
        if (!IsJSRuntimeAvailable)
        {
            return;
        }

        await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudExitPrompt.enable", TextToDisplay);
    }

    private async Task SetTextAsync()
    {
        if (!IsJSRuntimeAvailable)
        {
            return;
        }

        await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudExitPrompt.setText", TextToDisplay);
    }

    private async Task DisableAsync()
    {
        if (!IsJSRuntimeAvailable)
        {
            return;
        }

        await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudExitPrompt.disable");
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        _locationChangingRegistration?.Dispose();
        if (!Disabled && _navigatedAway)
        {
            await DisableAsync();
        }
    }
}
