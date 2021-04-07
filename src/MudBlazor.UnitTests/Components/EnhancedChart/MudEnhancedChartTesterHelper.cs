﻿// Not Used

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using AngleSharp.Html.Dom;
using Bunit;
using MudBlazor.EnhanceChart;

namespace MudBlazor.UnitTests.Components.EnhancedChart
{
    public static class MudEnhancedChartTesterHelper
    {
        public static Regex _removeBlazorColonRegex = new Regex(@"(blazor:)([^>]*?)("".*?"")", RegexOptions.Multiline);

        public static XElement GetElementAsXmlDocument(IRenderedComponent<MudEnhancedBarChart> comp)
        {
            XElement root = new XElement("svg");

            foreach (var item in comp.Nodes.OfType<IHtmlUnknownElement>())
            {
                if (item.NodeName == "POLYGON")
                {
                    var preParsedHtml = _removeBlazorColonRegex.Replace(item.OuterHtml, String.Empty);
                    var element = XElement.Parse(preParsedHtml);
                    RoundElementValues(item, element);

                    if(item.FirstChild != null && item.FirstChild is IHtmlUnknownElement childElement && item.FirstChild.NodeName == "ANIMATE")
                    {
                        element.RemoveNodes();

                        var animateNode = XElement.Parse(childElement.OuterHtml);
                        RoundElementValues(childElement, animateNode);
                        element.Add(animateNode);
                    }

                  
                    root.Add(element);
                }
            }

            return root;
        }

        public static void RoundElementValues(IHtmlUnknownElement item, XElement element)
        {
            if (item.NodeName == "POLYGON")
            {
                RoundPointArrayValues(element, "points");
            }
            else if(item.NodeName == "ANIMATE")
            {
                RoundPointArrayValues(element, "from");
                RoundPointArrayValues(element, "to");
            }
            else if (item.NodeName == "TEXT")
            {
                RoundValue(element, "x");
                RoundValue(element, "y");
            }
            else if (item.NodeName == "LINE")
            {
                RoundValue(element, "x1");
                RoundValue(element, "y1");
                RoundValue(element, "x2");
                RoundValue(element, "y2");
            }
        }

        private static void RoundPointArrayValues(XElement element, String attributeName)
        {
            String pointRaw = element.Attribute(attributeName).Value;
            String[] points = pointRaw.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            List<Point> roundedPoints = new();
            foreach (var point in points)
            {
                String[] parts = point.Split(",");
                Double x = Math.Round(Double.Parse(parts[0], CultureInfo.InvariantCulture), 4);
                Double y = Math.Round(Double.Parse(parts[1], CultureInfo.InvariantCulture), 4);

                roundedPoints.Add(new Point(x, y));
            }

            String roundendPointsAttribute = String.Empty;
            foreach (var roundedPoint in roundedPoints)
            {
                roundendPointsAttribute += $"{roundedPoint.X.ToString(CultureInfo.InvariantCulture)},{roundedPoint.Y.ToString(CultureInfo.InvariantCulture)} ";
            }

            roundendPointsAttribute = roundendPointsAttribute.Trim();
            element.SetAttributeValue(attributeName, roundendPointsAttribute);
        }

        public static void RoundValue(XElement element, String attributeName, Int32 precission = 6)
        {
            Double unroundedValue = Double.Parse(element.Attribute(attributeName).Value, CultureInfo.InvariantCulture);
            Double roundedValue = Math.Round(unroundedValue, precission);
            String result = roundedValue.ToString(CultureInfo.InvariantCulture);
            if (result == "-0")
            {
                result = "0";
            }

            element.SetAttributeValue(attributeName, result);
        }
    }
}
