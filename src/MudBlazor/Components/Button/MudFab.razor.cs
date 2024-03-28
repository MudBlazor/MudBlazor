using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudFab : MudBaseButton, IHandleEvent
    {
        protected string Classname =>
            new CssBuilder("mud-button-root mud-fab")
                .AddClass($"mud-fab-extended", !string.IsNullOrEmpty(Label))
                .AddClass($"mud-fab-{Color.ToDescriptionString()}")
                .AddClass($"mud-fab-size-{Size.ToDescriptionString()}")
                .AddClass($"mud-ripple", !DisableRipple && !GetDisabledState())
                .AddClass($"mud-fab-disable-elevation", DisableElevation)
                .AddClass(Class)
                .Build();

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            var hasStartIcon = StartIcon?.AsSpan().Trim().Length > 0;
            var hasEndIcon = EndIcon.AsSpan().Trim().Length > 0;

            if (IconProperties is not null)
            {
                _iconProperties = IconProperties;
                if (_iconProperties.HasIcon())
                {
                    // Backwards compatibility

                    if (hasStartIcon) StartIcon = _iconProperties.Icon;
                    else if (hasEndIcon) EndIcon = _iconProperties.Icon;
                }

                if (_iconProperties.HasTitle()) Title = _iconProperties.Title;
            }
            else
            {
                _iconProperties.Icon = hasStartIcon ? StartIcon : hasEndIcon ? EndIcon : string.Empty;
                _iconProperties.Title = Title;
                _iconProperties.Size = IconSize;
                _iconProperties.Color = IconColor;
            }

            _iconProperties.Position = hasStartIcon ? Position.Start : hasEndIcon ? Position.End : null;
        }


        IconProperties _iconProperties = new();

        /// <summary>
        /// The icon properties.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Icon.Behavior)]
        public IconProperties? IconProperties { get; set; }

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The Size of the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Size Size { get; set; } = Size.Large;

        /// <summary>
        /// If applied Icon will be added at the start of the component.
        /// </summary>
        [Obsolete("This property is obsolete. Use StartIcon instead.")]
        [Parameter]
        public string? Icon { get => StartIcon; set => StartIcon = value; }

        /// <summary>
        /// If applied Icon will be added at the start of the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public string? StartIcon { get; set; }

        /// <summary>
        /// If applied Icon will be added at the end of the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public string? EndIcon { get; set; }

        /// <summary>
        /// The color of the icon. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Color IconColor { get; set; } = Color.Inherit;

        /// <summary>
        /// The size of the icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Size IconSize { get; set; } = Size.Medium;

        /// <summary>
        /// If applied the text will be added to the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public string? Label { get; set; }

        /// <summary>
        /// Title of the icon used for accessibility.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public string? Title { get; set; }

        /// <inheritdoc/>
        /// <remarks>
        /// See: https://github.com/MudBlazor/MudBlazor/issues/8365
        /// <para/>
        /// Since <see cref="MudFab"/> implements only single <see cref="EventCallback"/> <see cref="MudBaseButton.OnClick"/> this is safe to disable globally within the component.
        /// </remarks>
        Task IHandleEvent.HandleEventAsync(EventCallbackWorkItem callback, object? arg) => callback.InvokeAsync(arg);
    }
}
