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
    private readonly string _promptId = Identifier.Create("exitPrompt");
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
    /// The title shown in the confirmation dialog.
    /// </summary>
    /// <remarks>
    /// Defaults to the localized version of <i>"Confirm navigation"</i>.
    /// </remarks>
    [Parameter, Category(CategoryTypes.ExitPrompt.Appearance)]
    public string? Title { get; set; }

    /// <summary>
    /// The message shown in the confirmation dialog.
    /// </summary>
    /// <remarks>
    /// Defaults to the localized version of <i>"Leave site? Changes you made may not be saved."</i>.
    /// </remarks>
    [Parameter, Category(CategoryTypes.ExitPrompt.Appearance)]
    public string? Text { get; set; }

    /// <summary>
    /// Disables exit prompt protection.
    /// </summary>
    /// <remarks>
    /// When set to <c>true</c>, navigation proceeds without confirmation and JS unload protection is removed.
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter, Category(CategoryTypes.ExitPrompt.Behavior)]
    public bool Disabled { get; set; }

    /// <summary>
    /// Uses the browser's native confirmation prompt for in-app navigation.
    /// </summary>
    /// <remarks>
    /// When <c>false</c>, navigation inside the app uses a MudBlazor message box with <see cref="Title"/> and <see cref="Text"/>.
    /// When <c>true</c>, in-app navigation uses the browser <c>confirm</c> dialog and <see cref="Title"/> is ignored.
    /// Tab close, refresh, and direct URL navigation always use the browser's native unload prompt.
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter, Category(CategoryTypes.ExitPrompt.Behavior)]
    public bool UseNativePrompt { get; set; }

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
            if (!Disabled)
            {
                await EnableAsync();
            }

            _locationChangingRegistration = NavigationManager.RegisterLocationChangingHandler(OnLocationChanging);
        }
    }

    private async ValueTask OnLocationChanging(LocationChangingContext context)
    {
        if (Disabled)
        {
            return;
        }

        if (!await IsNavigationAllowedAsync())
        {
            context.PreventNavigation();
            return;
        }
    }

    private async Task<bool> IsNavigationAllowedAsync()
    {
        // For in-app navigation, optionally use the browser confirm dialog when native prompts are enabled.
        if (UseNativePrompt)
        {
            return await JsRuntime.InvokeAsync<bool>("mudExitPrompt.handleBeforeNavigation", _promptId);
        }

        // Otherwise, use the MudBlazor confirmation dialog and allow navigation only on explicit confirmation.
        return await DialogService.ShowMessageBoxAsync(
            TitleToDisplay,
            TextToDisplay,
            Localizer[LanguageResource.MudExitPrompt_Exit],
            Localizer[LanguageResource.MudExitPrompt_Cancel]
        ) == true;
    }

    private async Task EnableAsync()
    {
        if (!IsJSRuntimeAvailable || Disabled)
        {
            return;
        }

        var success = await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudExitPrompt.enable", _promptId, TextToDisplay);
        if (!success)
        {
            throw new InvalidOperationException($"Failed to enable {nameof(MudExitPrompt)} JS interop for prompt '{_promptId}'.");
        }
    }

    private async Task SetTextAsync()
    {
        if (!IsJSRuntimeAvailable || Disabled || _locationChangingRegistration is null)
        {
            return;
        }

        await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudExitPrompt.setText", _promptId, TextToDisplay);
    }

    private async Task DisableAsync()
    {
        if (!IsJSRuntimeAvailable || _locationChangingRegistration is null)
        {
            return;
        }

        await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudExitPrompt.disable", _promptId);
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
        if (!Disabled)
        {
            await DisableAsync();
        }
    }
}
