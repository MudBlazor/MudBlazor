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
                .AddClass("mud-animation", FadeIn)
                .AddClass("mud-absolute", Absolute)
                .AddClass(Class)
                .Build();

        protected string Styles =>
            new StyleBuilder()
            .AddStyle("background-color", $"{BackgroundColor}", !String.IsNullOrEmpty(BackgroundColor))
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

        [Parameter] public string BackgroundColor { get; set; }

        /// <summary>
        /// If true will fadein.
        /// </summary>
        [Parameter] public bool FadeIn { get; set; }

        /// <summary>
        /// Icon class names, separated by space
        /// </summary>
        [Parameter] public bool Absolute { get; set; }

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
