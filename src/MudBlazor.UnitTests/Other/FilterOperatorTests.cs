// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Other
{
    [TestFixture]
    public class FilterOperatorTests
    {
        /// <summary>
        /// Custom filters can access the type-based operator lookup without internal access (#10607).
        /// </summary>
        [Test]
        public void GetOperatorByDataType_IsPublic()
        {
            var method = typeof(FilterOperator).GetMethod(nameof(FilterOperator.GetOperatorByDataType),
                BindingFlags.Public | BindingFlags.Static, new[] { typeof(Type) });

            method.Should().NotBeNull();
        }

        /// <summary>
        /// Supported data types expose the operators needed by custom filters (#10607).
        /// </summary>
        [TestCase(typeof(string), FilterOperator.String.Contains)]
        [TestCase(typeof(int), FilterOperator.Number.GreaterThan)]
        [TestCase(typeof(Adornment), FilterOperator.Enum.IsAnyOf)]
        [TestCase(typeof(bool), FilterOperator.Boolean.Is)]
        [TestCase(typeof(DateOnly), FilterOperator.DateOnly.Before)]
        [TestCase(typeof(DateTime), FilterOperator.DateTime.After)]
        [TestCase(typeof(Guid), FilterOperator.Guid.Equal)]
        public void GetOperatorByDataType_SupportedType(Type type, string expectedOperator)
        {
            FilterOperator.GetOperatorByDataType(type).Should().Contain(expectedOperator);
        }

        /// <summary>
        /// Nullable value types expose the same operators as their underlying types (#10607).
        /// </summary>
        [TestCase(typeof(int?))]
        [TestCase(typeof(Adornment?))]
        [TestCase(typeof(bool?))]
        [TestCase(typeof(DateOnly?))]
        [TestCase(typeof(DateTime?))]
        [TestCase(typeof(Guid?))]
        public void GetOperatorByDataType_NullableType(Type type)
        {
            var operators = FilterOperator.GetOperatorByDataType(type);
            var underlyingOperators = FilterOperator.GetOperatorByDataType(Nullable.GetUnderlyingType(type));

            operators.Should().NotBeEmpty().And.Equal(underlyingOperators);
        }

        /// <summary>
        /// Missing or unsupported data types have no built-in filter operators (#10607).
        /// </summary>
        [TestCase(null)]
        [TestCase(typeof(object))]
        [TestCase(typeof(TimeOnly))]
        public void GetOperatorByDataType_UnsupportedType(Type type)
        {
            FilterOperator.GetOperatorByDataType(type).Should().BeEmpty();
        }
    }
}
