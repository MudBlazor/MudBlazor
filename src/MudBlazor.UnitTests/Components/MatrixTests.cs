// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Bunit;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class MatrixTests : BunitTest
    {
        [Test]
        public void Matrix_CheckDefaultValues()
        {
            var matrix = new MudMatrix();

            matrix.ColumnGap.Should().Be(6);
            matrix.RowGap.Should().Be(6);
            matrix.ExplicitColumns.ToString().Should().Be("none");
            matrix.ExplicitRows.ToString().Should().Be("none");
            matrix.ImplicitColumns.ToString().Should().Be("auto");
            matrix.ImplicitRows.ToString().Should().Be("auto");
            matrix.JustifyColumns.Should().Be(MatrixJustify.Start);
            matrix.JustifyRows.Should().Be(MatrixJustify.Start);
            matrix.HorizontalFlow.Should().Be(false);
        }

        [Test]
        public void Matrix_CheckDefaultClass()
        {
            var matrix = Context.Render<MudMatrix>();

            var matrixClass = matrix.Find(".mud-matrix");
            matrixClass.ClassList.Should().ContainInOrder(
            [
                "mud-matrix",
                "gap-y-6",
                "gap-x-6",
                "mud-matrix-justify-rows-start",
                "mud-matrix-justify-columns-start",
            ]);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        [TestCase(8)]
        [TestCase(9)]
        [TestCase(10)]
        [TestCase(11)]
        [TestCase(12)]
        [TestCase(13)]
        [TestCase(14)]
        [TestCase(15)]
        [TestCase(16)]
        [TestCase(17)]
        [TestCase(18)]
        [TestCase(19)]
        [TestCase(20)]
        public void Matrix_CheckColumnGapClass(int gap)
        {
            var matrix = Context.Render<MudMatrix>(x => x.Add(c => c.ColumnGap, gap));

            var matrixClass = matrix.Find(".mud-matrix");
            matrixClass.ClassList.Should().Contain($"gap-x-{gap}");
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        [TestCase(8)]
        [TestCase(9)]
        [TestCase(10)]
        [TestCase(11)]
        [TestCase(12)]
        [TestCase(13)]
        [TestCase(14)]
        [TestCase(15)]
        [TestCase(16)]
        [TestCase(17)]
        [TestCase(18)]
        [TestCase(19)]
        [TestCase(20)]
        public void Matrix_CheckRowGapClass(int gap)
        {
            var matrix = Context.Render<MudMatrix>(x => x.Add(c => c.RowGap, gap));

            var matrixClass = matrix.Find(".mud-matrix");
            matrixClass.ClassList.Should().Contain($"gap-y-{gap}");
        }

        [Test]
        [TestCase(MatrixJustify.Start, "start")]
        [TestCase(MatrixJustify.Center, "center")]
        [TestCase(MatrixJustify.End, "end")]
        [TestCase(MatrixJustify.SpaceBetween, "space-between")]
        [TestCase(MatrixJustify.SpaceAround, "space-around")]
        [TestCase(MatrixJustify.SpaceEvenly, "space-evenly")]
        public void Matrix_CheckJustifyColumnsClass(MatrixJustify justify, string expectedClass)
        {
            var matrix = Context.Render<MudMatrix>(x => x.Add(c => c.JustifyColumns, justify));

            var matrixClass = matrix.Find(".mud-matrix");
            matrixClass.ClassList.Should().Contain($"mud-matrix-justify-columns-{expectedClass}");
        }


        [Test]
        [TestCase(MatrixJustify.Start, "start")]
        [TestCase(MatrixJustify.Center, "center")]
        [TestCase(MatrixJustify.End, "end")]
        [TestCase(MatrixJustify.SpaceBetween, "space-between")]
        [TestCase(MatrixJustify.SpaceAround, "space-around")]
        [TestCase(MatrixJustify.SpaceEvenly, "space-evenly")]
        public void Matrix_CheckJustifyRowsClass(MatrixJustify justify, string expectedClass)
        {
            var matrix = Context.Render<MudMatrix>(x => x.Add(c => c.JustifyRows, justify));

            var matrixClass = matrix.Find(".mud-matrix");
            matrixClass.ClassList.Should().Contain($"mud-matrix-justify-rows-{expectedClass}");
        }

        [Test]
        [TestCase(false, "row")]
        [TestCase(true, "column")]
        public void Matrix_CheckHorizontalFlowStyle(bool horizontalFlow, string expectedFlow)
        {
            var matrix = Context.Render<MudMatrix>(x => x.Add(c => c.HorizontalFlow, horizontalFlow));

            var matrixStyle = matrix.Find(".mud-matrix");
            matrixStyle.GetAttribute("style").Should().Contain($"grid-auto-flow:{expectedFlow}");
        }

        public record ExplicitTestCase(ExplicitMatrix ExplicitMatrix, string ExpectedValue);

        private static IEnumerable<ExplicitTestCase> ExplicitCases()
        {
            yield return new ExplicitTestCase(ExplicitMatrix.Pattern(Units.Px(50)), "50px");
            yield return new ExplicitTestCase(ExplicitMatrix.Pattern(Units.Px(50), Units.Rem(10)), "50px 10rem");
            yield return new ExplicitTestCase(ExplicitMatrix.Pattern(3, Units.Px(50)), "repeat(3, 50px)");
            yield return new ExplicitTestCase(ExplicitMatrix.Fit(Units.Px(50)), "repeat(auto-fit, 50px)");
            yield return new ExplicitTestCase(ExplicitMatrix.Fill(Units.Px(50)), "repeat(auto-fill, 50px)");
        }

        [Test]
        public void Matrix_CheckExplicitColumnsStyle()
        {
            foreach (var testCase in ExplicitCases())
            {
                var matrix = Context.Render<MudMatrix>(x => x.Add(c => c.ExplicitColumns, testCase.ExplicitMatrix));

                var style = matrix.Find(".mud-matrix").GetAttribute("style");
                style.Should().Contain($"grid-template-columns:{testCase.ExpectedValue}");
            }
        }

        [Test]
        public void Matrix_CheckExplicitRowsStyle()
        {
            foreach (var testCase in ExplicitCases())
            {
                var matrix = Context.Render<MudMatrix>(x => x.Add(c => c.ExplicitRows, testCase.ExplicitMatrix));

                var style = matrix.Find(".mud-matrix").GetAttribute("style");
                style.Should().Contain($"grid-template-rows:{testCase.ExpectedValue}");
            }
        }

        public record ImplicitTestCase(ImplicitMatrix ImplicitMatrix, string ExpectedValue);

        private static IEnumerable<ImplicitTestCase> ImplicitCases()
        {
            yield return new ImplicitTestCase(ImplicitMatrix.Pattern(Units.Px(50)), "50px");
            yield return new ImplicitTestCase(ImplicitMatrix.Pattern(Units.Px(50), Units.Rem(10)), "50px 10rem");
        }

        [Test]
        public void Matrix_CheckImplicitColumnsStyle()
        {
            foreach (var testCase in ImplicitCases())
            {
                var matrix = Context.Render<MudMatrix>(x => x.Add(c => c.ImplicitColumns, testCase.ImplicitMatrix));

                var style = matrix.Find(".mud-matrix").GetAttribute("style");
                style.Should().Contain($"grid-auto-columns:{testCase.ExpectedValue}");
            }
        }

        [Test]
        public void Matrix_CheckImplicitRowsStyle()
        {
            foreach (var testCase in ImplicitCases())
            {
                var matrix = Context.Render<MudMatrix>(x => x.Add(c => c.ImplicitRows, testCase.ImplicitMatrix));

                var style = matrix.Find(".mud-matrix").GetAttribute("style");
                style.Should().Contain($"grid-auto-rows:{testCase.ExpectedValue}");
            }
        }

        [TestCase(1, "1px")]
        [TestCase(10, "10px")]
        [TestCase(25, "25px")]
        [TestCase(50, "50px")]
        public void ExplicitMatrix_Pattern_ReturnsCorrectString(double px, string expected)
        {
            ExplicitMatrix.Pattern(Units.Px(px)).ToString().Should().Be(expected);
        }

        [TestCase(1, "repeat(1, 50px)")]
        [TestCase(10, "repeat(10, 50px)")]
        [TestCase(25, "repeat(25, 50px)")]
        [TestCase(50, "repeat(50, 50px)")]
        public void ExplicitMatrix_Pattern_Repeated_ReturnsCorrectString(int count, string expected)
        {
            ExplicitMatrix.Pattern(count, Units.Px(50)).ToString().Should().Be(expected);
        }

        [TestCase(1, "repeat(auto-fit, 1px)")]
        [TestCase(10, "repeat(auto-fit, 10px)")]
        [TestCase(25, "repeat(auto-fit, 25px)")]
        [TestCase(50, "repeat(auto-fit, 50px)")]
        public void ExplicitMatrix_Fit_ReturnsCorrectString(double px, string expected)
        {
            ExplicitMatrix.Fit(Units.Px(px)).ToString().Should().Be(expected);
        }

        [TestCase(1, "repeat(auto-fill, 1px)")]
        [TestCase(10, "repeat(auto-fill, 10px)")]
        [TestCase(25, "repeat(auto-fill, 25px)")]
        [TestCase(50, "repeat(auto-fill, 50px)")]
        public void ExplicitMatrix_Fill_ReturnsCorrectString(double px, string expected)
        {
            ExplicitMatrix.Fill(Units.Px(px)).ToString().Should().Be(expected);
        }

        [TestCase(1, "1px")]
        [TestCase(10, "10px")]
        [TestCase(25, "25px")]
        [TestCase(50, "50px")]
        public void ImplicitMatrix_Pattern_ReturnsCorrectString(double px, string expected)
        {
            ImplicitMatrix.Pattern(Units.Px(px)).ToString().Should().Be(expected);
        }
    }
}
