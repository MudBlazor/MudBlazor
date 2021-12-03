using System;
using System.Reflection;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Other
{
    [TestFixture]
    public class CategoryAttributeTests
    {

        [Test]
        public void CategoryTypesClassConstantsAreCorrect()
        {
            foreach (Type component in typeof(CategoryTypes).GetNestedTypes())
            {
                foreach (FieldInfo category in component.GetFields())
                {
                    var categoryName = (string)category.GetRawConstantValue();
                    new CategoryAttribute(categoryName);
                }
            }
        }
    }
}
