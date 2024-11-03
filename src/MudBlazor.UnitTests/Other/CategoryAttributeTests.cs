using System;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Other
{
    [TestFixture]
    public class CategoryAttributeTests
    {
        [Test]
        public void CategoryTypesClassConstantsAreCorrect()
        {
            foreach (var component in typeof(CategoryTypes).GetNestedTypes())
            {
                foreach (var category in component.GetFields())
                {
                    var categoryName = (string)category.GetRawConstantValue();
                    new CategoryAttribute(categoryName);
                }
            }
        }

        [Test]
        public void AllComponentPropertiesHaveCategories()
        {
            // Currently, these classes inheriting from MudComponentBase have uncategorized properties.
            // If you want you can categorize them, and then remove from this list.
            Type[] exceptions = {
                typeof(MudDataGrid<>),  // TODO: remove it later
                typeof(FilterHeaderCell<>),
                typeof(Column<>),
                typeof(PropertyColumn<,>),
                typeof(HeaderCell<>),
                typeof(FooterCell<>),
                typeof(Cell<>),
                typeof(HeaderContext<>),
                typeof(FooterContext<>),
                typeof(CellContext<>),
                typeof(MudDataGridPager<>),
                typeof(SelectColumn<>),
                typeof(HierarchyColumn<>),

                typeof(MudTHeadRow),
                typeof(MudTFootRow),
                typeof(MudTr),
                typeof(MudTh),
                typeof(MudTd),
                typeof(MudTablePager),
                typeof(MudTableSortLabel<>),
                typeof(MudTableGroupRow<>),

                typeof(MudInput<>),
                typeof(MudInputControl),
                typeof(MudInputLabel),
                typeof(MudRangeInput<>),

                typeof(MudCollapse),
                typeof(MudPageContentNavigation),
                typeof(MudSnackbarElement),
                typeof(MudBlazor.Charts.Legend),

                typeof(MudRatingItem),  // TODO: remove it later; see also: https://github.com/MudBlazor/MudBlazor/discussions/3452
            };

            var isTestOK = true;

            var components = typeof(MudElement).Assembly.GetTypes()
                                                                      .Where(type => type.IsSubclassOf(typeof(MudComponentBase)))
                                                                      .Except(exceptions);

            foreach (var component in components)
            {
                foreach (var property in component.GetProperties())
                {
                    if (GetBaseDefinitionClass(property) == component &&              // property isn't inherited
                        !property.PropertyType.Name.Contains("EventCallback") &&      // property isn't an event callback
                        property.GetCustomAttribute<ObsoleteAttribute>() == null &&   // property isn't obsolete
                        property.GetCustomAttribute<ParameterAttribute>() != null &&  // property has the [Parameter] attribute
                        property.GetCustomAttribute<CategoryAttribute>() == null)     // property doesn't have a category
                    {
                        isTestOK = false;
                    }
                }
            }

            // If this fails some component properties don't have categories.
            isTestOK.Should().BeTrue();
        }

        // Returns the class that declares the specified method.
        private static Type GetBaseDefinitionClass(MethodInfo m) => m.GetBaseDefinition().DeclaringType;

        // Returns the class that declares the specified property.
        private static Type GetBaseDefinitionClass(PropertyInfo p) => GetBaseDefinitionClass(p.GetMethod ?? p.SetMethod);
    }
}
