namespace MudBlazor
{
    public class ChartOptions
    {
        public int YAxisTicks { get; set; } = 20;
        public bool YAxisLines { get; set; } = true;
        public bool XAxisLines { get; set; }

        /// <summary>
        /// If true, legend will not be displayed.
        /// </summary>
        public bool DisableLegend { get; set; }
        public string[] ChartPalette { get; set; } = { Colors.Blue.Accent3, Colors.Teal.Accent3, Colors.Amber.Accent3, Colors.Orange.Accent3, Colors.Red.Accent3, Colors.DeepPurple.Accent3, Colors.Green.Accent3, Colors.LightBlue.Accent3, Colors.Teal.Lighten1, Colors.Amber.Lighten1, Colors.Orange.Lighten1, Colors.Red.Lighten1, Colors.DeepPurple.Lighten1, Colors.Green.Lighten1, Colors.LightBlue.Lighten1, Colors.Amber.Darken2, Colors.Orange.Darken2, Colors.Red.Darken2, Colors.DeepPurple.Darken2, Colors.Grey.Darken2 };
        public InterpolationOption CurveEnum { get; set; } = InterpolationOption.Straight;
    }
    public enum InterpolationOption
    {
        NaturalSpline,
        EndSlope,
        Periodic,
        Straight
    }
}
