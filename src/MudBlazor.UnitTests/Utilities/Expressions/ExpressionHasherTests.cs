// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq.Expressions;
using System;
using FluentAssertions;
using MudBlazor.Utilities.Expressions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities.Expressions
{
#nullable enable
    [TestFixture]
    public class ExpressionHasherTests
    {
        // ReSharper disable once ClassNeverInstantiated.Local
        private class ExpressionTestClass
        {
            public string? Name => null;

            public string? Convert(int a, int[] arr) => null;
        }

        [Test]
        public void ExpressionHasherTests_Get_Same_HashCode_Test()
        {
            Expression<Func<ExpressionTestClass, string?>> exp1 = x => x.Name + "123" + x.Convert(2, new[] { 1, 2, 3 });
            Expression<Func<ExpressionTestClass, string?>> exp2 = x => x.Name + "123" + x.Convert(2, new[] { 1, 2, 3 });

            var h1 = ExpressionHasher.GetHashCode(exp1);
            var h2 = ExpressionHasher.GetHashCode(exp2);

            h1.Equals(h2).Should().BeTrue();
        }

        [Test]
        public void ExpressionHasherTests_Get_Null_HashCode_Test()
        {
            Expression<Func<ExpressionTestClass, string?>>? exp1 = null;
            Expression<Func<ExpressionTestClass, string?>>? exp2 = null;

            var h1 = ExpressionHasher.GetHashCode(exp1);
            var h2 = ExpressionHasher.GetHashCode(exp2);

            h1.Equals(h2).Should().BeTrue();
        }

        [Test]
        public void ExpressionHasherTests_Get_NotSame_HashCode_Test()
        {
            Expression<Func<ExpressionTestClass, string?>> exp1 = x => x.Name + x.Convert(2, Array.Empty<int>());
            Expression<Func<ExpressionTestClass, string?>> exp2 = x => x.Name + "123" + x.Convert(2, new[] { 1, 2, 3 });

            var h1 = ExpressionHasher.GetHashCode(exp1);
            var h2 = ExpressionHasher.GetHashCode(exp2);

            h1.Equals(h2).Should().BeFalse();
        }
    }
}
