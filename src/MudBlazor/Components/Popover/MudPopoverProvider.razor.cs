// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor
{
    public partial class MudPopoverProvider : IDisposable
    {
        public void Dispose()
        {
            Service.FragmentsChanged -= Service_FragmentsChanged;
        }

        protected override void OnInitialized()
        {
            Service.FragmentsChanged += Service_FragmentsChanged;
        }

        private void Service_FragmentsChanged(object sender, EventArgs e)
        {
            InvokeAsync(StateHasChanged);
        }
    }
}
