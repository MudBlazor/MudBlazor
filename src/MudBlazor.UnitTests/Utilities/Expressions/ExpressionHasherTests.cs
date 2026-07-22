// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentAssertions;
using MudBlazor.Utilities.Expressions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities.Expressions
{
#nullable enable
    [TestFixture]
    public class ExpressionHasherTests
    {
        private class ExpressionTestClass
        {
            // ReSharper disable once ClassNeverInstantiated.Local
            public class SubClass
            {
                // ReSharper disable once PropertyCanBeMadeInitOnly.Local
                // ReSharper disable once UnusedAutoPropertyAccessor.Local
                public string SubMember1 { get; set; } = string.Empty;

                public int this[int index] => index;
            }

            // ReSharper disable MemberCanBeMadeStatic.Local
            // ReSharper disable once PropertyCanBeMadeInitOnly.Local
            public string? FirstName { get; set; }

            public string LastName => string.Empty;

            // ReSharper disable once PropertyCanBeMadeInitOnly.Local
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public SubClass Nested1 { get; } = new();

            public List<ExpressionTestClass> Children { get; } = new();

            public ExpressionTestClass(string? name) => FirstName = name;

            public ExpressionTestClass() : this(null) { }

            // ReSharper disable UnusedParameter.Local
            public string? Convert(int[] arr) => null;
            // ReSharper restore UnusedParameter.Local
            // ReSharper restore MemberCanBeMadeStatic.Local

            // ReSharper disable MemberCanBeMadeStatic.Local
            public void Method1() { }

            public void Method2() { }

            // ReSharper disable once UnusedParameter.Local
            public void MethodParam(int a) { }
            // ReSharper restore MemberCanBeMadeStatic.Local
        }

        [Test(Description = "VisitMethodCall")]
        public void ExpressionHasherTests_Get_Same_HashCode_Test1()
        {
            Expression<Func<ExpressionTestClass, string?>> exp1 = x => x.FirstName;
            Expression<Func<ExpressionTestClass, string?>> exp2 = x => x.FirstName;

            var h1 = ExpressionHasher.GetHashCode(exp1);
            var h2 = ExpressionHasher.GetHashCode(exp2);

            h1.Equals(h2).Should().BeTrue();
        }

        [Test(Description = "VisitMethodCall")]
        public void ExpressionHasherTests_Get_Same_HashCode_Test2()
        {
            Expression<Action<ExpressionTestClass>> exp1 = x => x.Method1();
            Expression<Action<ExpressionTestClass>> exp2 = x => x.Method1();

            var h1 = ExpressionHasher.GetHashCode(exp1);
            var h2 = ExpressionHasher.GetHashCode(exp2);

            h1.Equals(h2).Should().BeTrue();
        }

        [Test(Description = "VisitParameter")]
        public void ExpressionHasherTests_Get_Same_HashCode_Test3()
        {
            Expression<Action<ExpressionTestClass>> exp1 = x => x.MethodParam(1);
            Expression<Action<ExpressionTestClass>> exp2 = x => x.MethodParam(1);

            var h1 = ExpressionHasher.GetHashCode(exp1);
            var h2 = ExpressionHasher.GetHashCode(exp2);

            h1.Equals(h2).Should().BeTrue();
        }

        [Test(Description = "VisitNewArray")]
        public void ExpressionHasherTests_Get_Same_HashCode_Test4()
        {
            Expression<Func<ExpressionTestClass, string?>> exp1 = x => x.Convert(new[] { 1, 2, 3 });
            Expression<Func<ExpressionTestClass, string?>> exp2 = x => x.Convert(new[] { 1, 2, 3 });

            var h1 = ExpressionHasher.GetHashCode(exp1);
            var h2 = ExpressionHasher.GetHashCode(exp2);

            h1.Equals(h2).Should().BeTrue();
        }

        [Test(Description = "VisitConstant")]
        public void ExpressionHasherTests_Get_Same_HashCode_Test5()
        {
            const string ConstEmptyString = "";
            Expression<Func<ExpressionTestClass, string?>> exp1 = x => ConstEmptyString;
            Expression<Func<ExpressionTestClass, string?>> exp2 = x => ConstEmptyString;

            var h1 = ExpressionHasher.GetHashCode(exp1);
            var h2 = ExpressionHasher.GetHashCode(exp2);

            h1.Equals(h2).Should().BeTrue();
        }

        [Test(Description = "Checks VisitConstant -> UpdateHash(node.Value) where values is null")]
        public void ExpressionHasherTests_Get_Same_HashCode_Test6()
        {
            Expression<Func<ExpressionTestClass, string?>> exp1 = x => null;
            Expression<Func<ExpressionTestClass, string?>> exp2 = x => null;

            var h1 = ExpressionHasher.GetHashCode(exp1);
            var h2 = ExpressionHasher.GetHashCode(exp2);

            h1.Equals(h2).Should().BeTrue();
        }

        [Test(Description = "VisitNew")]
        public void ExpressionHasherTests_Get_Same_HashCode_Test7()
        {
            Expression<Func<ExpressionTestClass>> exp1 = () => new ExpressionTestClass("Name");
            Expression<Func<ExpressionTestClass>> exp2 = () => new ExpressionTestClass("Name");

            var h1 = ExpressionHasher.GetHashCode(exp1);
            var h2 = ExpressionHasher.GetHashCode(exp2);

            h1.Equals(h2).Should().BeTrue();
        }

        [Test]
        public void ExpressionHasherTests_Get_Same_HashCode_Test8()
        {
            Expression<Func<ExpressionTestClass, bool>> exp1 = x => string.Equals(x.FirstName, string.Empty, StringComparison.Ordinal);
            Expression<Func<ExpressionTestClass, bool>> exp2 = x => string.Equals(x.FirstName, string.Empty, StringComparison.Ordinal);

            var h1 = ExpressionHasher.GetHashCode(exp1);
            var h2 = ExpressionHasher.GetHashCode(exp2);

            h1.Equals(h2).Should().BeTrue();
        }

        [Test]
        public void ExpressionHasherTests_Get_Same_HashCode_Test9()
        {
            Expression<Func<bool?>> exp1 = () => 4 < 5;
            Expression<Func<bool?>> exp2 = () => 4 < 5;

            var h1 = ExpressionHasher.GetHashCode(exp1);
            var h2 = ExpressionHasher.GetHashCode(exp2);

            h1.Equals(h2).Should().BeTrue();
        }

        [Test(Description = "VisitMemberAssignment")]
        public void ExpressionHasherTests_Get_Same_HashCode_Test10()
        {
            Expression<Func<ExpressionTestClass>> exp1 = () => new ExpressionTestClass { FirstName = "assignment" };
            Expression<Func<ExpressionTestClass>> exp2 = () => new ExpressionTestClass { FirstName = "assignment" };

            var h1 = ExpressionHasher.GetHashCode(exp1);
            var h2 = ExpressionHasher.GetHashCode(exp2);

            h1.Equals(h2).Should().BeTrue();
        }

        [Test(Description = "VisitTypeBinary")]
        public void ExpressionHasherTests_Get_Same_HashCode_Test11()
        {
            Expression<Func<ExpressionTestClass, bool>> exp1 = x => x.Children[0] is ICloneable;
            Expression<Func<ExpressionTestClass, bool>> exp2 = x => x.Children[0] is ICloneable;

            var h1 = ExpressionHasher.GetHashCode(exp1);
            var h2 = ExpressionHasher.GetHashCode(exp2);

            h1.Equals(h2).Should().BeTrue();
        }

        [Test(Description = "VisitMemberMemberBinding")]
        public void ExpressionHasherTests_Get_Same_HashCode_Test12()
        {
            Expression<Func<ExpressionTestClass>> exp1 = () => new ExpressionTestClass { Nested1 = { SubMember1 = "MemberMemberBinding" } };
            Expression<Func<ExpressionTestClass>> exp2 = () => new ExpressionTestClass { Nested1 = { SubMember1 = "MemberMemberBinding" } };

            var h1 = ExpressionHasher.GetHashCode(exp1);
            var h2 = ExpressionHasher.GetHashCode(exp2);

            h1.Equals(h2).Should().BeTrue();
        }

        [Test(Description = "VisitMemberListBinding")]
        public void ExpressionHasherTests_Get_Same_HashCode_Test13()
        {
            Expression<Func<ExpressionTestClass>> exp1 = () => new ExpressionTestClass { Children = { new ExpressionTestClass(), new ExpressionTestClass() } };
            Expression<Func<ExpressionTestClass>> exp2 = () => new ExpressionTestClass { Children = { new ExpressionTestClass(), new ExpressionTestClass() } };

            var h1 = ExpressionHasher.GetHashCode(exp1);
            var h2 = ExpressionHasher.GetHashCode(exp2);

            h1.Equals(h2).Should().BeTrue();
        }

        [Test(Description = "VisitIndex")]
        public void ExpressionHasherTests_Get_Same_HashCode_Test14()
        {
            Expression<Func<ExpressionTestClass, int>> exp1 = x => x.Nested1[0];
            Expression<Func<ExpressionTestClass, int>> exp2 = x => x.Nested1[0];

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

        [Test(Description = "VisitMethodCall")]
        public void ExpressionHasherTests_Get_NotSame_HashCode_Test1()
        {
            Expression<Func<ExpressionTestClass, string?>> exp1 = x => x.FirstName;
            Expression<Func<ExpressionTestClass, string?>> exp2 = x => x.LastName;

            var h1 = ExpressionHasher.GetHashCode(exp1);
            var h2 = ExpressionHasher.GetHashCode(exp2);

            h1.Equals(h2).Should().BeFalse();
        }

        [Test(Description = "VisitMethodCall")]
        public void ExpressionHasherTests_Get_NotSame_HashCode_Test2()
        {
            Expression<Action<ExpressionTestClass>> exp1 = x => x.Method1();
            Expression<Action<ExpressionTestClass>> exp2 = x => x.Method2();

            var h1 = ExpressionHasher.GetHashCode(exp1);
            var h2 = ExpressionHasher.GetHashCode(exp2);

            h1.Equals(h2).Should().BeFalse();
        }

        [Test(Description = "VisitParameter")]
        public void ExpressionHasherTests_Get_NotSame_HashCode_Test3()
        {
            Expression<Action<ExpressionTestClass>> exp1 = x => x.MethodParam(1);
            Expression<Action<ExpressionTestClass>> exp2 = x => x.MethodParam(2);

            var h1 = ExpressionHasher.GetHashCode(exp1);
            var h2 = ExpressionHasher.GetHashCode(exp2);

            h1.Equals(h2).Should().BeFalse();
        }

        [Test(Description = "VisitNewArray")]
        public void ExpressionHasherTests_Get_NotSame_HashCode_Test4()
        {
            Expression<Func<ExpressionTestClass, string?>> exp1 = x => x.Convert(Array.Empty<int>());
            Expression<Func<ExpressionTestClass, string?>> exp2 = x => x.Convert(new[] { 1, 2, 3 });

            var h1 = ExpressionHasher.GetHashCode(exp1);
            var h2 = ExpressionHasher.GetHashCode(exp2);

            h1.Equals(h2).Should().BeFalse();
        }

        [Test]
        public void ExpressionHasherTests_Get_NotSame_HashCode_Test5()
        {
            Expression<Func<ExpressionTestClass, bool>> exp1 = x => string.Equals(x.FirstName, "Test1", StringComparison.Ordinal);
            Expression<Func<ExpressionTestClass, bool>> exp2 = x => string.Equals(x.FirstName, "Test2", StringComparison.Ordinal);

            var h1 = ExpressionHasher.GetHashCode(exp1);
            var h2 = ExpressionHasher.GetHashCode(exp2);

            h1.Equals(h2).Should().BeFalse();
        }

        [Test(Description = "VisitConstant")]
        public void ExpressionHasherTests_Get_NotSame_HashCode_Test6()
        {
            const string ConstString1 = "Test1";
            const string ConstString2 = "Test2";
            Expression<Func<ExpressionTestClass, string?>> exp1 = x => ConstString1;
            Expression<Func<ExpressionTestClass, string?>> exp2 = x => ConstString2;

            var h1 = ExpressionHasher.GetHashCode(exp1);
            var h2 = ExpressionHasher.GetHashCode(exp2);

            h1.Equals(h2).Should().BeFalse();
        }

        [Test(Description = "VisitNew")]
        public void ExpressionHasherTests_Get_NotSame_HashCode_Test7()
        {
            Expression<Func<ExpressionTestClass>> exp1 = () => new ExpressionTestClass("Name1");
            Expression<Func<ExpressionTestClass>> exp2 = () => new ExpressionTestClass("Name2");

            var h1 = ExpressionHasher.GetHashCode(exp1);
            var h2 = ExpressionHasher.GetHashCode(exp2);

            h1.Equals(h2).Should().BeFalse();
        }

        [Test]
        public void ExpressionHasherTests_Get_NotSame_HashCode_Test8()
        {
            Expression<Func<bool?>> exp1 = () => 4 < 5;
            Expression<Func<bool?>> exp2 = () => 4 > 5;

            var h1 = ExpressionHasher.GetHashCode(exp1);
            var h2 = ExpressionHasher.GetHashCode(exp2);

            h1.Equals(h2).Should().BeFalse();
        }

        [Test(Description = "VisitMemberAssignment")]
        public void ExpressionHasherTests_Get_NotSame_HashCode_Test9()
        {
            Expression<Func<ExpressionTestClass>> exp1 = () => new ExpressionTestClass { FirstName = "assignment1" };
            Expression<Func<ExpressionTestClass>> exp2 = () => new ExpressionTestClass { FirstName = "assignment2" };

            var h1 = ExpressionHasher.GetHashCode(exp1);
            var h2 = ExpressionHasher.GetHashCode(exp2);

            h1.Equals(h2).Should().BeFalse();
        }

        [Test(Description = "VisitTypeBinary")]
        public void ExpressionHasherTests_Get_NotSame_HashCode_Test10()
        {
            Expression<Func<ExpressionTestClass, bool>> exp1 = x => x.Children[0] is ICloneable;
            Expression<Func<ExpressionTestClass, bool>> exp2 = x => x.Children[0] is IComparable;

            var h1 = ExpressionHasher.GetHashCode(exp1);
            var h2 = ExpressionHasher.GetHashCode(exp2);

            h1.Equals(h2).Should().BeFalse();
        }

        [Test(Description = "VisitMemberMemberBinding")]
        public void ExpressionHasherTests_Get_NotSame_HashCode_Test11()
        {
            Expression<Func<ExpressionTestClass>> exp1 = () => new ExpressionTestClass { Nested1 = { SubMember1 = "MemberMemberBinding1" } };
            Expression<Func<ExpressionTestClass>> exp2 = () => new ExpressionTestClass { Nested1 = { SubMember1 = "MemberMemberBinding2" } };

            var h1 = ExpressionHasher.GetHashCode(exp1);
            var h2 = ExpressionHasher.GetHashCode(exp2);

            h1.Equals(h2).Should().BeFalse();
        }

        [Test(Description = "VisitMemberListBinding")]
        public void ExpressionHasherTests_Get_NotSame_HashCode_Test12()
        {
            Expression<Func<ExpressionTestClass>> exp1 = () => new ExpressionTestClass { Children = { new ExpressionTestClass() } };
            Expression<Func<ExpressionTestClass>> exp2 = () => new ExpressionTestClass { Children = { new ExpressionTestClass(), new ExpressionTestClass() } };

            var h1 = ExpressionHasher.GetHashCode(exp1);
            var h2 = ExpressionHasher.GetHashCode(exp2);

            h1.Equals(h2).Should().BeFalse();
        }


        [Test(Description = "VisitIndex")]
        public void ExpressionHasherTests_Get_NotSame_HashCode_Test14()
        {
            Expression<Func<ExpressionTestClass, int>> exp1 = x => x.Nested1[0];
            Expression<Func<ExpressionTestClass, int>> exp2 = x => x.Nested1[1];

            var h1 = ExpressionHasher.GetHashCode(exp1);
            var h2 = ExpressionHasher.GetHashCode(exp2);

            h1.Equals(h2).Should().BeFalse();
        }
    }
}
