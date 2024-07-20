using System;
using System.Collections.Generic;

namespace MudBlazor
{

    /// <summary>
    /// Specifies the name of the category in which to group the property of a MudBlazor component when displayed in the API documentation.
    /// </summary>
    /// <remarks>
    /// Use this attribute together with the <see cref="Microsoft.AspNetCore.Components.ParameterAttribute"/>. <br/>
    /// This attribute is similar to <see cref="System.ComponentModel.CategoryAttribute"/>. <br/>
    /// The name of the category can be specified by using a constant defined in the <see cref="CategoryTypes"/> class.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class CategoryAttribute : Attribute
    {
        public CategoryAttribute(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("The category name cannot be null nor empty.");
            if (!categoryOrder.ContainsKey(name))
                throw new ArgumentException($"The given category name '{name}' isn't in the categoryOrder field.");
            Name = name;
        }

        /// <summary> The name of the category. </summary>
        public string Name { get; }

        /// <summary> The order of the category - the greater the number the lower the category will be displayed in the API documentation. </summary>
        public int Order => categoryOrder[Name];

        // Possible categories of component properties and the order in which they are displayed in the API documentation.
        private static readonly Dictionary<string, int> categoryOrder = new()
        {
            ["Data"] = 0, // general category
            ["Validation"] = 1, // general category

            // specific categories associated with data
            ["Validated data"] = 2,
            ["Validation result"] = 3,

            ["Behavior"] = 100, // general category

            ["Header"] = 101,
            ["Rows"] = 102,
            ["Footer"] = 103,

            // specific behaviors of a component
            ["Filtering"] = 200,
            ["Grouping"] = 201,
            ["Expanding"] = 202,
            ["Sorting"] = 203,
            ["Pagination"] = 204,
            ["Selecting"] = 205,
            ["Editing"] = 206,
            ["Click action"] = 207,
            ["Items"] = 208,
            ["Disable"] = 209,
            ["DraggingClass"] = 210,
            ["DropRules"] = 211,

            ["Appearance"] = 300, // general category

            // specific parts of a component together with their behavior and appearance
            ["Popup behavior"] = 400,
            ["Popup appearance"] = 401,
            ["List behavior"] = 402,
            ["List appearance"] = 403,
            ["Picker behavior"] = 404,
            ["Picker appearance"] = 405,
            ["Dot"] = 406,

            // "Miscellaneous" category. In classes inheriting from MudComponentBase it can be used only exceptionally -
            //  - only when the property can define behavior or appearance depending on value of the property.
            ["Misc"] = int.MaxValue - 1,

            ["Common"] = int.MaxValue // general category
        };
    }

    /// <summary>
    /// Possible categories of MudBlazor components properties.
    /// </summary>
    /// <remarks>
    ///     <b>General categories</b>
    ///     <para>
    ///       - <i>Data</i>       - Used e.g. in form fields, pickers, <see cref="MudRating"/>, <see cref="MudTable{T}"/>, <see cref="MudTreeView{T}"/>,
    ///                              <see cref="MudTreeViewItem{T}"/>, and <see cref="MudCarousel{TData}"/>. Containers have this group when their items can be defined
    ///                             not only in markup language, but also programmatically in the Items property and by specifying ItemTemplate.<br/>
    ///       - <i>Validation</i> - Used in form fields and pickers.<br/>
    ///       - <i>Behavior</i>   - Changing these properties changes behavior of the component and behavior of the application. So in some way they are or may be more
    ///                             important than the "Appearance" category. Example properties are: a) the <c>Disabled</c> property, b) icons (or avatars) without default value
    ///                             (because setting their value can pass additional information).<br/>
    ///       - <i>Appearance</i> - Changing these properties doesn't change behavior of the component and behavior of the application, but only changes the appearance
    ///                             of the component irrelevant to the understanding of the application by a user. So in some way they are less important than the "Behavior"
    ///                             category, because they are only used to adjust the look of the application. Example properties are: a) <c>Elevation</c>, <c>Outlined</c>,
    ///                             <c>Square</c>, <c>Rounded</c>, <c>Gutters</c>, <c>Dense</c>, <c>Ripple</c>; b) size, color, and typography of the item
    ///                             and its subelements; c) CSS classes and styles of subelements; d) icons with the default value already set (because most often changing its value
    ///                             doesn't change passed information).<br/>
    ///       - <i>Common</i>     - Properties defined in <see cref="MudComponentBase"/>.
    ///     </para>
    ///     <para>
    ///     Note: The following properties belong to the "Behavior" group, not to the "Appearance" group:<br/>
    ///      - <see cref="MudIconButton.Icon"/> - because it describes meaning of the button, since MudIconButton doesn't have text,<br/>
    ///      - <see cref="MudBaseInput{T}.Label"/> - because it describes meaning of the field,<br/>
    ///      - <see cref="MudBaseInput{T}.AdornmentText"/> - because it can describe information important to a user, e.g. a numeric field unit such as kilograms.<br/>
    ///     Sometimes choosing a category can be difficult - in such case choose a category that makes the most sense.
    ///     </para>
    ///
    ///     <b>Categories for specific behaviors or specific parts of components</b>
    ///     <para>If some elements or behaviors can be distinguished in a component, then their properties are included in separate groups.</para>
    ///
    ///     <para>Note: If a property qualifies for both the "Appearance" or "Behavior" group, and for some special group, then this special group takes precedence.
    ///           For example, <see cref="MudTableBase.CommitEditIcon"/> could belong to the "Appearance" group, but belongs to the "Editing" group.</para>
    /// 
    ///     <b>Additional information</b>
    ///     <para>The list of categories is inspired by the categories displayed for Windows Forms and Web Forms components in the Visual Studio "Properties" window.</para>
    /// </remarks>
    public static class CategoryTypes
    {
        /* Implementation note:
         * Almost all components use the "Behavior" and "Appearance" categories. We could simplify the code
         * by inheriting these constants, but C# doesn't allow to inherit static members of a class.
         */

