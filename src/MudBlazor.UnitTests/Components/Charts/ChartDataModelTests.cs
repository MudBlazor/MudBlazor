// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AwesomeAssertions;
using MudBlazor.Charts;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Charts;

[TestFixture]
public class ChartDataModelTests : BunitTest
{
    #region ChartData ctors

    [Test]
    public void ChartData_SingleValueCtor_HasOnePointWithNullX()
    {
        var data = new ChartData<double>(5.0);

        data.Count.Should().Be(1);
        data[0].X.Should().BeNull();
        data.GetValue(0).Should().Be(5.0);
        data.Values.Should().ContainSingle().Which.Should().Be(5.0);
    }

    [Test]
    public void ChartData_ReadOnlyListCtor_MapsValues()
    {
        IReadOnlyList<double> values = new List<double> { 1.0, 2.0, 3.0 };
        var data = new ChartData<double>(values);

        data.Count.Should().Be(3);
        data.Values.Should().Equal(1.0, 2.0, 3.0);
        data.Points.All(p => p.X is null).Should().BeTrue();
    }

    [Test]
    public void ChartData_DefaultCtor_IsEmpty()
    {
        var data = new ChartData<double>();

        data.Count.Should().Be(0);
        data.Points.Should().BeEmpty();
        data.Values.Should().BeEmpty();
    }

    [Test]
    public void ChartData_SinglePointCtor_SetsXAndY()
    {
        var data = new ChartData<double>((3.0, 4.0));

        data.Count.Should().Be(1);
        data[0].X.Should().Be(3.0);
        data[0].Y.Should().Be(4.0);
    }

    [Test]
    public void ChartData_PointListCtor_MapsXY()
    {
        IReadOnlyList<(double x, double y)> points = new List<(double, double)> { (1.0, 2.0), (3.0, 4.0) };
        var data = new ChartData<double>(points);

        data.Count.Should().Be(2);
        data[0].X.Should().Be(1.0);
        data[1].Y.Should().Be(4.0);
    }

    [Test]
    public void ChartData_DateTimeValueCtor_SetsXAndY()
    {
        var dt = new DateTime(2024, 1, 1, 12, 0, 0);
        var data = new ChartData<double>(dt, 99.0);

        data.Count.Should().Be(1);
        data[0].X.Should().Be(dt);
        data[0].Y.Should().Be(99.0);
    }

    [Test]
    public void ChartData_DateTimeTupleListCtor_MapsValues()
    {
        var dt1 = new DateTime(2024, 3, 1);
        var dt2 = new DateTime(2024, 3, 2);
        IReadOnlyList<(DateTime, double)> values = new List<(DateTime, double)> { (dt1, 1.0), (dt2, 2.0) };
        var data = new ChartData<double>(values);

        data.Count.Should().Be(2);
        data[0].X.Should().Be(dt1);
        data[1].Y.Should().Be(2.0);
    }

    [Test]
    public void ChartData_SankeyLinkWeightCtor_SetsXAndY()
    {
        var link = new SankeyLink("A", "B");
        var data = new ChartData<double>(link, 7.0);

        data.Count.Should().Be(1);
        data[0].X.Should().Be(link);
        data[0].Y.Should().Be(7.0);
    }

    [Test]
    public void ChartData_SankeyEdgeTupleListCtor_MapsValues()
    {
        var link1 = new SankeyLink("A", "B");
        var link2 = new SankeyLink("B", "C");
        IReadOnlyList<(SankeyLink, double)> edges = new List<(SankeyLink, double)> { (link1, 1.0), (link2, 2.0) };
        var data = new ChartData<double>(edges);

        data.Count.Should().Be(2);
        data[0].X.Should().Be(link1);
        data[1].Y.Should().Be(2.0);
    }

    [Test]
    public void ChartData_StringEdgeTupleCtor_BuildsSankeyLink()
    {
        var data = new ChartData<double>(("Source", "Target", 9.0));

        data.Count.Should().Be(1);
        data[0].X.Should().Be(new SankeyLink("Source", "Target"));
        data[0].Y.Should().Be(9.0);
    }

    #endregion

    #region ChartData enumerators

