// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

#nullable enable
namespace MudBlazor;

/// <summary>
/// Shared a base class for designing category <see cref="MudChart"/> and <see cref="MudTimeSeriesChart"/> components.
/// </summary>
public abstract class MudChartBase : MudComponentBase
{
    /// <summary>
    /// The display options applied to the chart.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public ChartOptions ChartOptions { get; set; } = new();

    /// <summary>
    /// The custom graphics within this chart.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public RenderFragment? CustomGraphics { get; set; }

    protected string Classname => new CssBuilder("mud-chart")
        .AddClass($"mud-chart-legend-{ConvertLegendPosition(LegendPosition).ToDescriptionString()}", ChartType != ChartType.HeatMap)
        .AddClass(Class)
        .Build();

    [CascadingParameter(Name = "RightToLeft")]
    [Category(CategoryTypes.Chart.Behavior)]
    public bool RightToLeft { get; set; }

    /// <summary>
    /// The type of chart to display.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Behavior)]
    public ChartType ChartType { get; set; }

    /// <summary>
    /// The width of the chart, as a CSS style.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>80%</c>.  Values can be a percentage or pixel width such as <c>200px</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public string Width { get; set; } = "80%";

    /// <summary>
    /// The height of the chart, as a CSS style.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>80%</c>.  Values can be a percentage or pixel width such as <c>200px</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public string Height { get; set; } = "80%";

    /// <summary>
    /// The location of series labels.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Position.Bottom"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public Position LegendPosition { get; set; } = Position.Bottom;

    private Position ConvertLegendPosition(Position position)
    {
        return position switch
        {
            Position.Start => RightToLeft ? Position.Right : Position.Left,
            Position.End => RightToLeft ? Position.Left : Position.Right,
            _ => position
        };
    }

    private int _selectedIndex;

    /// <summary>
    /// The currently selected data point.
    /// </summary>
    /// <remarks>
    /// When this property changes, the <see cref="SelectedIndexChanged"/> event occurs.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chart.Behavior)]
    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            if (value != _selectedIndex)
            {
                _selectedIndex = value;
                SelectedIndexChanged.InvokeAsync(value);
            }
        }
    }

    internal void SetSelectedIndex(int index)
    {
        SelectedIndex = index;
    }

    /// <summary>
    /// Occurs when the <see cref="SelectedIndex"/> has changed.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Behavior)]
    public EventCallback<int> SelectedIndexChanged { get; set; }

    protected string ToS(double d, string? format = null)
    {
        if (string.IsNullOrEmpty(format))
            return d.ToString(CultureInfo.InvariantCulture);

        return d.ToString(format);
    }

    /// <summary>
    /// Allows series to be hidden when <see cref="ChartType"/> is <see cref="ChartType.Line"/>.
    /// </summary>
    /// <remarks>
    /// When <c>true</c>, checkboxes are displayed which can toggle visibility of each line.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chart.Behavior)]
    public bool CanHideSeries { get; set; } = false;
}
