using System;
using System.Windows.Input;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudOverlay : MudComponentBase, IDisposable
    {
        private bool _visible;

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

        [Inject] public IScrollManager ScrollManager { get; set; }

        /// <summary>
        /// Child content of the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Overlay.Behavior)]
        public RenderFragment ChildContent { get; set; }

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
        public bool Visible
        {
            get => _visible;
            set
            {
                if (_visible == value)
                    return;
                _visible = value;
                VisibleChanged.InvokeAsync(_visible);
            }
        }

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
        /// If true applys the themes dark overlay color.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Overlay.Behavior)]
        public bool DarkBackground { get; set; }

        /// <summary>
        /// If true applys the themes light overlay color.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Overlay.Behavior)]
        public bool LightBackground { get; set; }

        /// <summary>
        /// Icon class names, separated by space
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
        /// Command parameter.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Overlay.ClickAction)]
        public object CommandParameter { get; set; }

        /// <summary>
        /// Command executed when the user clicks on an element.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Overlay.ClickAction)]
        public ICommand Command { get; set; }

        /// <summary>
        /// Fired when the overlay is clicked
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }
        protected internal void OnClickHandler(MouseEventArgs ev)
        {
            if (AutoClose)
                Visible = false;
            OnClick.InvokeAsync(ev);
            if (Command?.CanExecute(CommandParameter) ?? false)
            {
                Command.Execute(CommandParameter);
            }
        }

        //if not visible or CSS `position:absolute`, don't lock scroll
        protected override void OnAfterRender(bool firstTime)
        {
            if (!LockScroll || Absolute)
                return;

            if (Visible)
                BlockScroll();
            else
                UnblockScroll();

        }

        //locks the scroll attaching a CSS class to the specified element, in this case the body
        void BlockScroll()
        {
            ScrollManager.LockScrollAsync("body", LockScrollClass);
        }

        //removes the CSS class that prevented scrolling
        void UnblockScroll()
        {
            ScrollManager.UnlockScrollAsync("body", LockScrollClass);
        }

        //When disposing the overlay, remove the class that prevented scrolling
        public void Dispose()
        {
            UnblockScroll();
        }

    }
}
