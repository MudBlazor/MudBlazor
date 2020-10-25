using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using MudBlazor.Charts.SVG.Models;

namespace MudBlazor.Charts
{
    public class LineBase : MudChartBase
    {
        [CascadingParameter] public MudChart MudChartParent { get; set; }

        public List<SvgPath> HorizontalLines = new List<SvgPath>();
        public List<SvgText> HorizontalValues = new List<SvgText>();

        public List<SvgPath> VerticalLines = new List<SvgPath>();
        
        public List<SvgLegend> Legends = new List<SvgLegend>();
        public List<ChartSeries> Series = new List<ChartSeries>();

        public string Test1 { get; set; }
        public string Test2 { get; set; }
        public string Test3 { get; set; }
        public string Test4 { get; set; }
        public string Test5 { get; set; }

        protected override void OnInitialized()
        {

            //string[] inputLabels = InputLabels.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            //string[] xAxisinputLabels = XAxisLabels.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            Series = MudChartParent.ChartSeries;

            int SeriesCount = Series.Count;

            double maxY = 0.0;
            int numValues = 0;
            int numXLabels = XAxisLabels.Length;
            foreach (var item in Series)
            {
                if(numValues < item.Data.Length)
                {
                    numValues = item.Data.Length;
                }
                foreach (int i in item.Data)
                {
                    if(maxY < i)
                    {
                        maxY = i;
                    }
                }
            }

            double boundHeight = 350.0;
            double boundWidth = 650.0;

            double gridYUnits = 10;
            double gridXUnits = 10;

            int numVerticalLines = numValues;

            int numHorizontalLines = ((int)(maxY / gridYUnits)) + 1;

            double verticalStartSpace = 25.0;
            double horizontalStartSpace = 25.0;
            double verticalEndSpace = 25.0;
            double horizontalEndSpace = 25.0;

            double verticalSpace = (boundHeight - verticalStartSpace - verticalEndSpace) / (numHorizontalLines);
            double horizontalSpace = (boundWidth - horizontalStartSpace - horizontalEndSpace) / (numVerticalLines);

            double totalGridWidth = ((double)(numVerticalLines - 1)) * horizontalSpace;
            double totalGridHeight = ((double)(numHorizontalLines - 1)) * verticalSpace;

            //Horizontal Grid Lines
            double y = verticalStartSpace;
            double startGridY = 0;
            for (int counter = 0; counter <= numHorizontalLines; counter++)
            {
                SvgPath Line = new SvgPath()
                {
                    Index = counter,
                    Data = $"M {horizontalStartSpace.ToString(CultureInfo.InvariantCulture)} {(boundHeight - y).ToString(CultureInfo.InvariantCulture)} L {(boundWidth - horizontalEndSpace).ToString(CultureInfo.InvariantCulture)} {(boundHeight - y).ToString(CultureInfo.InvariantCulture)}"
                };
                HorizontalLines.Add(Line);

                SvgText LineValue = new SvgText() { X = (horizontalStartSpace - 2), Y = (boundHeight - y), Value = startGridY.ToString(CultureInfo.InvariantCulture) };
                HorizontalValues.Add(LineValue);

                y = y + verticalSpace;
                startGridY = startGridY + gridYUnits;
            }

            //Vertical Grid Lines
            double x = horizontalStartSpace;
            double startGridX = 0;
            int xLabelsCounter = 0;
            for (int counter = 0; counter <= numVerticalLines; counter++)
            {

                SvgPath Line = new SvgPath()
                {
                    Index = counter,
                    Data = $"M {x.ToString(CultureInfo.InvariantCulture)} {(boundHeight - verticalStartSpace).ToString(CultureInfo.InvariantCulture)} L {x.ToString(CultureInfo.InvariantCulture)} {verticalEndSpace.ToString(CultureInfo.InvariantCulture)}"
                };
                VerticalLines.Add(Line);

                //not required. just need number of grid lines
                startGridX = startGridX + gridXUnits;

                x = x + horizontalSpace;
            }

            foreach (var item in Series)
            {
                string chartLine = "";
                double gridValueX = 0;
                double gridValueY = 0;
                bool firstTime = true;
            }



            //Test
            Test1 = $"MaxY is: {maxY}";
            Test2 = $"Number of Values is: {numValues}";
            Test3 = $"Number of XLabels is: {numXLabels}";
            Test4 = $"Number of VerticalLines: {numVerticalLines}";
            Test5 = $"Number of HorizontalLines: {numHorizontalLines}";
        }
    }
}
