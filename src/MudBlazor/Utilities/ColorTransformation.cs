// Copyright (c) Steven Coco
// https://stackoverflow.com/questions/4087581/creating-a-c-sharp-color-from-hsl-values/4087601#4087601
// Stripped and adapted by Meinrad Recheis for MudBlazor

using System;
using System.Drawing;
using SystemMath = System.Math;
using MudColor = System.Drawing.Color;

namespace MudBlazor.Utilities
{
    /// <summary>
    /// Static methods for transforming argb spaces and argb values.
    /// </summary>
    public static class ColorTransformation
    {
        private static double EPSILON => 0.000000000000001;
 
        public class HSLColor
        {
            public double H;
            public double S;
            public double L;
        }

        /// <summary>
        /// Converts RGB to HSL. Alpha is ignored.
        /// Output is: { H: [0, 360], S: [0, 1], L: [0, 1] }.
        /// </summary>
        /// <param name="color">The color to convert.</param>
        public static HSLColor RgBtoHsl(MudColor color)
        {
            double h = 0D;
            double s = 0D;
            double l;

            // normalize red, green, blue values
            double r = color.R / 255D;
            double g = color.G / 255D;
            double b = color.B / 255D;

            double max = SystemMath.Max(r, SystemMath.Max(g, b));
            double min = SystemMath.Min(r, SystemMath.Min(g, b));

            // hue
            if (SystemMath.Abs(max - min) < EPSILON)
                h = 0D; // undefined
            else if ((SystemMath.Abs(max - r) < EPSILON)
                    && (g >= b))
                h = (60D * (g - b)) / (max - min);
            else if ((SystemMath.Abs(max - r) < EPSILON)
                    && (g < b))
                h = ((60D * (g - b)) / (max - min)) + 360D;
            else if (SystemMath.Abs(max - g) < EPSILON)
                h = ((60D * (b - r)) / (max - min)) + 120D;
            else if (SystemMath.Abs(max - b) < EPSILON)
                h = ((60D * (r - g)) / (max - min)) + 240D;

            // luminance
            l = (max + min) / 2D;

            // saturation
            if ((SystemMath.Abs(l) < EPSILON)
                    || (SystemMath.Abs(max - min) < EPSILON))
                s = 0D;
            else if ((0D < l)
                    && (l <= .5D))
                s = (max - min) / (max + min);
            else if (l > .5D)
                s = (max - min) / (2D - (max + min)); //(max-min > 0)?

            return new HSLColor
            {
                H=SystemMath.Max(0D, SystemMath.Min(360D, h)),
                S=SystemMath.Max(0D, SystemMath.Min(1D, s)),
                L=SystemMath.Max(0D, SystemMath.Min(1D, l))
            };
        }

        /// <summary>
        /// Converts HSL to RGB, with a specified output Alpha.
        /// Arguments are limited to the defined range:
        /// does not raise exceptions.
        /// </summary>
        /// <param name="hsl">HSL comprising of - </param>
        /// hsl.H (Hue), must be in [0, 360] - 
        /// hsl.S (Saturation), must be in [0, 1] - 
        /// hsl.L (Luminance), must be in [0, 1].
        /// <param name="a">Output Alpha, must be in [0, 255].</param>
        public static MudColor HsLtoRgb(HSLColor hsl, int a = 255)
        {
            var h = SystemMath.Max(0D, SystemMath.Min(360D, hsl.H));
            var s = SystemMath.Max(0D, SystemMath.Min(1D, hsl.S));
            var l = SystemMath.Max(0D, SystemMath.Min(1D, hsl.L));
            a = SystemMath.Max(0, SystemMath.Min(255, a));

            // achromatic argb (gray scale)
            if (SystemMath.Abs(s) < EPSILON)
            {
                return MudColor.FromArgb(
                        a,
                        SystemMath.Max(0, SystemMath.Min(255, Convert.ToInt32(double.Parse($"{l * 255D:0.00}")))),
                        SystemMath.Max(0, SystemMath.Min(255, Convert.ToInt32(double.Parse($"{l * 255D:0.00}")))),
                        SystemMath.Max(0, SystemMath.Min(255, Convert.ToInt32(double.Parse($"{l * 255D:0.00}")))));
            }

            double q = l < .5D
                    ? l * (1D + s)
                    : (l + s) - (l * s);
            double p = (2D * l) - q;

            double hk = h / 360D;
            double[] T = new double[3];
            T[0] = hk + (1D / 3D); // Tr
            T[1] = hk; // Tb
            T[2] = hk - (1D / 3D); // Tg

            for (int i = 0; i < 3; i++)
            {
                if (T[i] < 0D)
                    T[i] += 1D;
                if (T[i] > 1D)
                    T[i] -= 1D;

                if ((T[i] * 6D) < 1D)
                    T[i] = p + ((q - p) * 6D * T[i]);
                else if ((T[i] * 2D) < 1)
                    T[i] = q;
                else if ((T[i] * 3D) < 2)
                    T[i] = p + ((q - p) * ((2D / 3D) - T[i]) * 6D);
                else
                    T[i] = p;
            }

            return MudColor.FromArgb(
                    a,
                    SystemMath.Max(0, SystemMath.Min(255, (int)Math.Round(T[0] * 255D))),
                    SystemMath.Max(0, SystemMath.Min(255, (int)Math.Round(T[1] * 255D))),
                    SystemMath.Max(0, SystemMath.Min(255, (int) Math.Round(T[2] * 255D))));
        }
 
    }
}
