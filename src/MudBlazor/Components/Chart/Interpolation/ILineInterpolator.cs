namespace MudBlazor;

internal interface ILineInterpolator
{
    public double[] GivenYs { get; set; }
    public double[] GivenXs { get; set; }
    public double[] InterpolatedXs { get; set; }
    public double[] InterpolatedYs { get; set; }
}
