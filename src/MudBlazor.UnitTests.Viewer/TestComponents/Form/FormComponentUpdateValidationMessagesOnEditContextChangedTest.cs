// Copyright (c) MudBlazor 2023
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace MudBlazor.UnitTests.TestComponents.Form
{
    public class FormComponentUpdateValidationMessagesModel
    {
        public string Name;
    }

    public class FormComponentUpdateValidationMessagesValidator : ComponentBase
    {
        private ValidationMessageStore messageStore;

        [CascadingParameter]
        private EditContext CurrentEditContext { get; set; }

        protected override void OnInitialized()
        {
            messageStore = new(CurrentEditContext);
        }

        public void AddError(string @field, string error)
        {
            messageStore.Add(CurrentEditContext.Field(@field), error);
            CurrentEditContext?.NotifyValidationStateChanged();
        }

        public void ClearErrors()
        {
            messageStore?.Clear();
            CurrentEditContext?.NotifyValidationStateChanged();
        }
    }
}
