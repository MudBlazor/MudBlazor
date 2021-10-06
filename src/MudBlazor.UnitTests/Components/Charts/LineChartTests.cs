﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using MudBlazor.Charts;
using MudBlazor.UnitTests.Components;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Charts
{
    public class LineChartTests: BunitTest
    {
        private static Array GetInterpolationOptions()
        {
            return Enum.GetValues(typeof(InterpolationOption));
        }
        
        [SetUp]
        public void Init()
        {
 
        }

        [Test]
        public void LineChartEmptyData()
        {
            var comp = Context.RenderComponent<Bar>();
            comp.Markup.Should().Contain("mud-chart");
        }

        [Theory]
        [TestCaseSource("GetInterpolationOptions")]
        public void LineChartExampleData(InterpolationOption opt)
        {
            List<ChartSeries> chartSeries = new List<ChartSeries>()
            {
                new ChartSeries() { Name = "Series 1", Data = new double[] { 90, 79, 72, 69, 62, 62, 55, 65, 70 } },
                new ChartSeries() { Name = "Series 2", Data = new double[] { 10, 41, 35, 51, 49, 62, 69, 91, 148 } },
            };
            string[] xAxisLabels = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep" };
            
            var comp = Context.RenderComponent<MudChart>(parameters => parameters
                .Add(p => p.ChartType, ChartType.Line)
                .Add(p => p.Height, "350px")
                .Add(p => p.Width, "100%")
                .Add(p => p.ChartSeries, chartSeries)
                .Add(p => p.XAxisLabels, xAxisLabels)
                .Add(p => p.ChartOptions, new ChartOptions() {InterpolationOption = opt}));

            comp.Instance.ChartSeries.Should().NotBeEmpty();
            
            comp.Markup.Should().Contain("class=\"mud-charts-xaxis\"");
            comp.Markup.Should().Contain("class=\"mud-charts-yaxis\"");
            comp.Markup.Should().Contain("mud-chart-legend-item");
            
            if (chartSeries.Count <= 3)
            {
                comp.Markup.Should().
                    Contain("Series 1").And.Contain("Series 2");
            }

            if (chartSeries.FirstOrDefault(x => x.Name == "Series 1") is not null)
            {
                switch (opt)
                {
                    case InterpolationOption.NaturalSpline:
                        comp.Markup.Should().Contain(
                        "d=\"M 30 156.25 L 37.28395061728395 158.43411991439618 L 44.5679012345679 160.61086892488956 " +
                        "L 51.85185185185185 162.77287612757732 L 59.1358024691358 164.91277061855672 L 66.41975308641975 167.0231814939249 " +
                        "L 73.7037037037037 169.0967378497791 L 80.98765432098764 171.1260687822165 L 88.2716049382716 173.10380338733432 L 95.55555555555554 175.02257076122976 " +
                        "L 102.83950617283949 176.875 L 110.12345679012344 178.65359110364508 L 117.40740740740739 180.35032768777612 L 124.69135802469134 181.95706427190723 L 131.9753086419753 183.46565537555227 " +
                        "L 139.25925925925924 184.8679555182253 L 146.5432098765432 186.15581921944036 L 153.82716049382714 187.32110099871136 L 161.1111111111111 188.3556553755523 L 168.39506172839504 189.25133686947717 " +
                        "L 175.67901234567898 190 L 182.96296296296293 190.60151567102358 L 190.24691358024688 191.08782032400586 L 197.53086419753083 191.49886678479382 L 204.81481481481478 191.87460787923416 L 212.09876543209873 192.2549964331738 " +
                        "L 219.38271604938268 192.6799852724595 L 226.66666666666663 193.18952722293815 L 233.95061728395058 193.82357511045655 L 241.23456790123453 194.6220817608616 L 248.51851851851848 195.625 L 255.80246913580243 196.85534621226068 " +
                        "L 263.0864197530864 198.2683910162003 L 270.3703703703703 199.80246858891752 L 277.6543209876543 201.39591310751103 L 284.9382716049382 202.98705874907955 L 292.2222222222222 204.51423969072167 L 299.5061728395061 205.91579010953606 " +
                        "L 306.79012345679007 207.1300441826215 L 314.074074074074 208.0953360870766 L 321.35802469135797 208.75 L 328.6419753086419 209.05647447993374 L 335.92592592592587 209.0736156111929 L 343.2098765432098 208.88438385953606 " +
                        "L 350.49382716049377 208.57173969072167 L 357.7777777777777 208.2186435705081 L 365.06172839506166 207.9080559646539 L 372.3456790123456 207.72293733891752 L 379.62962962962956 207.74624815905744 L 386.9135802469135 208.0609488908321 " +
                        "L 394.19753086419746 208.75 L 401.4814814814814 209.8637558680044 L 408.76543209876536 211.32214653902795 L 416.0493827160493 213.01249597293813 L 423.33333333333326 214.82212812960236 L 430.6172839506172 216.63836696888808 " +
                        "L 437.90123456790116 218.34853645066275 L 445.1851851851851 219.83996053479382 L 452.46913580246905 220.99996318114876 L 459.753086419753 221.71586834959498 L 467.03703703703695 221.875 L 474.3209876543209 221.3997520480486 " +
                        "L 481.60493827160485 220.35279823269516 L 488.8888888888888 218.83188224871134 L 496.17283950617275 216.93474779086893 L 503.4567901234567 214.75913855393964 L 510.74074074074065 212.40279823269512 L 518.0246913580246 209.96347052190723 " +
                        "L 525.3086419753085 207.53889911634758 L 532.5925925925925 205.22682771078792 L 539.8765432098764 203.125 L 547.1604938271604 201.30973593980116 L 554.4444444444443 199.77166053019144 L 561.7283950617283 198.4799750322165 L 569.0123456790122 197.40388070692194 " +
                        "L 576.2962962962962 196.51257881535344 L 583.5802469135801 195.7752706185567 L 590.8641975308641 195.1611573775773 L 598.148148148148 194.63944035346097 L 605.432098765432 194.1793208072533 L 612.7160493827159 193.75\"");
                        break;
                    case InterpolationOption.Straight:
                        comp.Markup.Should()
                            .Contain("d=\"M 30 156.25 L 103.75 176.875 L 177.5 190 L 251.25 195.625 L 325 208.75 L 398.75 208.75 L 472.5 221.875 L 546.25 203.125 L 620 193.75\"");
                        break;
                    case InterpolationOption.EndSlope:
                        comp.Markup.Should().Contain("d=\"M 30 156.25 L 37.28395061728395 156.61129061579527 L 44.5679012345679 157.6262555228277 L 51.85185185185185 159.19153431056702 L 59.1358024691358 161.20376656848308 L 66.41975308641975 163.55959188604564 L 73.7037037037037 166.1556498527246 L 80.98765432098764 168.88858005798969 L 88.2716049382716 171.65502209131074 L 95.55555555555554 174.3516155421576 L 102.83950617283949 176.875 L 110.12345679012344 179.14197199466128 L 117.40740740740739 181.1499558173785 L 124.69135802469134 182.91653269974228 L 131.9753086419753 184.45928387334317 L 139.25925925925924 185.79579056977175 L 146.5432098765432 186.94363402061853 L 153.82716049382714 187.92039545747423 L 161.1111111111111 188.7436561119293 L 168.39506172839504 189.4309972155744 L 175.67901234567898 190 L 182.96296296296293 190.47082140555963 L 190.24691358024688 190.87392120765833 L 197.53086419753083 191.24233489046392 L 204.81481481481478 191.60909793814432 L 212.09876543209873 192.00724583486746 L 219.38271604938268 192.46981406480117 L 226.66666666666663 193.02983811211342 L 233.95061728395058 193.72035346097204 L 241.23456790123453 194.57439559554493 L 248.51851851851848 195.625 L 255.80246913580243 196.88974238310016 L 263.0864197530864 198.32435935198822 L 270.3703703703703 199.86912773840209 L 277.6543209876543 201.46432437407952 L 284.9382716049382 203.05022609075846 L 292.2222222222222 204.56710972017675 L 299.5061728395061 205.95525209407214 L 306.79012345679007 207.15493004418263 L 314.074074074074 208.10642040224596 L 321.35802469135797 208.75 L 328.6419753086419 209.04958406203977 L 335.92592592592587 209.0636413843888 L 343.2098765432098 208.87427915592784 L 350.49382716049377 208.56360456553753 L 357.7777777777777 208.21372480209868 L 365.06172839506166 207.90674705449192 L 372.3456790123456 207.72477851159792 L 379.62962962962956 207.74992636229751 L 386.9135802469135 208.06429779547128 L 394.19753086419746 208.75 L 401.4814814814814 209.8569213687408 L 408.76543209876536 211.30607511045656 L 416.0493827160493 212.98625563788661 L 423.33333333333326 214.78625736377026 L 430.6172839506172 216.59487470084684 L 437.90123456790116 218.30090206185568 L 445.1851851851851 219.79313385953608 L 452.46913580246905 220.96036450662737 L 459.753086419753 221.69138841586891 L 467.03703703703695 221.875 L 474.3209876543209 221.43398046299706 L 481.60493827160485 220.42705817378499 L 488.8888888888888 218.94694829252578 L 496.17283950617275 217.08636597938144 L 503.4567901234567 214.93802639451397 L 510.74074074074065 212.5946446980854 L 518.0246913580246 210.14893605025776 L 525.3086419753085 207.69361561119294 L 532.5925925925925 205.32139854105301 L 539.8765432098764 203.125 L 547.1604938271604 201.17965677927097 L 554.4444444444443 199.49069219440352 L 561.7283950617283 198.04595119201034 L 569.0123456790122 196.83327871870398 L 576.2962962962962 195.8405197210972 L 583.5802469135801 195.05551914580266 L 590.8641975308641 194.46612193943298 L 598.148148148148 194.06017304860092 L 605.432098765432 193.825517419919 L 612.7160493827159 193.75\"");
                    break;
                    case InterpolationOption.Periodic:
                        comp.Markup.Should().Contain("d=\"M 30 156.25 L 37.28395061728395 158.9097544642857 L 44.5679012345679 161.38964285714286 L 51.85185185185185 163.70734375 L 59.1358024691358 165.8805357142857 L 66.41975308641975 167.92689732142858 L 73.7037037037037 169.86410714285714 L 80.98765432098764 171.70984375 L 88.2716049382716 173.4817857142857 L 95.55555555555554 175.19761160714287 L 102.83950617283949 176.875 L 110.12345679012344 178.52620535714286 L 117.40740740740739 180.14178571428573 L 124.69135802469134 181.706875 L 131.9753086419753 183.20660714285714 L 139.25925925925924 184.62611607142856 L 146.5432098765432 185.95053571428573 L 153.82716049382714 187.16500000000002 L 161.1111111111111 188.25464285714287 L 168.39506172839504 189.20459821428574 L 175.67901234567898 190 L 182.96296296296293 190.63542410714285 L 190.24691358024688 191.1432142857143 L 197.53086419753083 191.56515625 L 204.81481481481478 191.9430357142857 L 212.09876543209873 192.31863839285717 L 219.38271604938268 192.73375000000001 L 226.66666666666663 193.23015625 L 233.95061728395058 193.84964285714284 L 241.23456790123453 194.6339955357143 L 248.51851851851848 195.625 L 255.80246913580243 196.84709821428572 L 263.0864197530864 198.25535714285712 L 270.3703703703703 199.7875 L 277.6543209876543 201.38125000000002 L 284.9382716049382 202.97433035714286 L 292.2222222222222 204.5044642857143 L 299.5061728395061 205.90937499999998 L 306.79012345679007 207.1267857142857 L 314.074074074074 208.09441964285716 L 321.35802469135797 208.75 L 328.6419753086419 209.05555803571428 L 335.92592592592587 209.07035714285715 L 343.2098765432098 208.87796875 L 350.49382716049377 208.5619642857143 L 357.7777777777777 208.20591517857142 L 365.06172839506166 207.89339285714286 L 372.3456790123456 207.70796875 L 379.62962962962956 207.73321428571427 L 386.9135802469135 208.05270089285713 L 394.19753086419746 208.75 L 401.4814814814814 209.87566964285713 L 408.76543209876536 211.34821428571428 L 416.0493827160493 213.053125 L 423.33333333333326 214.87589285714287 L 430.6172839506172 216.70200892857144 L 437.90123456790116 218.41696428571427 L 445.1851851851851 219.90625 L 452.46913580246905 221.05535714285713 L 459.753086419753 221.7497767857143 L 467.03703703703695 221.875 L 474.3209876543209 221.35301339285715 L 481.60493827160485 220.25178571428572 L 488.8888888888888 218.67578125000003 L 496.17283950617275 216.72946428571433 L 503.4567901234567 214.51729910714283 L 510.74074074074065 212.14375 L 518.0246913580246 209.71328125000002 L 525.3086419753085 207.33035714285714 L 532.5925925925925 205.09944196428572 L 539.8765432098764 203.125 L 547.1604938271604 201.48477678571427 L 554.4444444444443 200.14964285714285 L 561.7283950617283 199.06375000000003 L 569.0123456790122 198.17125 L 576.2962962962962 197.41629464285714 L 583.5802469135801 196.7430357142857 L 590.8641975308641 196.095625 L 598.148148148148 195.4182142857143 L 605.432098765432 194.65495535714285 L 612.7160493827159 193.75\"");
                        break;
                }
            }

            if (comp.Instance.ChartOptions.InterpolationOption == InterpolationOption.Straight && chartSeries.FirstOrDefault(x => x.Name == "Series 2") is not null)
            {
                comp.Markup.Should()
                    .Contain("d=\"M 30 306.25 L 103.75 248.125 L 177.5 259.375 L 251.25 229.375 L 325 233.125 L 398.75 208.75 L 472.5 195.625 L 546.25 154.375 L 620 47.5\"");
            }
        }
    }
}
