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

            [DataType(DataType.DateTime)]
            public DateTime Field4 { get; set; }

            [DataType(DataType.Currency)]
            public Decimal Field5 { get; set; }

            [DataType(DataType.Url)]
            public string Field6 { get; set; }

            [DataType(DataType.CreditCard)]
            public string Field7 { get; set; }

            [DataType(DataType.Time)]
            public DateTime Field8 { get; set; }
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
        public void GetInputTypeFromDataType_DateTime_Date()
        {
            var model = new TestClass3();

            Expression<Func<DateTime>> expression = () => model.Field4;

            expression.GetInputTypeFromDataType().Should().Be(InputType.Date);
        }

        [Test]
        public void GetInputTypeFromDataType_Currency()
        {
            var model = new TestClass3();

            Expression<Func<Decimal>> expression = () => model.Field5;

            expression.GetInputTypeFromDataType().Should().Be(InputType.Number);
        }

        [Test]
        public void GetInputTypeFromDataType_Url()
        {
            var model = new TestClass3();

            Expression<Func<string>> expression = () => model.Field6;

            expression.GetInputTypeFromDataType().Should().Be(InputType.Url);
        }

        [Test]
        public void GetInputTypeFromDataType_DateTime_Time()
        {
            var model = new TestClass3();

            Expression<Func<DateTime>> expression = () => model.Field8;

            expression.GetInputTypeFromDataType().Should().Be(InputType.Time);
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

        [Test]
        public void GetInputModeFromDataType_Decimal()
        {
            var model = new TestClass3();

            Expression<Func<decimal>> expression = () => model.Field5;

            expression.GetInputModeFromDataType().Should().Be(InputMode.@decimal);
        }

        [Test]
        public void GetInputModeFromDataType_Url()
        {
            var model = new TestClass3();

            Expression<Func<string>> expression = () => model.Field6;

            expression.GetInputModeFromDataType().Should().Be(InputMode.url);
        }

        [Test]
        public void GetInputModeFromDataType_CreditCard()
        {
            var model = new TestClass3();

            Expression<Func<string>> expression = () => model.Field7;

            expression.GetInputModeFromDataType().Should().Be(InputMode.numeric);
        }
    }
}
