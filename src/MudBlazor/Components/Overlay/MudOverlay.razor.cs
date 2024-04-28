using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudOverlay : MudComponentBase, IAsyncDisposable
    {
        private readonly ParameterState<bool> _visibleState;

        protected string Classname =>
            new CssBuilder("mud-overlay")
                .AddClass("mud-overlay-absolute", Absolute)
                .AddClass(Class)
                .Build();

        protected string ScrimClassname =>
            new CssBuilder("mud-overlay-scrim")
                .AddClass("mud-overlay-dark", DarkBackground)
                .AddClass("mud-overlay-light", LightBackground)
                .Build();

        protected string Styles =>
            new StyleBuilder()
                .AddStyle("z-index", $"{ZIndex}", ZIndex != 5)
                .AddStyle(Style)
                .Build();

        [Inject]
        public IScrollManager ScrollManager { get; set; } = null!;

        /// <summary>
        /// Child content of the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Overlay.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Fires when Visible changes
        /// </summary>
        [Parameter]
        public EventCallback<bool> VisibleChanged { get; set; }

        /// <summary>
        /// If true overlay will be visible. Two-way bindable.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Overlay.Behavior)]
        public bool Visible { get; set; }

        /// <summary>
        /// If true overlay will set Visible false on click.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Overlay.ClickAction)]
        public bool AutoClose { get; set; }

        /// <summary>
        /// If true (default), the Document.body element will not be able to scroll
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Overlay.Behavior)]
        public bool LockScroll { get; set; } = true;

        /// <summary>
        /// The css class that will be added to body if lockscroll is used.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Overlay.Behavior)]
        public string LockScrollClass { get; set; } = "scroll-locked";

        /// <summary>
        /// If true applies the themes dark overlay color.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Overlay.Behavior)]
        public bool DarkBackground { get; set; }

        /// <summary>
        /// If true applies the themes light overlay color.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Overlay.Behavior)]
        public bool LightBackground { get; set; }

        /// <summary>
        /// If true, use absolute positioning for the overlay.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Overlay.Behavior)]
        public bool Absolute { get; set; }

        /// <summary>
        /// Sets the z-index of the overlay.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Overlay.Behavior)]
        public int ZIndex { get; set; } = 5;

        /// <summary>
        /// Fired when the overlay is clicked
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        public MudOverlay()
        {
            using var registerScope = CreateRegisterScope();
            _visibleState = registerScope.RegisterParameter<bool>(nameof(Visible))
                .WithParameter(() => Visible)
                .WithEventCallback(() => VisibleChanged)
                .WithChangeHandler(OnVisibleParameterChangedAsync);
        }

        protected internal async Task OnClickHandlerAsync(MouseEventArgs ev)
        {
            if (AutoClose)
            {
                await _visibleState.SetValueAsync(false);
            }

            await OnClick.InvokeAsync(ev);
        }

        //if not visible or CSS `position:absolute`, don't lock scroll
        protected override async Task OnAfterRenderAsync(bool firstTime)
        {
            if (!LockScroll || Absolute)
            {
                return;
            }

            if (Visible)
            {
                await BlockScrollAsync();
            }
            else
            {
                await UnblockScrollAsync();
            }
        }

        private Task OnVisibleParameterChangedAsync()
        {
            return VisibleChanged.InvokeAsync(_visibleState.Value);
        }

        //locks the scroll attaching a CSS class to the specified element, in this case the body
        private ValueTask BlockScrollAsync()
        {
            return ScrollManager.LockScrollAsync("body", LockScrollClass);
        }

        //removes the CSS class that prevented scrolling
        private ValueTask UnblockScrollAsync()
        {
            return ScrollManager.UnlockScrollAsync("body", LockScrollClass);
        }

        //When disposing the overlay, remove the class that prevented scrolling
        public ValueTask DisposeAsync()
        {
            if (IsJSRuntimeAvailable)
            {
                return UnblockScrollAsync();
            }

            return ValueTask.CompletedTask;
        }
    }
}
