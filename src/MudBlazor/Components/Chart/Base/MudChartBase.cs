// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using MudBlazor.Utilities;

#nullable enable
namespace MudBlazor.Charts;

public abstract class MudChartBase<TOptions> : MudComponentBase, IMudChart where TOptions : IChartOptions
{
    [CascadingParameter(Name = "RightToLeft")]
    [Category(CategoryTypes.Chart.Behavior)]
    public bool RightToLeft { get; set; }

    /// <summary>
    /// The labels describing data values.
    /// </summary>
    /// <remarks>
    /// The number of labels in this array is typically the same as the number of values in the <see cref="ChartDataSet.Data"/> property.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chart.Behavior)]
    public string[] ChartLabels { get; set; } = [];

    /// <summary>
    /// The series of values to display.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Behavior)]
    public List<ChartDataSet> ChartSeries { get; set; } = [];

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
    [Parameter]
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

    protected string Classname => new CssBuilder("mud-chart")
        .AddClass($"mud-chart-legend-{ConvertLegendPosition(LegendPosition).ToDescriptionString()}", ChartType != ChartType.HeatMap)
        .AddClass(Class)
        .Build();

    protected readonly ParameterState<int> SelectedIndexState;

    protected MudChartBase()
    {
        using var registerScope = CreateRegisterScope();
        SelectedIndexState = registerScope.RegisterParameter<int>(nameof(SelectedIndex))
            .WithParameter(() => SelectedIndex)
            .WithEventCallback(() => SelectedIndexChanged);
    }

    private Position ConvertLegendPosition(Position position) => position switch
    {
        Position.Start => RightToLeft ? Position.Right : Position.Left,
        Position.End => RightToLeft ? Position.Left : Position.Right,
        _ => position
    };

    internal async Task SetSelectedIndexAsync(int index)
    {
        await SelectedIndexState.SetValueAsync(index);
    }
}
