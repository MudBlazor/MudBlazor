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
        public event Action<IDialogReference> OnDialogInstanceAdded;
        public event Action<IDialogReference, DialogResult> OnDialogCloseRequested;

        public IDialogReference Show<T>() where T : ComponentBase
        {
            return Show<T>(string.Empty, new DialogParameters(), new DialogOptions());
        }

        public IDialogReference Show<T>(string title) where T : ComponentBase
        {
            return Show<T>(title, new DialogParameters(), new DialogOptions());
        }

        public IDialogReference Show<T>(string title, DialogOptions options) where T : ComponentBase
        {
            return Show<T>(title, new DialogParameters(), options);
        }

        public IDialogReference Show<T>(string title, DialogParameters parameters) where T : ComponentBase
        {
            return Show<T>(title, parameters, new DialogOptions());
        }

        public IDialogReference Show<T>(string title, DialogParameters parameters, DialogOptions options) where T : ComponentBase
        {
            return Show(typeof(T), title, parameters, options);
        }

        public IDialogReference Show(Type contentComponent)
        {
            return Show(contentComponent, string.Empty, new DialogParameters(), new DialogOptions());
        }

        public IDialogReference Show(Type contentComponent, string title)
        {
            return Show(contentComponent, title, new DialogParameters(), new DialogOptions());
        }

        public IDialogReference Show(Type contentComponent, string title, DialogOptions options)
        {
            return Show(contentComponent, title, new DialogParameters(), options);
        }

        public IDialogReference Show(Type contentComponent, string title, DialogParameters parameters)
        {
            return Show(contentComponent, title, parameters, new DialogOptions());
        }

        public IDialogReference Show(Type contentComponent, string title, DialogParameters parameters, DialogOptions options)
        {
            if (!typeof(ComponentBase).IsAssignableFrom(contentComponent))
            {
                throw new ArgumentException($"{contentComponent.FullName} must be a Blazor Component");
            }
            var dialogReference = CreateReference();

            var dialogContent = new RenderFragment(builder =>
            {
                var i = 0;
                builder.OpenComponent(i++, contentComponent);

                if (!dialogReference.AreParametersRendered)
                {
                    foreach (var parameter in parameters)
                    {
                        builder.AddAttribute(i++, parameter.Key, parameter.Value);
                    }

                    dialogReference.AreParametersRendered = true;
                }
                else
                {
                    i += parameters.Count;
                }

                builder.AddComponentReferenceCapture(i++, inst => { dialogReference.InjectDialog(inst); });
                builder.CloseComponent();
            });
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

        public async Task<bool?> ShowMessageBox(MessageBoxOptions mboxOptions, DialogOptions options = null)
        {
            var parameters = new DialogParameters()
            {
                [nameof(MessageBoxOptions.Title)] = mboxOptions.Title,
                [nameof(MessageBoxOptions.Message)] = mboxOptions.Message,
                [nameof(MessageBoxOptions.CancelText)] = mboxOptions.CancelText,
                [nameof(MessageBoxOptions.NoText)] = mboxOptions.NoText,
                [nameof(MessageBoxOptions.YesText)] = mboxOptions.YesText,
            };
            var reference = Show<MudMessageBox>(parameters: parameters, options: options, title: mboxOptions.Title);
            var result = await reference.Result;
            if (result.Cancelled || !(result.Data is bool))
                return null;
            return (bool)result.Data;
        }

        public void Close(DialogReference dialog)
        {
            Close(dialog, DialogResult.Ok<object>(null));
        }

        public void Close(DialogReference dialog, DialogResult result)
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
