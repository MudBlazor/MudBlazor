using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Input;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudOverlay :  MudComponentBase
    {
        protected string Classname =>
            new CssBuilder("mud-overlay")
                .AddClass("mud-overlay-absolute", Absolute)
                .AddClass(Class)
                .Build();

        protected string ScrimClassname =>
            new CssBuilder("mud-overlay-scrim")
                .AddClass("mud-overlay-dark", DarkBackground)
                .AddClass("mud-overlay-light", LightBackground)
                .AddClass(Class)
                .Build();

        protected string Styles =>
            new StyleBuilder()
            .AddStyle("z-index", $"{ZIndex}", ZIndex != 5)
            .AddStyle(Style)
            .Build();

        /// <summary>
        /// Child content of the component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// If true overlay will be visible.
        /// </summary>
        [Parameter] public bool Visible { get; set; }

        /// <summary>
        /// If true applys the themes dark overlay color.
        /// </summary>
        [Parameter] public bool DarkBackground { get; set; }

        /// <summary>
        /// If true applys the themes light overlay color.
        /// </summary>
        [Parameter] public bool LightBackground { get; set; }

        /// <summary>
        /// Icon class names, separated by space
        /// </summary>
        [Parameter] public bool Absolute { get; set; }

        /// <summary>
        /// Sets the z-index of the overlay.
        /// </summary>
        [Parameter] public int ZIndex { get; set; } = 5;

        /// <summary>
        /// Command parameter.
        /// </summary>
        [Parameter] public object CommandParameter { get; set; }

        /// <summary>
        /// Command executed when the user clicks on an element.
        /// </summary>
        [Parameter] public ICommand Command { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }
        protected void OnClickHandler(MouseEventArgs ev)
        {
            OnClick.InvokeAsync(ev);
            if (Command?.CanExecute(CommandParameter) ?? false)
            {
                Command.Execute(CommandParameter);
            }
        }
    }
}