    [Test]
    public void ChartData_GenericEnumerator_YieldsValues()
    {
        var data = new ChartData<double>(new List<double> { 10.0, 20.0 });

        var collected = new List<double>();
        foreach (var v in data)
        {
            collected.Add(v);
        }

        collected.Should().Equal(10.0, 20.0);
    }

    [Test]
    public void ChartData_NonGenericEnumerator_YieldsValues()
    {
        var data = new ChartData<double>(new List<double> { 1.5, 2.5 });

        IEnumerable enumerable = data;
        var collected = new List<double>();
        foreach (var v in enumerable)
        {
            collected.Add((double)v);
        }

        collected.Should().Equal(1.5, 2.5);
    }

    #endregion

    #region ChartData implicit conversions

    [Test]
    public void ChartData_ImplicitFromValue()
    {
        ChartData<double> single = 7.0;
        single.Values.Should().Equal(7.0);
        single.Points.Should().OnlyContain(p => p.X == null);

        ChartData<double> array = new[] { 1.0, 2.0 };
        array.Values.Should().Equal(1.0, 2.0);

        ChartData<double> list = new List<double> { 4.0, 5.0, 6.0 };
        list.Values.Should().Equal(4.0, 5.0, 6.0);
    }

    [Test]
    public void ChartData_ImplicitFromScatterPoint()
    {
        ChartData<double> array = new[] { (1.0, 10.0), (2.0, 20.0) };
        array.Count.Should().Be(2);
        array[1].X.Should().Be(2.0);
        array[1].Y.Should().Be(20.0);

        ChartData<double> list = new List<(double x, double y)> { (5.0, 50.0) };
        list[0].X.Should().Be(5.0);
        list[0].Y.Should().Be(50.0);
    }

    [Test]
    public void ChartData_ImplicitFromTimeValue()
    {
        var dt = new DateTime(2024, 4, 1);

        ChartData<double> array = new[] { (dt, 3.0) };
        array[0].X.Should().Be(dt);
        array[0].Y.Should().Be(3.0);

        ChartData<double> list = new List<(DateTime, double)> { (dt, 7.0) };
        list[0].X.Should().Be(dt);
        list[0].Y.Should().Be(7.0);

        ChartData<double> single = new TimeValue<double>(dt, 12.0);
        single[0].X.Should().Be(dt);
        single[0].Y.Should().Be(12.0);

        ChartData<double> timeValueArray = new[] { new TimeValue<double>(dt, 8.0) };
        timeValueArray[0].X.Should().Be(dt);
        timeValueArray[0].Y.Should().Be(8.0);
    }

    [Test]
    public void ChartData_ImplicitFromSankey()
    {
        var link = new SankeyLink("X", "Y");

        ChartData<double> tuple = (link, 3.0);
        tuple[0].X.Should().Be(link);
        tuple[0].Y.Should().Be(3.0);

        ChartData<double> tupleList = new List<(SankeyLink, double)> { (link, 4.0) };
        tupleList[0].X.Should().Be(link);
        tupleList[0].Y.Should().Be(4.0);

        ChartData<double> edge = new SankeyEdge<double>("A", "B", 5.0);
        edge[0].X.Should().Be(new SankeyLink("A", "B"));
        edge[0].Y.Should().Be(5.0);

        ChartData<double> edgeArray = new[] { new SankeyEdge<double>("A", "B", 6.0) };
        edgeArray[0].X.Should().Be(new SankeyLink("A", "B"));
        edgeArray[0].Y.Should().Be(6.0);
    }

    #endregion

    #region ChartPoint

    [Test]
    public void ChartPoint_SingleYCtor_SetsYAndNullX()
    {
        var point = new ChartPoint<double>(42.0);

        point.X.Should().BeNull();
        point.Y.Should().Be(42.0);
    }

    [Test]
    public void ChartPoint_XYCtor_SetsBoth()
    {
        var point = new ChartPoint<double>("label", 1.0);

        point.X.Should().Be("label");
        point.Y.Should().Be(1.0);
    }

    [TestCaseSource(nameof(ChartPointConversionCases))]
    public void ChartPoint_ImplicitConversion_SetsXAndY(ChartPoint<double> point, object expectedX, double expectedY)
    {
        point.X.Should().Be(expectedX);
        point.Y.Should().Be(expectedY);
    }

