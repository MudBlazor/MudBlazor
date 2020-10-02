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
        public void AttributePostprocessingTests()
        {
            var source = "<span class=\"htmlAttributeValue\">\"This is the value\"</span>";
            var actual = Docs.Compiler.Program.AttributePostprocessing(source);
            var expected = "<span class=\"quot\">&quot;</span><span class=\"htmlAttributeValue\">This is the value</span><span class=\"quot\">&quot;</span>";
            Assert.AreEqual(expected, actual);
        }
    }
}
