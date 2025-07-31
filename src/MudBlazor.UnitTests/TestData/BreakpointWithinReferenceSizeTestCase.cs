// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Castle.Components.DictionaryAdapter.Xml;
using NUnit.Framework;

namespace MudBlazor.UnitTests.TestData
{
    public static class BreakpointWithinReferenceSizeTestCase
    {
        public static TestCaseData[] AllCombinations()
        {
            return new[]
            {
                // Xs
                new TestCaseData(Breakpoint.Xs, Breakpoint.Xs, true),
                new TestCaseData(Breakpoint.Xs, Breakpoint.Sm, false),
                new TestCaseData(Breakpoint.Xs, Breakpoint.Md, false),
                new TestCaseData(Breakpoint.Xs, Breakpoint.Lg, false),
                new TestCaseData(Breakpoint.Xs, Breakpoint.Xl, false),
                new TestCaseData(Breakpoint.Xs, Breakpoint.Xxl, false),

                // Sm
                new TestCaseData(Breakpoint.Sm, Breakpoint.Xs, false),
                new TestCaseData(Breakpoint.Sm, Breakpoint.Sm, true),
                new TestCaseData(Breakpoint.Sm, Breakpoint.Md, false),
                new TestCaseData(Breakpoint.Sm, Breakpoint.Lg, false),
                new TestCaseData(Breakpoint.Sm, Breakpoint.Xl, false),
                new TestCaseData(Breakpoint.Sm, Breakpoint.Xxl, false),

                // Md
                new TestCaseData(Breakpoint.Md, Breakpoint.Xs, false),
                new TestCaseData(Breakpoint.Md, Breakpoint.Sm, false),
                new TestCaseData(Breakpoint.Md, Breakpoint.Md, true),
                new TestCaseData(Breakpoint.Md, Breakpoint.Lg, false),
                new TestCaseData(Breakpoint.Md, Breakpoint.Xl, false),
                new TestCaseData(Breakpoint.Md, Breakpoint.Xxl, false),

                // Lg
                new TestCaseData(Breakpoint.Lg, Breakpoint.Xs, false),
                new TestCaseData(Breakpoint.Lg, Breakpoint.Sm, false),
                new TestCaseData(Breakpoint.Lg, Breakpoint.Md, false),
                new TestCaseData(Breakpoint.Lg, Breakpoint.Lg, true),
                new TestCaseData(Breakpoint.Lg, Breakpoint.Xl, false),
                new TestCaseData(Breakpoint.Lg, Breakpoint.Xxl, false),

                // Xl
                new TestCaseData(Breakpoint.Xl, Breakpoint.Xs, false),
                new TestCaseData(Breakpoint.Xl, Breakpoint.Sm, false),
                new TestCaseData(Breakpoint.Xl, Breakpoint.Md, false),
                new TestCaseData(Breakpoint.Xl, Breakpoint.Lg, false),
                new TestCaseData(Breakpoint.Xl, Breakpoint.Xl, true),
                new TestCaseData(Breakpoint.Xl, Breakpoint.Xxl, false),

                // Xxl
                new TestCaseData(Breakpoint.Xxl, Breakpoint.Xs, false),
                new TestCaseData(Breakpoint.Xxl, Breakpoint.Sm, false),
                new TestCaseData(Breakpoint.Xxl, Breakpoint.Md, false),
                new TestCaseData(Breakpoint.Xxl, Breakpoint.Lg, false),
                new TestCaseData(Breakpoint.Xxl, Breakpoint.Xl, false),
                new TestCaseData(Breakpoint.Xxl, Breakpoint.Xxl, true),

                // SmAndDown
                new TestCaseData(Breakpoint.SmAndDown, Breakpoint.Xs, true),
                new TestCaseData(Breakpoint.SmAndDown, Breakpoint.Sm, true),
                new TestCaseData(Breakpoint.SmAndDown, Breakpoint.Md, false),
                new TestCaseData(Breakpoint.SmAndDown, Breakpoint.Lg, false),
                new TestCaseData(Breakpoint.SmAndDown, Breakpoint.Xl, false),
                new TestCaseData(Breakpoint.SmAndDown, Breakpoint.Xxl, false),

                // MdAndDown
                new TestCaseData(Breakpoint.MdAndDown, Breakpoint.Xs, true),
                new TestCaseData(Breakpoint.MdAndDown, Breakpoint.Sm, true),
                new TestCaseData(Breakpoint.MdAndDown, Breakpoint.Md, true),
                new TestCaseData(Breakpoint.MdAndDown, Breakpoint.Lg, false),
                new TestCaseData(Breakpoint.MdAndDown, Breakpoint.Xl, false),
                new TestCaseData(Breakpoint.MdAndDown, Breakpoint.Xxl, false),

                // LgAndDown
                new TestCaseData(Breakpoint.LgAndDown, Breakpoint.Xs, true),
                new TestCaseData(Breakpoint.LgAndDown, Breakpoint.Sm, true),
                new TestCaseData(Breakpoint.LgAndDown, Breakpoint.Md, true),
                new TestCaseData(Breakpoint.LgAndDown, Breakpoint.Lg, true),
                new TestCaseData(Breakpoint.LgAndDown, Breakpoint.Xl, false),
                new TestCaseData(Breakpoint.LgAndDown, Breakpoint.Xxl, false),

                // XlAndDown
                new TestCaseData(Breakpoint.XlAndDown, Breakpoint.Xs, true),
                new TestCaseData(Breakpoint.XlAndDown, Breakpoint.Sm, true),
                new TestCaseData(Breakpoint.XlAndDown, Breakpoint.Md, true),
                new TestCaseData(Breakpoint.XlAndDown, Breakpoint.Lg, true),
                new TestCaseData(Breakpoint.XlAndDown, Breakpoint.Xl, true),
                new TestCaseData(Breakpoint.XlAndDown, Breakpoint.Xxl, false),

                // SmAndUp
                new TestCaseData(Breakpoint.SmAndUp, Breakpoint.Xs, false),
                new TestCaseData(Breakpoint.SmAndUp, Breakpoint.Sm, true),
                new TestCaseData(Breakpoint.SmAndUp, Breakpoint.Md, true),
                new TestCaseData(Breakpoint.SmAndUp, Breakpoint.Lg, true),
                new TestCaseData(Breakpoint.SmAndUp, Breakpoint.Xl, true),
                new TestCaseData(Breakpoint.SmAndUp, Breakpoint.Xxl, true),

                // MdAndUp
                new TestCaseData(Breakpoint.MdAndUp, Breakpoint.Xs, false),
                new TestCaseData(Breakpoint.MdAndUp, Breakpoint.Sm, false),
                new TestCaseData(Breakpoint.MdAndUp, Breakpoint.Md, true),
                new TestCaseData(Breakpoint.MdAndUp, Breakpoint.Lg, true),
                new TestCaseData(Breakpoint.MdAndUp, Breakpoint.Xl, true),
                new TestCaseData(Breakpoint.MdAndUp, Breakpoint.Xxl, true),

                // LgAndUp
                new TestCaseData(Breakpoint.LgAndUp, Breakpoint.Xs, false),
                new TestCaseData(Breakpoint.LgAndUp, Breakpoint.Sm, false),
                new TestCaseData(Breakpoint.LgAndUp, Breakpoint.Md, false),
                new TestCaseData(Breakpoint.LgAndUp, Breakpoint.Lg, true),
                new TestCaseData(Breakpoint.LgAndUp, Breakpoint.Xl, true),
                new TestCaseData(Breakpoint.LgAndUp, Breakpoint.Xxl, true),

                // XlAndUp
                new TestCaseData(Breakpoint.XlAndUp, Breakpoint.Xs, false),
                new TestCaseData(Breakpoint.XlAndUp, Breakpoint.Sm, false),
                new TestCaseData(Breakpoint.XlAndUp, Breakpoint.Md, false),
                new TestCaseData(Breakpoint.XlAndUp, Breakpoint.Lg, false),
                new TestCaseData(Breakpoint.XlAndUp, Breakpoint.Xl, true),
                new TestCaseData(Breakpoint.XlAndUp, Breakpoint.Xxl, true),

                // None
                new TestCaseData(Breakpoint.None, Breakpoint.Xs, false),
                new TestCaseData(Breakpoint.None, Breakpoint.Sm, false),
                new TestCaseData(Breakpoint.None, Breakpoint.Md, false),
                new TestCaseData(Breakpoint.None, Breakpoint.Lg, false),
                new TestCaseData(Breakpoint.None, Breakpoint.Xl, false),
                new TestCaseData(Breakpoint.None, Breakpoint.Xxl, false),

                // Always
                new TestCaseData(Breakpoint.Always, Breakpoint.Xs, true),
                new TestCaseData(Breakpoint.Always, Breakpoint.Sm, true),
                new TestCaseData(Breakpoint.Always, Breakpoint.Md, true),
                new TestCaseData(Breakpoint.Always, Breakpoint.Lg, true),
                new TestCaseData(Breakpoint.Always, Breakpoint.Xl, true),
                new TestCaseData(Breakpoint.Always, Breakpoint.Xxl, true),

                // Invalid
                new TestCaseData((Breakpoint)(-1), (Breakpoint)(-1), false),
            };
        }
    }
}
