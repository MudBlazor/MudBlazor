using System.Reflection;
using AwesomeAssertions;
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
                typeof(TemplateColumn<>),
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
                typeof(MudBlazor.Charts.Legend<>),
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

        [TestCase(null)]
        [TestCase("")]
        public void Constructor_NullOrEmptyName_Throws(string name)
        {
            var construct = () => new CategoryAttribute(name);

            construct.Should().Throw<ArgumentException>().WithMessage("The category name cannot be null nor empty.");
        }

        [Test]
        public void Constructor_UnknownName_Throws()
        {
            // A typo or a category that isn't registered in the order table must be rejected.
            var construct = () => new CategoryAttribute("Behaviour");

            construct.Should().Throw<ArgumentException>().WithMessage("*isn't in the categoryOrder field*");
        }

        [Test]
        public void Order_GeneralCategories_AscendingByImportance()
        {
            // Documentation ordering contract: Data before Behavior before Appearance before Common.
            var data = new CategoryAttribute(CategoryTypes.General.Data).Order;
            var behavior = new CategoryAttribute(CategoryTypes.General.Behavior).Order;
            var appearance = new CategoryAttribute(CategoryTypes.General.Appearance).Order;
            var common = new CategoryAttribute(CategoryTypes.ComponentBase.Common).Order;

            data.Should().BeLessThan(behavior);
            behavior.Should().BeLessThan(appearance);
            appearance.Should().BeLessThan(common);
        }

        // Returns the class that declares the specified method.
        private static Type GetBaseDefinitionClass(MethodInfo m) => m.GetBaseDefinition().DeclaringType;

        // Returns the class that declares the specified property.
        private static Type GetBaseDefinitionClass(PropertyInfo p) => GetBaseDefinitionClass(p.GetMethod ?? p.SetMethod);
    }
}
