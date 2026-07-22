// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
#nullable enable
    [TestFixture]
    public class AggregateDefinitionTests
    {
        public record AccountingModel(long Id, int Position, decimal Salary);
        public record AccountingNullableModel(long? Id, int? Position, decimal? Salary);

        private readonly List<AccountingModel> _accountingModels = new()
        {
            new AccountingModel(47219479, 1, 50_000.00M),
            new AccountingModel(45376824, 3, 75_000.00M),
            new AccountingModel(72746864, 8, 102_000.00M),
            new AccountingModel(9434843, 20, 132_000.00M),
            new AccountingModel(44279196, 23, 245_100.00M),
            new AccountingModel(18223533, 28, 500_000.00M),
            new AccountingModel(67898330, 30, 1_000_000.00M),
        };

        private readonly List<AccountingNullableModel> _accountingNullableModels = new()
        {
            new AccountingNullableModel(null, null, null),
            new AccountingNullableModel(47219479, 1, 50_000.00M),
            new AccountingNullableModel(45376824, 3, 75_000.00M),
            new AccountingNullableModel(72746864, 8, 102_000.00M),
            new AccountingNullableModel(9434843, 20, 132_000.00M),
            new AccountingNullableModel(44279196, 23, 245_100.00M),
            new AccountingNullableModel(18223533, 28, 500_000.00M),
            new AccountingNullableModel(67898330, 30, 1_00_000.00M),
        };

        [Test]
        public void AggregateDefinition_Cache_Returns_SameObject_Test()
        {
            Expression<Func<AccountingModel, decimal?>> propertyExpression1 = model => model.Salary;
            Expression<Func<AccountingModel, decimal?>> propertyExpression2 = model => model.Salary;
            var cache = new AggregateDefinition<AccountingModel>.AggregateDefinitionExpressionCache();

            var expression1 = cache.CachedCompile(propertyExpression1);
            var expression2 = cache.CachedCompile(propertyExpression2);

            propertyExpression1.Equals(propertyExpression2).Should().BeFalse();
            expression1.Equals(expression2).Should().BeTrue();
        }

        [Test]
        public void AggregateDefinition_Cache_Returns_DifferentObject_Test()
        {
            Expression<Func<AccountingModel, decimal?>> propertyExpression1 = model => model.Salary;
            Expression<Func<AccountingModel, decimal?>> propertyExpression2 = model => model.Id;
            var cache = new AggregateDefinition<AccountingModel>.AggregateDefinitionExpressionCache();

            var expression1 = cache.CachedCompile(propertyExpression1);
            var expression2 = cache.CachedCompile(propertyExpression2);

            propertyExpression1.Equals(propertyExpression2).Should().BeFalse();
            expression1.Equals(expression2).Should().BeFalse();
        }

        [Test]
        public void AggregateDefinition_Decimal_NullList_Test()
        {
            Expression<Func<AccountingModel, decimal>> propertyExpression = model => model.Salary;

            var aggregateDefinitionAverage = new AggregateDefinition<AccountingModel>();
            var value = aggregateDefinitionAverage.GetValue(propertyExpression, null);

            value.Should().Be("0");
        }

        [Test]
        public void AggregateDefinition_Decimal_EmptyList_Test()
        {
            Expression<Func<AccountingModel, decimal>> propertyExpression = model => model.Salary;

            var aggregateDefinitionAverage = new AggregateDefinition<AccountingModel>();
            var value = aggregateDefinitionAverage.GetValue(propertyExpression, Array.Empty<AccountingModel>());

            value.Should().Be("0");
        }

        [Test]
        public void AggregateDefinition_Decimal_NullExpression_Test()
        {
            var aggregateDefinitionAverage = new AggregateDefinition<AccountingModel>();
            var value = aggregateDefinitionAverage.GetValue(null, _accountingModels);

            value.Should().Be("0");
        }

        [Test]
        public void AggregateDefinition_Decimal_Test()
        {
            Expression<Func<AccountingModel, decimal>> propertyExpression = model => model.Salary;

            var aggregateDefinitionAverage = AggregateDefinition<AccountingModel>.SimpleAvg();
            var aggregateDefinitionMin = AggregateDefinition<AccountingModel>.SimpleMin();
            var aggregateDefinitionMax = AggregateDefinition<AccountingModel>.SimpleMax();
            var aggregateDefinitionCount = AggregateDefinition<AccountingModel>.SimpleCount();
            var aggregateDefinitionSum = AggregateDefinition<AccountingModel>.SimpleSum();

            var valueAverage = aggregateDefinitionAverage.GetValue(propertyExpression, _accountingModels);
            var valueMin = aggregateDefinitionMin.GetValue(propertyExpression, _accountingModels);
            var valueMax = aggregateDefinitionMax.GetValue(propertyExpression, _accountingModels);
            var valueCount = aggregateDefinitionCount.GetValue(propertyExpression, _accountingModels);
            var valueSum = aggregateDefinitionSum.GetValue(propertyExpression, _accountingModels);

            var expectedAverage = _accountingModels.Select(x => x.Salary).Average();
            var expectedMin = _accountingModels.Select(x => x.Salary).Min();
            var expectedMax = _accountingModels.Select(x => x.Salary).Max();
            var expectedCount = _accountingModels.Select(x => x.Salary).Count();
            var expectedSum = _accountingModels.Select(x => x.Salary).Sum();

            valueAverage.Should().Be($"Average {expectedAverage}");
            valueMin.Should().Be($"Min {expectedMin}");
            valueMax.Should().Be($"Max {expectedMax}");
            valueCount.Should().Be($"Total {expectedCount}");
            valueSum.Should().Be($"Sum {expectedSum}");
        }

        [Test]
        public void AggregateDefinition_NullableDecimal_Test()
        {
            Expression<Func<AccountingNullableModel, decimal?>> propertyExpression = model => model.Salary;

            var aggregateDefinitionAverage = AggregateDefinition<AccountingNullableModel>.SimpleAvg();
            var aggregateDefinitionMin = AggregateDefinition<AccountingNullableModel>.SimpleMin();
            var aggregateDefinitionMax = AggregateDefinition<AccountingNullableModel>.SimpleMax();
            var aggregateDefinitionCount = AggregateDefinition<AccountingNullableModel>.SimpleCount();
            var aggregateDefinitionSum = AggregateDefinition<AccountingNullableModel>.SimpleSum();

            var valueAverage = aggregateDefinitionAverage.GetValue(propertyExpression, _accountingNullableModels);
            var valueMin = aggregateDefinitionMin.GetValue(propertyExpression, _accountingNullableModels);
            var valueMax = aggregateDefinitionMax.GetValue(propertyExpression, _accountingNullableModels);
            var valueCount = aggregateDefinitionCount.GetValue(propertyExpression, _accountingNullableModels);
            var valueSum = aggregateDefinitionSum.GetValue(propertyExpression, _accountingNullableModels);

            var expectedAverage = _accountingNullableModels.Select(x => x.Salary).Average();
            var expectedMin = _accountingNullableModels.Select(x => x.Salary).Min();
            var expectedMax = _accountingNullableModels.Select(x => x.Salary).Max();
            var expectedCount = _accountingNullableModels.Select(x => x.Salary).Count();
            var expectedSum = _accountingNullableModels.Select(x => x.Salary).Sum();

            valueAverage.Should().Be($"Average {expectedAverage}");
            valueMin.Should().Be($"Min {expectedMin}");
            valueMax.Should().Be($"Max {expectedMax}");
            valueCount.Should().Be($"Total {expectedCount}");
            valueSum.Should().Be($"Sum {expectedSum}");
        }

        [Test]
        public void AggregateDefinition_Integer_Test()
        {
            Expression<Func<AccountingModel, int>> propertyExpression = model => model.Position;

            var aggregateDefinitionAverage = AggregateDefinition<AccountingModel>.SimpleAvg();
            var aggregateDefinitionMin = AggregateDefinition<AccountingModel>.SimpleMin();
            var aggregateDefinitionMax = AggregateDefinition<AccountingModel>.SimpleMax();
            var aggregateDefinitionCount = AggregateDefinition<AccountingModel>.SimpleCount();
            var aggregateDefinitionSum = AggregateDefinition<AccountingModel>.SimpleSum();

            var valueAverage = aggregateDefinitionAverage.GetValue(propertyExpression, _accountingModels);
            var valueMin = aggregateDefinitionMin.GetValue(propertyExpression, _accountingModels);
            var valueMax = aggregateDefinitionMax.GetValue(propertyExpression, _accountingModels);
            var valueCount = aggregateDefinitionCount.GetValue(propertyExpression, _accountingModels);
            var valueSum = aggregateDefinitionSum.GetValue(propertyExpression, _accountingModels);

            //Need to cast to get decimal precision, inside AggregateDefinition.GetValue casts to decimal 
            var expectedAverage = _accountingModels.Select(x => (decimal)x.Position).Average();
            var expectedMin = _accountingModels.Select(x => x.Position).Min();
            var expectedMax = _accountingModels.Select(x => x.Position).Max();
            var expectedCount = _accountingModels.Select(x => x.Position).Count();
            var expectedSum = _accountingModels.Select(x => x.Position).Sum();

            valueAverage.Should().Be($"Average {expectedAverage}");
            valueMin.Should().Be($"Min {expectedMin}");
            valueMax.Should().Be($"Max {expectedMax}");
            valueCount.Should().Be($"Total {expectedCount}");
            valueSum.Should().Be($"Sum {expectedSum}");
        }

        [Test]
        public void AggregateDefinition_NullableInteger_Test()
        {
            Expression<Func<AccountingNullableModel, int?>> propertyExpression = model => model.Position;

            var aggregateDefinitionAverage = AggregateDefinition<AccountingNullableModel>.SimpleAvg();
            var aggregateDefinitionMin = AggregateDefinition<AccountingNullableModel>.SimpleMin();
            var aggregateDefinitionMax = AggregateDefinition<AccountingNullableModel>.SimpleMax();
            var aggregateDefinitionCount = AggregateDefinition<AccountingNullableModel>.SimpleCount();
            var aggregateDefinitionSum = AggregateDefinition<AccountingNullableModel>.SimpleSum();

            var valueAverage = aggregateDefinitionAverage.GetValue(propertyExpression, _accountingNullableModels);
            var valueMin = aggregateDefinitionMin.GetValue(propertyExpression, _accountingNullableModels);
            var valueMax = aggregateDefinitionMax.GetValue(propertyExpression, _accountingNullableModels);
            var valueCount = aggregateDefinitionCount.GetValue(propertyExpression, _accountingNullableModels);
            var valueSum = aggregateDefinitionSum.GetValue(propertyExpression, _accountingNullableModels);

            //Need to cast to get decimal precision, inside AggregateDefinition.GetValue casts to decimal 
            var expectedAverage = _accountingNullableModels.Select(x => (decimal?)x.Position).Average();
            var expectedMin = _accountingNullableModels.Select(x => x.Position).Min();
            var expectedMax = _accountingNullableModels.Select(x => x.Position).Max();
            var expectedCount = _accountingNullableModels.Select(x => x.Position).Count();
            var expectedSum = _accountingNullableModels.Select(x => x.Position).Sum();

            valueAverage.Should().Be($"Average {expectedAverage}");
            valueMin.Should().Be($"Min {expectedMin}");
            valueMax.Should().Be($"Max {expectedMax}");
            valueCount.Should().Be($"Total {expectedCount}");
            valueSum.Should().Be($"Sum {expectedSum}");
        }

        [Test]
        public void AggregateDefinition_Long_Test()
        {
            Expression<Func<AccountingModel, long>> propertyExpression = model => model.Id;

            var aggregateDefinitionAverage = AggregateDefinition<AccountingModel>.SimpleAvg();
            var aggregateDefinitionMin = AggregateDefinition<AccountingModel>.SimpleMin();
            var aggregateDefinitionMax = AggregateDefinition<AccountingModel>.SimpleMax();
            var aggregateDefinitionCount = AggregateDefinition<AccountingModel>.SimpleCount();
            var aggregateDefinitionSum = AggregateDefinition<AccountingModel>.SimpleSum();

            var valueAverage = aggregateDefinitionAverage.GetValue(propertyExpression, _accountingModels);
            var valueMin = aggregateDefinitionMin.GetValue(propertyExpression, _accountingModels);
            var valueMax = aggregateDefinitionMax.GetValue(propertyExpression, _accountingModels);
            var valueCount = aggregateDefinitionCount.GetValue(propertyExpression, _accountingModels);
            var valueSum = aggregateDefinitionSum.GetValue(propertyExpression, _accountingModels);

            //Need to cast to get decimal precision, inside AggregateDefinition.GetValue casts to decimal 
            var expectedAverage = _accountingModels.Select(x => (decimal)x.Id).Average();
            var expectedMin = _accountingModels.Select(x => x.Id).Min();
            var expectedMax = _accountingModels.Select(x => x.Id).Max();
            var expectedCount = _accountingModels.Select(x => x.Id).Count();
            var expectedSum = _accountingModels.Select(x => x.Id).Sum();

            valueAverage.Should().Be($"Average {expectedAverage}");
            valueMin.Should().Be($"Min {expectedMin}");
            valueMax.Should().Be($"Max {expectedMax}");
            valueCount.Should().Be($"Total {expectedCount}");
            valueSum.Should().Be($"Sum {expectedSum}");
        }

        [Test]
        public void AggregateDefinition_NullableLong_Test()
        {
            Expression<Func<AccountingNullableModel, long?>> propertyExpression = model => model.Id;

            var aggregateDefinitionAverage = AggregateDefinition<AccountingNullableModel>.SimpleAvg();
            var aggregateDefinitionMin = AggregateDefinition<AccountingNullableModel>.SimpleMin();
            var aggregateDefinitionMax = AggregateDefinition<AccountingNullableModel>.SimpleMax();
            var aggregateDefinitionCount = AggregateDefinition<AccountingNullableModel>.SimpleCount();
            var aggregateDefinitionSum = AggregateDefinition<AccountingNullableModel>.SimpleSum();

            var valueAverage = aggregateDefinitionAverage.GetValue(propertyExpression, _accountingNullableModels);
            var valueMin = aggregateDefinitionMin.GetValue(propertyExpression, _accountingNullableModels);
            var valueMax = aggregateDefinitionMax.GetValue(propertyExpression, _accountingNullableModels);
            var valueCount = aggregateDefinitionCount.GetValue(propertyExpression, _accountingNullableModels);
            var valueSum = aggregateDefinitionSum.GetValue(propertyExpression, _accountingNullableModels);

            //Need to cast to get decimal precision, inside AggregateDefinition.GetValue casts to decimal 
            var expectedAverage = _accountingNullableModels.Select(x => (decimal?)x.Id).Average();
            var expectedMin = _accountingNullableModels.Select(x => x.Id).Min();
            var expectedMax = _accountingNullableModels.Select(x => x.Id).Max();
            var expectedCount = _accountingNullableModels.Select(x => x.Id).Count();
            var expectedSum = _accountingNullableModels.Select(x => x.Id).Sum();

            valueAverage.Should().Be($"Average {expectedAverage}");
            valueMin.Should().Be($"Min {expectedMin}");
            valueMax.Should().Be($"Max {expectedMax}");
            valueCount.Should().Be($"Total {expectedCount}");
            valueSum.Should().Be($"Sum {expectedSum}");
        }

        [Test]
        public void AggregateDefinition_NumberFormat_Test()
        {
            Expression<Func<AccountingNullableModel, decimal?>> propertyExpression = model => model.Salary;

            var aggregateDefinitionAverage = AggregateDefinition<AccountingNullableModel>.SimpleAvg("C", CultureInfo.CurrentUICulture);
            var aggregateDefinitionMin = AggregateDefinition<AccountingNullableModel>.SimpleMin("F", new CultureInfo("en-US"));
            var aggregateDefinitionMax = AggregateDefinition<AccountingNullableModel>.SimpleMax("P", new CultureInfo("zh-CN"));
            var aggregateDefinitionCount = AggregateDefinition<AccountingNullableModel>.SimpleCount("N", CultureInfo.InvariantCulture);
            var aggregateDefinitionSum = AggregateDefinition<AccountingNullableModel>.SimpleSum("000000:00.00", new CultureInfo("da-DK"));

            var valueAverage = aggregateDefinitionAverage.GetValue(propertyExpression, _accountingNullableModels);
            var valueMin = aggregateDefinitionMin.GetValue(propertyExpression, _accountingNullableModels);
            var valueMax = aggregateDefinitionMax.GetValue(propertyExpression, _accountingNullableModels);
            var valueCount = aggregateDefinitionCount.GetValue(propertyExpression, _accountingNullableModels);
            var valueSum = aggregateDefinitionSum.GetValue(propertyExpression, _accountingNullableModels);

            var expectedAverage = _accountingNullableModels.Select(x => x.Salary).Average();
            var expectedMin = _accountingNullableModels.Select(x => x.Salary).Min();
            var expectedMax = _accountingNullableModels.Select(x => x.Salary).Max();
            var expectedCount = _accountingNullableModels.Select(x => x.Salary).Count();
            var expectedSum = _accountingNullableModels.Select(x => x.Salary).Sum();

            valueAverage.Should().Be($"Average {expectedAverage!.Value.ToString("C", CultureInfo.CurrentUICulture)}");
            valueMin.Should().Be($"Min {expectedMin!.Value.ToString("F", new CultureInfo("en-US"))}");
            valueMax.Should().Be($"Max {expectedMax!.Value.ToString("P", new CultureInfo("zh-CN"))}");
            valueCount.Should().Be($"Total {expectedCount.ToString("N", CultureInfo.InvariantCulture)}");
            valueSum.Should().Be($"Sum {expectedSum!.Value.ToString("000000:00.00", new CultureInfo("da-DK"))}");
        }

        [Test]
        public void AggregateDefinition_Custom_Test()
        {
            var aggregateDefinitionAverage = new AggregateDefinition<AccountingModel>
            {
                Type = AggregateType.Custom,
                CustomAggregate = model =>
                {
                    var accountingModels = model as AccountingModel[] ?? model.ToArray();
                    var highestSalary = accountingModels.Max(z => z.Salary);
                    var countOver100Grand = accountingModels.Count(z => z.Salary > 100_000);
                    return $"Highest: {highestSalary.ToString(NumberFormatInfo.InvariantInfo)} | {countOver100Grand.ToString(NumberFormatInfo.InvariantInfo)} Over {100000.ToString(NumberFormatInfo.InvariantInfo)}";
                }
            };

            var value = aggregateDefinitionAverage.GetValue(null, _accountingModels);

            value.Should().Be("Highest: 1000000.00 | 5 Over 100000");
        }

        [Test]
        public void AggregateDefinition_InvalidType_Test()
        {
            Expression<Func<AccountingModel, decimal>> propertyExpression = model => model.Salary;

            var aggregateDefinitionAverage = new AggregateDefinition<AccountingModel>
            {
                Type = (AggregateType)(-1)
            };

            var value = aggregateDefinitionAverage.GetValue(propertyExpression, _accountingModels);

            value.Should().Be("0");
        }

        [Test(Description = "This is to ensure that we do not return same cached compiled expression and get same results after passing different expression")]
        public void AggregateDefinition_ReuseDefinition_With_Different_Expressions_Test()
        {
            Expression<Func<AccountingModel, long>> propertyExpressionId = model => model.Id;
            Expression<Func<AccountingModel, decimal>> propertyExpressionSalary = model => model.Salary;

            var aggregateDefinitionAverage = AggregateDefinition<AccountingModel>.SimpleMin();

            var expectedIdMin = _accountingModels.Select(x => x.Id).Min();
            var expectedSalaryMin = _accountingModels.Select(x => x.Salary).Min();

            var valueIdMin = aggregateDefinitionAverage.GetValue(propertyExpressionId, _accountingModels);
            var valueSalaryMin = aggregateDefinitionAverage.GetValue(propertyExpressionSalary, _accountingModels);

            valueIdMin.Should().Be($"Min {expectedIdMin}");
            valueSalaryMin.Should().Be($"Min {expectedSalaryMin}");
        }
    }
}
