﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents a button for actions, links, and commands.
    /// </summary>
    public partial class MudButton : MudBaseButton, IHandleEvent
    {
        protected string Classname => new CssBuilder("mud-button-root mud-button")
            .AddClass($"mud-button-{Variant.ToDescriptionString()}")
            .AddClass($"mud-button-{Variant.ToDescriptionString()}-{Color.ToDescriptionString()}")
            .AddClass($"mud-button-{Variant.ToDescriptionString()}-size-{Size.ToDescriptionString()}")
            .AddClass($"mud-width-full", FullWidth)
            .AddClass($"mud-ripple", Ripple)
            .AddClass($"mud-button-disable-elevation", !DropShadow)
            .AddClass(Class)
            .Build();

        protected string StartIconClass => new CssBuilder("mud-button-icon-start")
            .AddClass($"mud-button-icon-size-{(IconSize ?? Size).ToDescriptionString()}")
            .AddClass(IconClass)
            .Build();

        protected string EndIconClass => new CssBuilder("mud-button-icon-end")
            .AddClass($"mud-button-icon-size-{(IconSize ?? Size).ToDescriptionString()}")
            .AddClass(IconClass)
            .Build();

        /// <summary>
        /// Gets or sets the icon displayed before the text.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Use <see cref="EndIcon"/> to display an icon after the text.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public string? StartIcon { get; set; }

        /// <summary>
        /// Gets or sets the icon displayed after the text.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Use <see cref="StartIcon"/> to display an icon before the text.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public string? EndIcon { get; set; }

        /// <summary>
        /// Gets or sets the color of icons.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Inherit"/>.  
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Color IconColor { get; set; } = Color.Inherit;

        /// <summary>
        /// Gets or sets the size of icons.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  When <c>null</c>, the value of <see cref="Size"/> is used.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Size? IconSize { get; set; }

        /// <summary>
        /// Gets or sets any CSS classes applied to icons.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  You can use spaces to separate multiple classes.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public string? IconClass { get; set; }

        /// <summary>
        /// Gets or sets the color of the button.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>.  Theme colors are supported.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// Gets or sets the size of the button.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Size.Medium"/>.   Use the <see cref="IconSize"/> property to set the size of icons.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// Gets or sets the display variation to use.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Variant.Text"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        /// Gets or sets whether the button takes up 100% of the container width.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public bool FullWidth { get; set; }

        /// <summary>
        /// Gets or sets any content within this component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <inheritdoc/>
        /// <remarks>
        /// See: https://github.com/MudBlazor/MudBlazor/issues/8365
        /// <para/>
        /// Since <see cref="MudButton"/> implements only single <see cref="EventCallback"/> <see cref="MudBaseButton.OnClick"/> this is safe to disable globally within the component.
        /// </remarks>
        Task IHandleEvent.HandleEventAsync(EventCallbackWorkItem callback, object? arg) => callback.InvokeAsync(arg);
    }
}
