// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;
using Microsoft.AspNetCore.Components;
using MudBlazor.Charts;
using MudBlazor.State;
using MudBlazor.Utilities;

#nullable enable
namespace MudBlazor;

/// <summary>
/// Represents a base class for chart components.
/// </summary>
/// <typeparam name="T">The data type of the chart.</typeparam>
/// <typeparam name="TOptions">The type of options for the chart.</typeparam>
public abstract class MudChartBase<T, TOptions> : MudComponentBase, IMudChart<T>
    where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
    where TOptions : IChartOptions
{
    /// <summary>
    /// If true, the chart will be rendered from right to left.
    /// </summary>
    [CascadingParameter(Name = "RightToLeft")]
    [Category(CategoryTypes.Chart.Behavior)]
    public bool RightToLeft { get; set; }

    /// <summary>
    /// A reference to the chart component.
    /// </summary>
    [CascadingParameter]
    [Category(CategoryTypes.Chart.Behavior)]
    public IMudChart<T>? ChartReference { get; set; }

    /// <summary>
    /// The labels describing data values.
    /// </summary>
    /// <remarks>
    /// The number of labels in this array is typically the same as the number of values in the <see cref="ChartSeries{T}.Data"/> property.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chart.Behavior)]
    public string[] ChartLabels { get; set; } = [];

    /// <summary>
    /// The series of values to display.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Behavior)]
    public List<ChartSeries<T>> ChartSeries { get; set; } = [];

    /// <summary>
    /// The display options applied to the chart.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public TOptions? ChartOptions { get; set; }

    /// <summary>
    /// The custom graphics within this chart.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public RenderFragment? CustomGraphics { get; set; }

    /// <summary>
    /// Optional template for custom tooltip rendering. If provided, this will be used instead of the default ChartTooltip.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public RenderFragment<(SvgPath Segment, string Color)>? TooltipTemplate { get; set; }

    /// <summary>
    /// Optional callback function to determine the position of the tooltip.    
    /// </summary>
    /// <remarks>
    /// If not provided, the default tooltip positioning logic will be used.
    /// </remarks>
    /// <returns>
    /// <see cref="Tuple{T1, T2}"/> representing the absolute coordinates where the tooltip should be positioned.
    /// </returns>
    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public Func<SvgPath, (double X, double Y)>? TooltipPositionFunc { get; set; }

    /// <summary>
    /// ChildContent for this component
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public RenderFragment? ChildContent { get; set; }


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
    /// Defaults to <c>80%</c>. Values can be a percentage or pixel width such as <c>200px</c>.
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
    /// Make the chart fill the parent
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public bool MatchBoundsToSize { get; set; }

    /// <summary>
    /// The location of series labels.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="Position.Bottom"/>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public Position LegendPosition { get; set; } = Position.Bottom;

    /// <summary>
    /// The currently selected data point.
    /// </summary>
    /// <remarks>
    /// When this property changes, the <see cref="SelectedIndexChanged"/> event occurs.
    /// </remarks>
    [Parameter, ParameterState]
    [Category(CategoryTypes.Chart.Behavior)]
    public int SelectedIndex { get; set; }

    /// <summary>
    /// Occurs when the <see cref="SelectedIndex"/> has changed.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Behavior)]
    public EventCallback<int> SelectedIndexChanged { get; set; }

    /// <summary>
    /// Allows series to be hidden 
    /// </summary>
    /// <remarks>
    /// When <c>true</c>, checkboxes are displayed which can toggle visibility of each data set
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chart.Behavior)]
    public bool CanHideSeries { get; set; } = false;

    /// <summary>
    /// The palette of colors to be used for the legend.
    /// </summary>
    public virtual string[] LegendPalette => ChartOptions?.ChartPalette ?? [];

    /// <summary>
    /// The CSS classes for the chart component.
    /// </summary>
    protected string Classname => new CssBuilder("mud-chart")
        .AddClass($"mud-chart-legend-{ConvertLegendPosition(LegendPosition).ToDescriptionString()}")
        .AddClass(Class)
        .Build();

    /// <summary>
    /// The state of the selected index.
    /// </summary>
    protected readonly ParameterState<int> SelectedIndexState;

    /// <summary>
    /// Initializes a new instance of the <see cref="MudChartBase{T, TOptions}"/> class.
    /// </summary>
    protected MudChartBase()
    {
        using var registerScope = CreateRegisterScope();
        SelectedIndexState = registerScope.RegisterParameter<int>(nameof(SelectedIndex))
            .WithParameter(() => SelectedIndex)
            .WithEventCallback(() => SelectedIndexChanged);
    }

    /// <summary>
    /// Rebuilds the chart.
    /// </summary>
    public abstract void RebuildChart();

    private Position ConvertLegendPosition(Position position) => position switch
    {
        Position.Start => RightToLeft ? Position.Right : Position.Left,
        Position.End => RightToLeft ? Position.Left : Position.Right,
        _ => position
    };

    /// <summary>
    /// Sets the selected index.
    /// </summary>
    /// <param name="index">The new selected index.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    internal async Task SetSelectedIndexAsync(int index)
    {
        await SelectedIndexState.SetValueAsync(index);
    }
}
