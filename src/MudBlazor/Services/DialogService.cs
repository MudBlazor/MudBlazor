using Microsoft.AspNetCore.Components;
using System;

namespace MudBlazor.Dialog
{
    public class DialogService : IDialogService
    {
        internal event Action<DialogReference> OnDialogInstanceAdded;
        internal event Action<DialogReference, DialogResult> OnDialogCloseRequested;

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

            var DialogInstanceId = Guid.NewGuid();
            DialogReference DialogReference = null;
            var DialogContent = new RenderFragment(builder =>
            {
                var i = 0;
                builder.OpenComponent(i++, contentComponent);
                foreach (var parameter in parameters._parameters)
                {
                    builder.AddAttribute(i++, parameter.Key, parameter.Value);
                }
                builder.CloseComponent();
            });
            var DialogInstance = new RenderFragment(builder =>
            {
                builder.OpenComponent<MudDialogInstance>(0);
                builder.AddAttribute(1, "Options", options);
                builder.AddAttribute(2, "Title", title);
                builder.AddAttribute(3, "Content", DialogContent);
                builder.AddAttribute(4, "Id", DialogInstanceId);
                builder.CloseComponent();
            });
            DialogReference = new DialogReference(DialogInstanceId, DialogInstance, this);

            OnDialogInstanceAdded?.Invoke(DialogReference);

            return DialogReference;
        }

        internal void Close(DialogReference Dialog)
        {
            Close(Dialog, DialogResult.Ok<object>(null));
        }

        internal void Close(DialogReference Dialog, DialogResult result)
        {
            OnDialogCloseRequested?.Invoke(Dialog, result);
        }
    }
}
