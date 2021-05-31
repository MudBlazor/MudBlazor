// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public partial class MudBodyRenderProvider : ComponentBase, IDisposable
    {
        private List<MudBodyRendered> components = new List<MudBodyRendered>();

        [Inject]
        public IBodyRenderService BodyRenderService { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            BodyRenderService.OnRenderRequested += StateHasChanged;
            BodyRenderService.OnComponentAdded += BodyRenderService_OnComponentAdded;
            BodyRenderService.OnComponentRemoved += BodyRenderService_OnComponentRemoved;
        }

        private void BodyRenderService_OnComponentAdded(MudBodyRendered obj)
        {
            components.Add(obj);
            StateHasChanged();
        }

        private void BodyRenderService_OnComponentRemoved(MudBodyRendered obj)
        {
            components.Remove(obj);
            StateHasChanged();
        }

        public void Dispose()
        {
            BodyRenderService.OnRenderRequested -= StateHasChanged;
            BodyRenderService.OnComponentAdded -= BodyRenderService_OnComponentAdded;
            BodyRenderService.OnComponentRemoved -= BodyRenderService_OnComponentRemoved;
        }
    }
}
