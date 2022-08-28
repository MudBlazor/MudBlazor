// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public partial class MudPopoverProvider : IDisposable
    {
        private bool _isConnectedToService = false;

        [Inject] public IMudPopoverService Service { get; set; }

        /// <summary>
        /// In some scenarios we need more than one ThemeProvider but we must not have more than one
        /// PopoverProvider. Set a cascading value with UsePopoverProvider=false to prevent it.
        /// </summary>
        [CascadingParameter(Name = "UsePopoverProvider")]
        public bool IsEnabled { get; set; } = true;


        public void Dispose()
        {
            Service.FragmentsChanged -= Service_FragmentsChanged;
        }

        protected override void OnInitialized()
        {
            if (IsEnabled == false)
            {
                return;
            }

            Service.FragmentsChanged += Service_FragmentsChanged;
            _isConnectedToService = true;
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (IsEnabled == false && _isConnectedToService == true)
            {
                Service.FragmentsChanged -= Service_FragmentsChanged;
                _isConnectedToService = false;
            }
            else if (IsEnabled == true && _isConnectedToService == false)
            {
                Service.FragmentsChanged -= Service_FragmentsChanged; // make sure to avoid multiple registration
                Service.FragmentsChanged += Service_FragmentsChanged;
                _isConnectedToService = true;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && IsEnabled && Service.ThrowOnDuplicateProvider)
            {
                await Service.InitializeIfNeeded();
                if (await Service.CountProviders() > 1)
                {
                    throw new InvalidOperationException("Duplicate MudPopoverProvider detected. Please ensure there is only one provider, or disable this warning with PopoverOptions.ThrowOnDuplicateProvider.");
                }
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        private void Service_FragmentsChanged(object sender, EventArgs e)
        {
            InvokeAsync(StateHasChanged);
        }

    }
}
