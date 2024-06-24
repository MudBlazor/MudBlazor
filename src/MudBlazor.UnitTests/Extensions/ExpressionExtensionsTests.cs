using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using FluentAssertions;
using MudBlazor;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Extensions
{
    public class ExpressionExtensionsTests
    {
        private class TestClass
        {
            [Label("Label Attribute")]
            public string Field1 { get; set; }

            public TestClass1 TestClass1 { get; set; }

            [Label("NonNullableDateTimeField Label Attribute")]
            public DateTime NonNullableDateTimeField { get; set; }
        }

        private class TestClass1
        {
            public string Field1 { get; set; }

            public TestClass2 TestClass2 { get; set; }
        }

        private class TestClass2
        {
            public string Field1 { get; set; }
        }

        [Test]
        public void GetFullPathOfMemberTest()
        {
            var model = new TestClass();

            Expression<Func<string>> expression = () => model.Field1;

            expression.GetFullPathOfMember().Should().Be("Field1");
        }

        [Test]
        public void GetFullPathOfNonNullableMemberTest()
        {
            var model = new TestClass();

            Expression<Func<DateTime?>> expression = () => model.NonNullableDateTimeField;

            expression.GetFullPathOfMember().Should().Be("NonNullableDateTimeField");
        }

        [Test]
        public void GetFullPathOfMemberTest1()
        {
            var model = new TestClass();

            Expression<Func<string>> expression = () => model.TestClass1.Field1;

            expression.GetFullPathOfMember().Should().Be("TestClass1.Field1");
        }

        [Test]
        public void GetFullPathOfMemberTest2()
        {
            var model = new TestClass();

            Expression<Func<string>> expression = () => model.TestClass1.TestClass2.Field1;

            expression.GetFullPathOfMember().Should().Be("TestClass1.TestClass2.Field1");
        }

        [Test]
        public void GetLabelStringTest1()
        {
            var model = new TestClass();

            Expression<Func<string>> expression = () => model.Field1;

            expression.GetLabelString().Should().Be("Label Attribute");
        }

        [Test]
        public void GetLabelStringTest2()
        {
            var model = new TestClass1();

            Expression<Func<string>> expression = () => model.Field1;

            expression.GetLabelString().Should().Be("");
        }

        [Test]
        public void GetLabelStringTest3()
        {
            var model = new TestClass();

            Expression<Func<DateTime?>> expression = () => model.NonNullableDateTimeField;

            expression.GetLabelString().Should().Be("NonNullableDateTimeField Label Attribute");
        }
    }
}