        /// <summary>Used in <see cref="MudComponentBase"/>.</summary>
        public static class ComponentBase
        {
            public const string Common = "Common";
        }

        /// <summary>Used in: <see cref="MudBaseButton"/>, all components inheriting from it, and <see cref="MudToggleIconButton"/>.</summary>
        public static class Button
        {
            public const string Behavior = "Behavior";
            public const string ClickAction = "Click action";
            public const string Appearance = "Appearance";
        }

        /// <summary>Used in <see cref="MudFormComponent{T, U}"/> and all components inheriting from it.</summary>
        public static class FormComponent
        {
            public const string Data = "Data";
            public const string Validation = "Validation";
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
            public const string ListBehavior = "List behavior";
            public const string ListAppearance = "List appearance";
            public const string PickerBehavior = "Picker behavior";
            public const string PickerAppearance = "Picker appearance";
        }

        /// <summary>Used in all charts, that is in <see cref="MudCategoryChartBase"/> and all components inheriting from it.</summary>
        public static class Chart
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        /// <summary>Used in other base classes.</summary>
        public static class General
        {
            public const string Data = "Data";
            public const string Behavior = "Behavior";
            public const string ClickAction = "Click action";
            public const string Appearance = "Appearance";
        }

        public static class Alert
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class AppBar
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class Avatar
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class AvatarGroup
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class Badge
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class Breadcrumbs
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class BreakpointProvider
        {
            public const string Behavior = "Behavior";
        }

        public static class ButtonGroup
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class Card
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class Carousel
        {
            public const string Data = "Data";
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class Chip
        {
            public const string Behavior = "Behavior";
            public const string ClickAction = "Click action";
            public const string Appearance = "Appearance";
        }

        public static class ChipSet
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class Container
        {
            public const string Behavior = "Behavior";
        }

        public static class Dialog
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
            public const string Misc = "Misc";
        }

        public static class Divider
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class Drawer
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class DropZone
        {
            public const string Appearance = "Appearance";
            public const string Behavior = "Behavior";
            public const string Disabled = "Disable";
            public const string Sorting = "Sorting";
            public const string DraggingClass = "DraggingClass";
            public const string DropRules = "DropRules";
            public const string Items = "Items";
        }

        public static class Element
        {
            public const string Misc = "Misc";
        }

        public static class ExpansionPanel
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class Field
        {
            public const string Data = "Data";
            public const string Validation = "Validation";
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class FileUpload
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class FocusTrap
        {
            public const string Behavior = "Behavior";
        }

        public static class Form
        {
            public const string ValidatedData = "Validated data";
            public const string ValidationResult = "Validation result";
            public const string Behavior = "Behavior";
        }

        public static class Grid
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class Highlighter
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class Image
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class Item
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class Hidden
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class Icon
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class Link
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class List
        {
            public const string Behavior = "Behavior";
            public const string Expanding = "Expanding";
            public const string Selecting = "Selecting";
            public const string ClickAction = "Click action";
            public const string Appearance = "Appearance";
        }

        public static class MainContent
        {
            public const string Behavior = "Behavior";
        }

        public static class Menu
        {
            public const string Behavior = "Behavior";
            public const string ClickAction = "Click action";
            public const string Appearance = "Appearance";
            public const string PopupBehavior = "Popup behavior";
            public const string PopupAppearance = "Popup appearance";
        }

        public static class MessageBox
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class NavMenu
        {
            public const string Behavior = "Behavior";
            public const string ClickAction = "Click action";
            public const string Appearance = "Appearance";
        }

        public static class Overlay
        {
            public const string Behavior = "Behavior";
            public const string ClickAction = "Click action";
            public const string Appearance = "Appearance";
        }

        public static class Pagination
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class Paper
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class Picker
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class Popover
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class ProgressLinear
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class ProgressCircular
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class Radio
        {
            public const string Data = "Data";
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class Rating
        {
            public const string Data = "Data";
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class RTLProvider
        {
            public const string Behavior = "Behavior";
        }

        public static class ScrollToTop
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class Skeleton
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class Stack
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class Slider
        {
            public const string Data = "Data";
            public const string Validation = "Validation";
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class SwipeArea
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class Table
        {
            public const string Data = "Data";
            public const string Behavior = "Behavior";
            public const string Header = "Header";
            public const string Rows = "Rows";
            public const string Footer = "Footer";
            public const string Filtering = "Filtering";
            public const string Grouping = "Grouping";
            public const string Sorting = "Sorting";
            public const string Pagination = "Pagination";
            public const string Selecting = "Selecting";
            public const string Editing = "Editing";
            public const string Appearance = "Appearance";
        }

        public static class SimpleTable
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class Tabs
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class Timeline
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
            public const string Dot = "Dot";
        }

        public static class ToolBar
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class Tooltip
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }

        public static class TreeView
        {
            public const string Data = "Data";
            public const string Behavior = "Behavior";
            public const string Expanding = "Expanding";
            public const string Selecting = "Selecting";
            public const string ClickAction = "Click action";
            public const string Appearance = "Appearance";
        }

        public static class Text
        {
            public const string Behavior = "Behavior";
            public const string Appearance = "Appearance";
        }
    }
}
