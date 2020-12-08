using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities
{
    [TestFixture]
    public class MarkupCompilerTests
    {
        [Test]
        public void AttributePostprocessingTest1()
        {
            // pull out quotation marks
            var source = "<span class=\"htmlAttributeValue\">&quot;Some random value&quot;</span>";
            var actual = Docs.Compiler.Program.AttributePostprocessing(source);
            var expected = "<span class=\"quot\">&quot;</span><span class=\"htmlAttributeValue\">Some random value</span><span class=\"quot\">&quot;</span>";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AttributePostprocessingTest2()
        {
            // true, false
            var source = "<span class=\"htmlAttributeValue\">&quot;true&quot;</span>";
            var actual = Docs.Compiler.Program.AttributePostprocessing(source);
            var expected = "<span class=\"quot\">&quot;</span><span class=\"keyword\">true</span><span class=\"quot\">&quot;</span>";
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AttributePostprocessingTest3()
        {
            // handle enums
            var source = "<span class=\"htmlAttributeValue\">&quot;Color.Primary&quot;</span>";
            var actual = Docs.Compiler.Program.AttributePostprocessing(source);
            var expected = "<span class=\"quot\">&quot;</span><span class=\"enum\">Color</span><span class=\"enumValue\">.Primary</span><span class=\"quot\">&quot;</span>";
            Assert.AreEqual(expected, actual);
        }
    }
}
