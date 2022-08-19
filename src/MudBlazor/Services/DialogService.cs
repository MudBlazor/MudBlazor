// Copyright (c) 2019 Blazored (https://github.com/Blazored)
// Copyright (c) 2020 Jonny Larsson (https://github.com/MudBlazor/MudBlazor)
// Copyright (c) 2021 improvements by Meinrad Recheis
// See https://github.com/Blazored
// License: MIT

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public class DialogService : IDialogService
    {
        /// <summary>
        /// This internal wrapper components prevents overwriting parameters of once
        /// instanciated dialog instances
        /// </summary>
        private class DialogHelperComponent : IComponent
        {
            const string ChildContent = nameof(ChildContent);
            RenderFragment _renderFragment;
            RenderHandle _renderHandle;
            void IComponent.Attach(RenderHandle renderHandle) => _renderHandle = renderHandle;
            Task IComponent.SetParametersAsync(ParameterView parameters)
            {
                if (_renderFragment == null)
                {
                    if (parameters.TryGetValue(ChildContent, out _renderFragment))
                    {
                        _renderHandle.Render(_renderFragment);
                    }
                }
                return Task.CompletedTask;
            }
            public static RenderFragment Wrap(RenderFragment renderFragment)
                => new RenderFragment(builder =>
                {
                    builder.OpenComponent<DialogHelperComponent>(1);
                    builder.AddAttribute(2, ChildContent, renderFragment);
                    builder.CloseComponent();
                });
        }

        public event Action<IDialogReference> OnDialogInstanceAdded;
        public event Action<IDialogReference, DialogResult> OnDialogCloseRequested;

        public IDialogReference Show<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>() where T : ComponentBase
        {
            return Show<T>(string.Empty, new DialogParameters(), new DialogOptions());
        }

        public IDialogReference Show<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(string title) where T : ComponentBase
        {
            return Show<T>(title, new DialogParameters(), new DialogOptions());
        }

        public IDialogReference Show<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(string title, DialogOptions options) where T : ComponentBase
        {
            return Show<T>(title, new DialogParameters(), options);
        }

        public IDialogReference Show<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(string title, DialogParameters parameters) where T : ComponentBase
        {
            return Show<T>(title, parameters, new DialogOptions());
        }

        public IDialogReference Show<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(string title, DialogParameters parameters, DialogOptions options) where T : ComponentBase
        {
            return Show(typeof(T), title, parameters, options);
        }

        public IDialogReference Show([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contentComponent)
        {
            return Show(contentComponent, string.Empty, new DialogParameters(), new DialogOptions());
        }

        public IDialogReference Show([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contentComponent, string title)
        {
            return Show(contentComponent, title, new DialogParameters(), new DialogOptions());
        }

        public IDialogReference Show([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contentComponent, string title, DialogOptions options)
        {
            return Show(contentComponent, title, new DialogParameters(), options);
        }

        public IDialogReference Show([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contentComponent, string title, DialogParameters parameters)
        {
            return Show(contentComponent, title, parameters, new DialogOptions());
        }

        public IDialogReference Show([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type contentComponent, string title, DialogParameters parameters, DialogOptions options)
        {
            if (!typeof(ComponentBase).IsAssignableFrom(contentComponent))
            {
                throw new ArgumentException($"{contentComponent?.FullName} must be a Blazor Component");
            }
            var dialogReference = CreateReference();

            var dialogContent = DialogHelperComponent.Wrap(new RenderFragment(builder =>
            {
                var i = 0;
                builder.OpenComponent(i++, contentComponent);
                foreach (var parameter in parameters)
                {
                    builder.AddAttribute(i++, parameter.Key, parameter.Value);
                }
                builder.AddComponentReferenceCapture(i++, inst => { dialogReference.InjectDialog(inst); });
                builder.CloseComponent();
            }));
            var dialogInstance = new RenderFragment(builder =>
            {
                builder.OpenComponent<MudDialogInstance>(0);
                builder.SetKey(dialogReference.Id);
                builder.AddAttribute(1, "Options", options);
                builder.AddAttribute(2, "Title", title);
                builder.AddAttribute(3, "Content", dialogContent);
                builder.AddAttribute(4, "Id", dialogReference.Id);
                builder.CloseComponent();
            });
            dialogReference.InjectRenderFragment(dialogInstance);
            OnDialogInstanceAdded?.Invoke(dialogReference);

            return dialogReference;
        }

        public Task<bool?> ShowMessageBox(string title, string message, string yesText = "OK",
            string noText = null, string cancelText = null, DialogOptions options = null)
        {
            return this.ShowMessageBox(new MessageBoxOptions
            {
                Title = title,
                Message = message,
                YesText = yesText,
                NoText = noText,
                CancelText = cancelText,
            }, options);
        }

        public Task<bool?> ShowMessageBox(string title, MarkupString markupMessage, string yesText = "OK",
            string noText = null, string cancelText = null, DialogOptions options = null)
        {
            return this.ShowMessageBox(new MessageBoxOptions
            {
                Title = title,
                MarkupMessage = markupMessage,
                YesText = yesText,
                NoText = noText,
                CancelText = cancelText,
            }, options);
        }

        public async Task<bool?> ShowMessageBox(MessageBoxOptions messageBoxOptions, DialogOptions options = null)
        {
            var parameters = new DialogParameters()
            {
                [nameof(MessageBoxOptions.Title)] = messageBoxOptions.Title,
                [nameof(MessageBoxOptions.Message)] = messageBoxOptions.Message,
                [nameof(MessageBoxOptions.MarkupMessage)] = messageBoxOptions.MarkupMessage,
                [nameof(MessageBoxOptions.CancelText)] = messageBoxOptions.CancelText,
                [nameof(MessageBoxOptions.NoText)] = messageBoxOptions.NoText,
                [nameof(MessageBoxOptions.YesText)] = messageBoxOptions.YesText,
            };
            var reference = Show<MudMessageBox>(parameters: parameters, options: options, title: messageBoxOptions.Title);
            var result = await reference.Result;
            if (result.Cancelled || result.Data is not bool data)
                return null;
            return data;
        }

        public void Close(DialogReference dialog)
        {
            Close(dialog, DialogResult.Ok<object>(null));
        }

        public virtual void Close(DialogReference dialog, DialogResult result)
        {
            OnDialogCloseRequested?.Invoke(dialog, result);
        }

        public virtual IDialogReference CreateReference()
        {
            return new DialogReference(Guid.NewGuid(), this);
        }
    }

    [ExcludeFromCodeCoverage]
    public class MessageBoxOptions
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public MarkupString MarkupMessage { get; set; }
        public string YesText { get; set; } = "OK";
        public string NoText { get; set; }
        public string CancelText { get; set; }
    }

    // MudBlazor.Dialog is obsolete but kept here for backwards compatibility reasons.
    // Don't remove, it will cause massive breakages in user code
    namespace Dialog
    {
        // Inside at least one Class needs to be kept or it will be stripped from assembly
        public class ObsoleteNamespace
        {
            public const string DoNotRemove = "because of backwards compatibility";
        }
    }
}