    public static IEnumerable<TestCaseData> ChartPointConversionCases()
    {
        var link = new SankeyLink("A", "B");
        var dt = new DateTime(2024, 9, 1);

        yield return new TestCaseData((ChartPoint<double>)(link, 3.0), link, 3.0).SetName("ChartPoint_ImplicitFromSankeyTuple");
        yield return new TestCaseData((ChartPoint<double>)(dt, 4.0), dt, 4.0).SetName("ChartPoint_ImplicitFromDateTimeTuple");
        yield return new TestCaseData((ChartPoint<double>)(5.0, 6.0), (object)5.0, 6.0).SetName("ChartPoint_ImplicitFromNumericTuple");
        yield return new TestCaseData((ChartPoint<double>)8.0, null, 8.0).SetName("ChartPoint_ImplicitFromValue");
    }

    #endregion

    #region ChartSeries

    [Test]
    public void ChartSeries_ValuesCtor_SetsData()
    {
        var series = new ChartSeries<double>(new List<double> { 1.0, 2.0, 3.0 });

        series.Data.Values.Should().Equal(1.0, 2.0, 3.0);
        series.Visible.Should().BeTrue();
        series.Name.Should().BeEmpty();
    }

    [Test]
    public void ChartSeries_ImplicitFromArray_SetsData()
    {
        ChartSeries<double> series = new[] { 1.0, 2.0, 3.0 };

        series.Data.Values.Should().Equal(1.0, 2.0, 3.0);
    }

    [Test]
    public void ChartSeries_Equals_SameNameAndValues_IsTrue()
    {
        var a = new ChartSeries<double> { Name = "S", Data = new[] { 1.0, 2.0 } };
        var b = new ChartSeries<double> { Name = "S", Data = new[] { 1.0, 2.0 } };

        a.Equals(b).Should().BeTrue();
        a.Equals((object)b).Should().BeTrue();
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [TestCaseSource(nameof(ChartSeriesInequalityCases))]
    public void ChartSeries_Equals_DifferingSeries_IsFalse(ChartSeries<double> other)
    {
        var a = new ChartSeries<double> { Name = "S", Data = new[] { 1.0, 2.0 } };

        a.Equals(other).Should().BeFalse();
    }

    public static IEnumerable<TestCaseData> ChartSeriesInequalityCases()
    {
        yield return new TestCaseData((ChartSeries<double>)null).SetName("ChartSeries_Equals_Null_IsFalse");
        yield return new TestCaseData(new ChartSeries<double> { Name = "S", Data = new[] { 1.0 } }).SetName("ChartSeries_Equals_DifferentCount_IsFalse");
        yield return new TestCaseData(new ChartSeries<double> { Name = "X", Data = new[] { 1.0, 2.0 } }).SetName("ChartSeries_Equals_DifferentName_IsFalse");
        yield return new TestCaseData(new ChartSeries<double> { Name = "S", Data = new[] { 1.0, 9.0 } }).SetName("ChartSeries_Equals_DifferentValues_IsFalse");
    }

    [Test]
    public void ChartSeries_GetHashCode_ManyValues_OnlyHashesFirstTen()
    {
        // Only the first ten values participate in the hash, so two series differing
        // beyond index ten still collide on hash code.
        var first = Enumerable.Range(0, 15).Select(i => (double)i).ToArray();
        var second = Enumerable.Range(0, 10).Select(i => (double)i).Concat(Enumerable.Repeat(-1.0, 5)).ToArray();
        var a = new ChartSeries<double> { Name = "S", Data = first };
        var b = new ChartSeries<double> { Name = "S", Data = second };

        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    #endregion

    #region ChartDataSet extensions

    [Test]
    public void ChartDataSetExtensions_AsList_WrapsSeries()
    {
        var series = new ChartSeries<double> { Name = "S", Data = new[] { 1.0 } };

        var list = series.AsList();

        list.Should().ContainSingle().Which.Should().BeSameAs(series);
    }

    [Test]
    public void ChartDataSetExtensions_AsChartDataSet_WrapsValues()
    {
        var list = new[] { 1.0, 2.0 }.AsChartDataSet();

        list.Should().ContainSingle();
        list[0].Data.Values.Should().Equal(1.0, 2.0);
    }

    #endregion
}
