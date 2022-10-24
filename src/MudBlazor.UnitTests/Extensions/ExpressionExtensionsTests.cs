using FluentAssertions;
using MudBlazor;
using NUnit.Framework;
using System.Linq.Expressions;
using System;
using System.ComponentModel.DataAnnotations;

namespace MudBlazor.UnitTests.Extensions
{
    public class ExpressionExtensionsTests
    {
        private class TestClass
        {
            [Label("Label Attribute")]
            public string Field1 { get; set; }

            public TestClass1 TestClass1 { get; set; }
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

        private class TestClass3
        {
            public string Field1 { get; set; }

            [DataType(DataType.Password)]
            public string Field2 { get; set; }

            [DataType(DataType.PhoneNumber)]
            public string Field3 { get; set; }
        }

        [Test]
        public void GetFullPathOfMemberTest()
        {
            var model = new TestClass();

            Expression<Func<string>> expression = () => model.Field1;

            expression.GetFullPathOfMember().Should().Be("Field1");
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
        public void GetInputTypeFromDataType_Default()
        {
            var model = new TestClass3();

            Expression<Func<string>> expression = () => model.Field1;

            expression.GetInputTypeFromDataType().Should().Be(InputType.Text);
        }

        [Test]
        public void GetInputTypeFromDataType_Password()
        {
            var model = new TestClass3();

            Expression<Func<string>> expression = () => model.Field2;

            expression.GetInputTypeFromDataType().Should().Be(InputType.Password);
        }

        [Test]
        public void GetInputTypeFromDataType_Telephone()
        {
            var model = new TestClass3();

            Expression<Func<string>> expression = () => model.Field3;

            expression.GetInputTypeFromDataType().Should().Be(InputType.Telephone);
        }

        [Test]
        public void GetInputModeFromDataType_Default()
        {
            var model = new TestClass3();

            Expression<Func<string>> expression = () => model.Field1;

            expression.GetInputModeFromDataType().Should().Be(InputMode.text);
        }

        [Test]
        public void GetInputModeFromDataType_Password()
        {
            var model = new TestClass3();

            Expression<Func<string>> expression = () => model.Field2;

            expression.GetInputModeFromDataType().Should().Be(InputMode.text);
        }

        [Test]
        public void GetInputModeFromDataType_Telephone()
        {
            var model = new TestClass3();

            Expression<Func<string>> expression = () => model.Field3;

            expression.GetInputModeFromDataType().Should().Be(InputMode.tel);
        }
    }
}
