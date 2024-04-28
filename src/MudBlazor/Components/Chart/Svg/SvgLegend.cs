﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.Charts.SVG.Models
{
    /// <summary>
    /// Represents a series of series labels as an SVG path.
    /// </summary>
    [DebuggerDisplay("{Index} = {Labels}")]
    public class SvgLegend
    {
        /// <summary>
        /// Gets or sets the position of this path within a list.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the series labels to display.
        /// </summary>
        public string Labels { get; set; }

        /// <summary>
        /// Gets or sets the data values to display.
        /// </summary>
        public string Data { get; set; }
        public bool Visible { get; set; } = true;

        /// <summary>
        /// Gets or sets whether the legend is displayed.
        /// </summary>
        public bool Visible { get; set; } = true;

        /// <summary>
        /// Occurs when the <see cref="Visible"/> property has changed.
        /// </summary>
        public EventCallback<SvgLegend> OnVisibilityChanged { get; set; }

        /// <summary>
        /// Toggles the visibility of this legend.
        /// </summary>
        public async Task HandleCheckboxChangeAsync()
        {
            Visible = !Visible;
            await OnVisibilityChanged.InvokeAsync(this);
        }
    }
}
