using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using MudBlazor.Resources;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Shows a confirmation dialog when the user tries to navigate away.
/// </summary>
public partial class MudExitPrompt : MudComponentBase, IAsyncDisposable
{
    private bool _navigatedAway;

    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = null!;

    [Inject]
    private InternalMudLocalizer Localizer { get; set; } = null!;

    /// <summary>
    /// Disables the navigation check.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter, Category(CategoryTypes.ExitPrompt.Behavior)]
    public bool Disabled { get; set; }

    public MudExitPrompt()
    {
        using var registerScope = CreateRegisterScope();
        registerScope.RegisterParameter<bool>(nameof(Disabled))
            .WithParameter(() => Disabled)
            .WithChangeHandler(args => !args.Value ? EnableAsync() : DisableAsync());
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender && !Disabled)
        {
            await EnableAsync();
            NavigationManager.RegisterLocationChangingHandler(OnLocationChanging);
        }
    }

    private async ValueTask OnLocationChanging(LocationChangingContext context)
    {
        if (Disabled || !IsJSRuntimeAvailable)
        {
            return;
        }

        var allow = await JsRuntime.InvokeAsync<bool>("mudExitPrompt.handleBeforeNavigation");
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

        await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudExitPrompt.enable", Localizer[LanguageResource.MudExitPrompt_Text]);
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
        if (!Disabled && _navigatedAway)
        {
            await DisableAsync();
        }
    }
}
