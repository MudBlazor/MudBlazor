// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Bunit;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class MatrixItemTests : BunitTest
    {
        [Test]
        public void MatrixItem_CheckDefaultValues()
        {
            var matrixItem = new MudMatrixItem();

            matrixItem.ColumnSpan.Should().Be(1);
            matrixItem.RowSpan.Should().Be(1);
            matrixItem.ColumnSpanBackward.Should().Be(false);
            matrixItem.RowSpanBackward.Should().Be(false);
            matrixItem.ColumnPosition.Should().BeNull();
            matrixItem.RowPosition.Should().BeNull();
        }

        [Test]
        public void MatrixItem_CheckDefaultClass()
        {
            var matrixItem = Context.Render<MudMatrixItem>();

            var matrixItemClass = matrixItem.Find(".mud-matrix-item");
            matrixItemClass.ClassList.Should().Contain("mud-matrix-item");
        }

        [Test]
        public void MatrixItem_CheckDefaultStyle()
        {
            var matrixItem = Context.Render<MudMatrixItem>();

            var matrixItemStyle = matrixItem.Find(".mud-matrix-item");
            matrixItemStyle.GetAttribute("style").Should().Contain("grid-column:span 1");
            matrixItemStyle.GetAttribute("style").Should().Contain("grid-row:span 1");
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(10)]
        [TestCase(25)]
        [TestCase(50)]
        public void MatrixItem_ColumnSpan_ReturnsCorrectStyle(int columnSpan)
        {
            var matrixItem = Context.Render<MudMatrixItem>(x => x.Add(c => c.ColumnSpan, columnSpan));

            var matrixItemStyle = matrixItem.Find(".mud-matrix-item");
            matrixItemStyle.GetAttribute("style").Should().Contain($"grid-column:span {columnSpan}");
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(10)]
        [TestCase(25)]
        [TestCase(50)]
        public void MatrixItem_RowSpan_ReturnsCorrectStyle(int rowSpan)
        {
            var matrixItem = Context.Render<MudMatrixItem>(x => x.Add(c => c.RowSpan, rowSpan));

            var matrixItemStyle = matrixItem.Find(".mud-matrix-item");
            matrixItemStyle.GetAttribute("style").Should().Contain($"grid-row:span {rowSpan}");
        }

        [Test]
        [TestCase(1, 1)]
        [TestCase(1, 3)]
        [TestCase(3, 1)]
        [TestCase(10, 5)]
        public void MatrixItem_ColumnPosition_ReturnsCorrectStyle(int columnSpan, int columnPosition)
        {
            var matrixItem = Context.Render<MudMatrixItem>(x => x
                .Add(c => c.ColumnSpan, columnSpan)
                .Add(c => c.ColumnPosition, columnPosition));

            var matrixItemStyle = matrixItem.Find(".mud-matrix-item");
            matrixItemStyle.GetAttribute("style").Should().Contain($"grid-column:{columnPosition} / span {columnSpan}");
        }

        [Test]
        [TestCase(1, 1)]
        [TestCase(1, 3)]
        [TestCase(3, 1)]
        [TestCase(10, 5)]
        public void MatrixItem_ColumnPosition_ReturnsCorrectStyle_Backward(int columnSpan, int columnPosition)
        {
            var matrixItem = Context.Render<MudMatrixItem>(x => x
                .Add(c => c.ColumnSpan, columnSpan)
                .Add(c => c.ColumnPosition, columnPosition)
                .Add(c => c.ColumnSpanBackward, true));

            var matrixItemStyle = matrixItem.Find(".mud-matrix-item");
            matrixItemStyle.GetAttribute("style").Should().Contain($"grid-column:span {columnSpan} / {columnPosition}");
        }

        [Test]
        [TestCase(1, 1)]
        [TestCase(1, 3)]
        [TestCase(3, 1)]
        [TestCase(10, 5)]
        public void MatrixItem_RowPosition_ReturnsCorrectStyle(int rowSpan, int rowPosition)
        {
            var matrixItem = Context.Render<MudMatrixItem>(x => x
                .Add(c => c.RowSpan, rowSpan)
                .Add(c => c.RowPosition, rowPosition));

            var matrixItemStyle = matrixItem.Find(".mud-matrix-item");
            matrixItemStyle.GetAttribute("style").Should().Contain($"grid-row:{rowPosition} / span {rowSpan}");
        }

        [Test]
        [TestCase(1, 1)]
        [TestCase(1, 3)]
        [TestCase(3, 1)]
        [TestCase(10, 5)]
        public void MatrixItem_RowPosition_ReturnsCorrectStyle_Backward(int rowSpan, int rowPosition)
        {
            var matrixItem = Context.Render<MudMatrixItem>(x => x
                .Add(c => c.RowSpan, rowSpan)
                .Add(c => c.RowPosition, rowPosition)
                .Add(c => c.RowSpanBackward, true));

            var matrixItemStyle = matrixItem.Find(".mud-matrix-item");
            matrixItemStyle.GetAttribute("style").Should().Contain($"grid-row:span {rowSpan} / {rowPosition}");
        }

        [Test]
        [TestCase(-1)]
        [TestCase(-3)]
        [TestCase(-10)]
        public void MatrixItem_ColumnPosition_ReturnsCorrectStyle_Negative(int columnPosition)
        {
            var matrixItem = Context.Render<MudMatrixItem>(x => x.Add(c => c.ColumnPosition, columnPosition));

            var matrixItemStyle = matrixItem.Find(".mud-matrix-item");
            matrixItemStyle.GetAttribute("style").Should().Contain($"grid-column:{columnPosition} / span 1");
        }

        [Test]
        [TestCase(-1)]
        [TestCase(-3)]
        [TestCase(-10)]
        public void MatrixItem_RowPosition_ReturnsCorrectStyle_Negative(int rowPosition)
        {
            var matrixItem = Context.Render<MudMatrixItem>(x => x.Add(c => c.RowPosition, rowPosition));

            var matrixItemStyle = matrixItem.Find(".mud-matrix-item");
            matrixItemStyle.GetAttribute("style").Should().Contain($"grid-row:{rowPosition} / span 1");
        }
    }
}
