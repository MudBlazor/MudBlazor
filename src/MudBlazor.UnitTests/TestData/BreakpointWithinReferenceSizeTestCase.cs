// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace MudBlazor.UnitTests.TestData
{
    public class BreakpointWithinReferenceSizeTestCase
    {
        public static IEnumerable<(Breakpoint, Breakpoint, bool)> AllCombinations()
        {
            return new[]
            {
                // Xs
                (Breakpoint.Xs, Breakpoint.Xs, true),
                (Breakpoint.Xs, Breakpoint.Sm, false),
                (Breakpoint.Xs, Breakpoint.Md, false),
                (Breakpoint.Xs, Breakpoint.Lg, false),
                (Breakpoint.Xs, Breakpoint.Xl, false),
                (Breakpoint.Xs, Breakpoint.Xxl, false),

                // Sm
                (Breakpoint.Sm, Breakpoint.Xs, false),
                (Breakpoint.Sm, Breakpoint.Sm, true),
                (Breakpoint.Sm, Breakpoint.Md, false),
                (Breakpoint.Sm, Breakpoint.Lg, false),
                (Breakpoint.Sm, Breakpoint.Xl, false),
                (Breakpoint.Sm, Breakpoint.Xxl, false),

                // Md
                (Breakpoint.Md, Breakpoint.Xs, false),
                (Breakpoint.Md, Breakpoint.Sm, false),
                (Breakpoint.Md, Breakpoint.Md, true),
                (Breakpoint.Md, Breakpoint.Lg, false),
                (Breakpoint.Md, Breakpoint.Xl, false),
                (Breakpoint.Md, Breakpoint.Xxl, false),

                // Lg
                (Breakpoint.Lg, Breakpoint.Xs, false),
                (Breakpoint.Lg, Breakpoint.Sm, false),
                (Breakpoint.Lg, Breakpoint.Md, false),
                (Breakpoint.Lg, Breakpoint.Lg, true),
                (Breakpoint.Lg, Breakpoint.Xl, false),
                (Breakpoint.Lg, Breakpoint.Xxl, false),

                // Xl
                (Breakpoint.Xl, Breakpoint.Xs, false),
                (Breakpoint.Xl, Breakpoint.Sm, false),
                (Breakpoint.Xl, Breakpoint.Md, false),
                (Breakpoint.Xl, Breakpoint.Lg, false),
                (Breakpoint.Xl, Breakpoint.Xl, true),
                (Breakpoint.Xl, Breakpoint.Xxl, false),

                // Xxl
                (Breakpoint.Xxl, Breakpoint.Xs, false),
                (Breakpoint.Xxl, Breakpoint.Sm, false),
                (Breakpoint.Xxl, Breakpoint.Md, false),
                (Breakpoint.Xxl, Breakpoint.Lg, false),
                (Breakpoint.Xxl, Breakpoint.Xl, false),
                (Breakpoint.Xxl, Breakpoint.Xxl, true),

                // SmAndDown
                (Breakpoint.SmAndDown, Breakpoint.Xs, true),
                (Breakpoint.SmAndDown, Breakpoint.Sm, true),
                (Breakpoint.SmAndDown, Breakpoint.Md, false),
                (Breakpoint.SmAndDown, Breakpoint.Lg, false),
                (Breakpoint.SmAndDown, Breakpoint.Xl, false),
                (Breakpoint.SmAndDown, Breakpoint.Xxl, false),

                // MdAndDown
                (Breakpoint.MdAndDown, Breakpoint.Xs, true),
                (Breakpoint.MdAndDown, Breakpoint.Sm, true),
                (Breakpoint.MdAndDown, Breakpoint.Md, true),
                (Breakpoint.MdAndDown, Breakpoint.Lg, false),
                (Breakpoint.MdAndDown, Breakpoint.Xl, false),
                (Breakpoint.MdAndDown, Breakpoint.Xxl, false),

                // LgAndDown
                (Breakpoint.LgAndDown, Breakpoint.Xs, true),
                (Breakpoint.LgAndDown, Breakpoint.Sm, true),
                (Breakpoint.LgAndDown, Breakpoint.Md, true),
                (Breakpoint.LgAndDown, Breakpoint.Lg, true),
                (Breakpoint.LgAndDown, Breakpoint.Xl, false),
                (Breakpoint.LgAndDown, Breakpoint.Xxl, false),

                // XlAndDown
                (Breakpoint.XlAndDown, Breakpoint.Xs, true),
                (Breakpoint.XlAndDown, Breakpoint.Sm, true),
                (Breakpoint.XlAndDown, Breakpoint.Md, true),
                (Breakpoint.XlAndDown, Breakpoint.Lg, true),
                (Breakpoint.XlAndDown, Breakpoint.Xl, true),
                (Breakpoint.XlAndDown, Breakpoint.Xxl, false),

                // SmAndUp
                (Breakpoint.SmAndUp, Breakpoint.Xs, false),
                (Breakpoint.SmAndUp, Breakpoint.Sm, true),
                (Breakpoint.SmAndUp, Breakpoint.Md, true),
                (Breakpoint.SmAndUp, Breakpoint.Lg, true),
                (Breakpoint.SmAndUp, Breakpoint.Xl, true),
                (Breakpoint.SmAndUp, Breakpoint.Xxl, true),

                // MdAndUp
                (Breakpoint.MdAndUp, Breakpoint.Xs, false),
                (Breakpoint.MdAndUp, Breakpoint.Sm, false),
                (Breakpoint.MdAndUp, Breakpoint.Md, true),
                (Breakpoint.MdAndUp, Breakpoint.Lg, true),
                (Breakpoint.MdAndUp, Breakpoint.Xl, true),
                (Breakpoint.MdAndUp, Breakpoint.Xxl, true),

                // LgAndUp
                (Breakpoint.LgAndUp, Breakpoint.Xs, false),
                (Breakpoint.LgAndUp, Breakpoint.Sm, false),
                (Breakpoint.LgAndUp, Breakpoint.Md, false),
                (Breakpoint.LgAndUp, Breakpoint.Lg, true),
                (Breakpoint.LgAndUp, Breakpoint.Xl, true),
                (Breakpoint.LgAndUp, Breakpoint.Xxl, true),

                // XlAndUp
                (Breakpoint.XlAndUp, Breakpoint.Xs, false),
                (Breakpoint.XlAndUp, Breakpoint.Sm, false),
                (Breakpoint.XlAndUp, Breakpoint.Md, false),
                (Breakpoint.XlAndUp, Breakpoint.Lg, false),
                (Breakpoint.XlAndUp, Breakpoint.Xl, true),
                (Breakpoint.XlAndUp, Breakpoint.Xxl, true),

                // None
                (Breakpoint.None, Breakpoint.Xs, false),
                (Breakpoint.None, Breakpoint.Sm, false),
                (Breakpoint.None, Breakpoint.Md, false),
                (Breakpoint.None, Breakpoint.Lg, false),
                (Breakpoint.None, Breakpoint.Xl, false),
                (Breakpoint.None, Breakpoint.Xxl, false),

                // Always
                (Breakpoint.Always, Breakpoint.Xs, true),
                (Breakpoint.Always, Breakpoint.Sm, true),
                (Breakpoint.Always, Breakpoint.Md, true),
                (Breakpoint.Always, Breakpoint.Lg, true),
                (Breakpoint.Always, Breakpoint.Xl, true),
                (Breakpoint.Always, Breakpoint.Xxl, true),

                // Invalid
                ((Breakpoint)(-1), (Breakpoint)(-1), false),
            };
        }
    }
}
