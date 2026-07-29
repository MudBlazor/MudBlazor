// Copyright (c) 2019 Blazored (https://github.com/Blazored)
// See https://github.com/Blazored
// License: MIT
// Copyright (c) 2020 Adapted by MudBlazor

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace MudBlazor
{
    /// <summary>
    /// Service used to create, show, and close MudBlazor dialogs.
    /// </summary>
    /// <remarks>
    /// Register this service and include a <see cref="MudDialogProvider"/> in your layout so dialogs can render and respond to close requests.
    /// </remarks>
    /// <seealso cref="MudDialog"/>
    /// <seealso cref="MudDialogContainer"/>
    /// <seealso cref="MudDialogProvider"/>
    /// <seealso cref="DialogOptions"/>
    /// <seealso cref="DialogParameters{T}"/>
    /// <seealso cref="DialogReference"/>
    public class DialogService : IDialogService
    {
        /// <summary>
        /// Internal wrapper component that prevents overwriting parameters on existing dialog instances.
        /// </summary>
        /// <remarks>
        /// This keeps dialog content stable while the parent fragment re-renders.
        /// See: https://github.com/MudBlazor/MudBlazor/issues/10659#issuecomment-2602911059
        /// </remarks>
        private sealed class DialogHelperComponent : IComponent
        {
            private const string ChildContent = nameof(ChildContent);
            private RenderFragment? _renderFragment;
            private RenderHandle _renderHandle;
            void IComponent.Attach(RenderHandle renderHandle) => _renderHandle = renderHandle;

            Task IComponent.SetParametersAsync(ParameterView parameters)
            {
                if (_renderFragment is null && parameters.TryGetValue<RenderFragment>(ChildContent, out var renderFragment))
                {
                    _renderFragment = renderFragment;
                    _renderHandle.Render(_renderFragment);
                }

                return Task.CompletedTask;
            }

            public static RenderFragment Wrap(RenderFragment renderFragment)
                => builder =>
                {
                    builder.OpenComponent<DialogHelperComponent>(1);
                    builder.AddAttribute(2, ChildContent, renderFragment);
                    builder.CloseComponent();
                };
        }

        internal const string MissingProviderMessage =
            "Missing <MudDialogProvider /> in the active render scope, so dialogs cannot be displayed. " +
            "Add <MudDialogProvider /> within the same interactive render mode as the components that open dialogs: in your layout for global interactivity, or on each page for per-page interactivity. " +
            "See https://mudblazor.com/getting-started/installation#manual-install-add-components";

        private readonly ILogger<DialogService> _logger;
        private bool _missingProviderLogged;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogService"/> class.
        /// </summary>
        /// <remarks>
        /// Declared explicitly rather than as an optional parameter on the logger constructor to preserve source compatibility.
        /// </remarks>
        public DialogService() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogService"/> class.
        /// </summary>
        /// <param name="logger">The logger used to surface configuration problems such as a missing provider.</param>
        public DialogService(ILogger<DialogService>? logger)
        {
            _logger = logger ?? NullLogger<DialogService>.Instance;
        }

        /// <inheritdoc />
        public event Func<IDialogReference, Task>? DialogInstanceAddedAsync;

        /// <inheritdoc />
        public event Action<IDialogReference, DialogResult?>? OnDialogCloseRequested;

        /// <inheritdoc />
        public Task<IDialogReference> ShowAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>() where T : IComponent
        {
            return ShowAsync<T>(string.Empty, DialogParameters.Default, DialogOptions.Default);
        }

        /// <inheritdoc />
        public Task<IDialogReference> ShowAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(string? title) where T : IComponent
        {
            return ShowAsync<T>(title, DialogParameters.Default, DialogOptions.Default);
        }

        /// <inheritdoc />
        public Task<IDialogReference> ShowAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(string? title, DialogOptions options) where T : IComponent
        {
            return ShowAsync<T>(title, DialogParameters.Default, options);
        }

        /// <inheritdoc />
        public Task<IDialogReference> ShowAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(DialogOptions options) where T : IComponent
        {
            return ShowAsync<T>(string.Empty, DialogParameters.Default, options);
        }

        /// <inheritdoc />
        public Task<IDialogReference> ShowAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(DialogParameters parameters) where T : IComponent
        {
            return ShowAsync<T>(string.Empty, parameters, DialogOptions.Default);
        }

        /// <inheritdoc />
        public Task<IDialogReference> ShowAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(string? title, DialogParameters parameters) where T : IComponent
        {
            return ShowAsync<T>(title, parameters, DialogOptions.Default);
        }

        /// <inheritdoc />
        public Task<IDialogReference> ShowAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(string? title, DialogParameters parameters,
            DialogOptions? options) where T : IComponent
        {
            return ShowAsync(typeof(T), title, parameters, options ?? DialogOptions.Default);
        }

        /// <inheritdoc />
        public Task<IDialogReference> ShowAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(DialogParameters parameters, DialogOptions options) where T : IComponent
        {
            return ShowAsync<T>(string.Empty, parameters, options);
        }

        /// <inheritdoc />
        public Task<IDialogReference> ShowAsync([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type component)
        {
            return ShowAsync(component, string.Empty, DialogParameters.Default, DialogOptions.Default);
        }

        /// <inheritdoc />
        public Task<IDialogReference> ShowAsync([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type component, string? title)
        {
            return ShowAsync(component, title, DialogParameters.Default, DialogOptions.Default);
        }

        /// <inheritdoc />
        public Task<IDialogReference> ShowAsync([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type component, string? title, DialogOptions options)
        {
            return ShowAsync(component, title, DialogParameters.Default, options);
        }

        /// <inheritdoc />
        public Task<IDialogReference> ShowAsync([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type component, string? title, DialogParameters parameters)
        {
            return ShowAsync(component, title, parameters, DialogOptions.Default);
        }

        /// <inheritdoc />
        public async Task<IDialogReference> ShowAsync([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type component, string? title,
            DialogParameters parameters, DialogOptions options)
        {
            var dialogReference = await ShowCoreAsync(component, title, parameters, options);

            //Do not wait forever, what if render fails because of some internal exception and we will never release the method.
            var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            var token = cancellationTokenSource.Token;
            await using (token.Register(() => dialogReference.RenderCompleteTaskCompletionSource.TrySetResult(false)))
            {
                await dialogReference.RenderCompleteTaskCompletionSource.Task;

                return dialogReference;
            }
        }

        /// <inheritdoc />
        public Task<bool?> ShowMessageBoxAsync(string? title, string message, string yesText = "OK",
            string? noText = null, string? cancelText = null, DialogOptions? options = null)
        {
            return ShowMessageBoxAsync(new MessageBoxOptions
            {
                Title = title,
                Message = message,
                YesText = yesText,
                NoText = noText,
                CancelText = cancelText,
            }, options);
        }

        /// <inheritdoc />
        public Task<bool?> ShowMessageBoxAsync(string? title, MarkupString markupMessage, string yesText = "OK",
            string? noText = null, string? cancelText = null, DialogOptions? options = null)
        {
            return ShowMessageBoxAsync(new MessageBoxOptions
            {
                Title = title,
                MarkupMessage = markupMessage,
                YesText = yesText,
                NoText = noText,
                CancelText = cancelText,
            }, options);
        }

        /// <inheritdoc />
        public async Task<bool?> ShowMessageBoxAsync(MessageBoxOptions messageBoxOptions, DialogOptions? options = null)
        {
            var parameters = new DialogParameters
            {
                [nameof(MessageBoxOptions.Title)] = messageBoxOptions.Title,
                [nameof(MessageBoxOptions.Message)] = messageBoxOptions.Message,
                [nameof(MessageBoxOptions.MarkupMessage)] = messageBoxOptions.MarkupMessage,
                [nameof(MessageBoxOptions.CancelText)] = messageBoxOptions.CancelText,
                [nameof(MessageBoxOptions.NoText)] = messageBoxOptions.NoText,
                [nameof(MessageBoxOptions.YesText)] = messageBoxOptions.YesText,
            };
            var reference = await ShowAsync<MudMessageBox>(title: messageBoxOptions.Title, parameters: parameters, options: options);
            var result = await reference.Result;

            if (result is null)
            {
                return null;
            }

            if (result.Canceled || result.Data is not bool data)
            {
                return null;
            }

            return data;
        }

        /// <inheritdoc />
        public void Close(IDialogReference dialog)
        {
            Close(dialog, DialogResult.Ok<object?>(null));
        }

        /// <inheritdoc />
        public virtual void Close(IDialogReference dialog, DialogResult? result)
        {
            OnDialogCloseRequested?.Invoke(dialog, result);
        }

        /// <inheritdoc />
        public virtual IDialogReference CreateReference()
        {
            return new DialogReference(Guid.NewGuid(), this);
        }

        private async Task<IDialogReference> ShowCoreAsync([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contentComponent, string? title, DialogParameters parameters,
            DialogOptions options)
        {
            if (!typeof(IComponent).IsAssignableFrom(contentComponent))
            {
                throw new ArgumentException($"{contentComponent.FullName} must be a Blazor IComponent");
            }

            var dialogReference = CreateReference();
            dialogReference.InjectOptions(options);
            var dialogContent = DialogHelperComponent.Wrap(builder =>
            {
                var i = 0;
                builder.OpenComponent(i++, contentComponent);
                foreach (var parameter in parameters)
                {
                    builder.AddAttribute(i++, parameter.Key, parameter.Value);
                }

                builder.AddComponentReferenceCapture(i, inst => { dialogReference.InjectDialog(inst); });
                builder.CloseComponent();
            });
            var dialogInstance = new RenderFragment(builder =>
            {
                builder.OpenComponent<MudDialogContainer>(0);
                builder.SetKey(dialogReference.Id);
                builder.AddComponentParameter(1, nameof(MudDialogContainer.Options), options);
                builder.AddComponentParameter(2, nameof(MudDialogContainer.Title), title);
                builder.AddComponentParameter(3, nameof(MudDialogContainer.Content), dialogContent);
                builder.AddComponentParameter(4, nameof(MudDialogContainer.Id), dialogReference.Id);
                builder.CloseComponent();
            });
            dialogReference.InjectRenderFragment(dialogInstance);

            var dialogInstanceAddedAsync = DialogInstanceAddedAsync;
            if (dialogInstanceAddedAsync is not null)
            {
                await dialogInstanceAddedAsync(dialogReference);
            }
            else if (!_missingProviderLogged)
            {
                // No MudDialogProvider is subscribed, so this dialog will never render (ShowAsync then blocks until its render timeout).
                // Log once with actionable guidance instead of failing silently.
                _missingProviderLogged = true;
                _logger.LogError(MissingProviderMessage);
            }

            return dialogReference;
        }
    }
}
