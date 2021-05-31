using System;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public class MudBodyRendered : ComponentBase, IDisposable
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool IsRendered { get; set; } = true;

        [Inject]
        public IBodyRenderService BodyRenderService { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            BodyRenderService?.AddComponent(this);
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            BodyRenderService?.Render();
        }

        public void Dispose()
        {

            BodyRenderService?.RemoveComponent(this);
        }
    }
}
