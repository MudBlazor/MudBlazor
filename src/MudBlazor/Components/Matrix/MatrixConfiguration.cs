// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MudBlazor
{
    /// <summary>
    /// Stores a global MudGrid CSS configuration
    /// The top level properties are default, but can be overridden by the xs, sm, md, lg, xl, xx breakpoint properties
    /// Passing this object to the Configuration property of a MudCSSGrid will apply these properties to the grid by its Id
    /// </summary>
    public class MatrixConfiguration
    {
        private List<string> Grids = new List<string>();

        [Category(CategoryTypes.Grid.Behavior)]
        [StyleAttribute("grid-template-columns")]
        public string TemplateColumns { get; set; }

        [Category(CategoryTypes.Grid.Behavior)]
        [StyleAttribute("grid-template-rows")]
        public string TemplateRows { get; set; }

        [Category(CategoryTypes.Grid.Behavior)]
        [StyleAttribute("grid-template-rows")]
        public string TemplateAreas { get; set; }

        [Category(CategoryTypes.Grid.Behavior)]
        [StyleAttribute("grid-column-gap")]
        public string ColumnGap { get; set; }

        [Category(CategoryTypes.Grid.Behavior)]
        [StyleAttribute("grid-row-gap")]
        public string RowGap { get; set; }

        [Category(CategoryTypes.Grid.Behavior)]
        [StyleAttribute("grid-gap")]
        public string Gap { get; set; }

        [Category(CategoryTypes.Grid.Behavior)]
        [StyleAttribute("justify-items")]
        public Justify JustifyItems { get; set; }

        [Category(CategoryTypes.Grid.Behavior)]
        [StyleAttribute("align-items")]
        public Align AlignItems { get; set; }

        [Category(CategoryTypes.Grid.Behavior)]
        [StyleAttribute("justify-content")]
        public Justify JustifyContent { get; set; }

        [Category(CategoryTypes.Grid.Behavior)]
        [StyleAttribute("align-content")]
        public Align AlignContent { get; set; }

        /// <summary>
        /// The number of columns spanned by this grid.
        /// </summary>
        [Category(CategoryTypes.Grid.Behavior)]
        public int ColumnCount { get; set; }

        /// <summary>
        /// The number of rows spanned by this grid.
        /// </summary>
        [Category(CategoryTypes.Grid.Behavior)]
        public int RowCount { get; set; }

        public MatrixConfiguration xs { get; set; }
        public MatrixConfiguration sm { get; set; }
        public MatrixConfiguration md { get; set; }
        public MatrixConfiguration lg { get; set; }
        public MatrixConfiguration xl { get; set; }
        public MatrixConfiguration xx { get; set; }

        public MatrixItemConfiguration GlobalMatrixItemConfiguration { get; set; }
        public MatrixItemConfiguration XsMatrixItemConfiguration { get; set; }
        public MatrixItemConfiguration SmMatrixItemConfiguration { get; set; }
        public MatrixItemConfiguration MdMatrixItemConfiguration { get; set; }
        public MatrixItemConfiguration LgMatrixItemConfiguration { get; set; }
        public MatrixItemConfiguration XlMatrixItemConfiguration { get; set; }
        public MatrixItemConfiguration XxMatrixItemConfiguration { get; set; }

        public void AddGrid(string gridId) => Grids.Add('#' + gridId);
        public bool RemoveGrid(string gridId) => Grids.Remove('#' + gridId);

        public string BuildConfiguration(Breakpoint? breakpoint = null)
        {
            Console.WriteLine(nameof(BuildConfiguration));
            Console.WriteLine(this.GetType().GetProperties().Length);
            StringBuilder builder = new StringBuilder();

            foreach (PropertyInfo prop in this.GetType().GetProperties())
            {
                Console.WriteLine(prop);
                if (prop.GetValue(this, null) is MatrixConfiguration childConfiguration)
                {
                    builder.Append(childConfiguration.BuildConfiguration());
                    continue;
                }
                if (prop.GetValue(this, null) is MatrixItemConfiguration childConfiguration2)
                {
                    //builder.Append(childConfiguration2.BuildConfiguration());
                    continue;
                }

                var styleValue = prop.GetValue(this, null);
                Console.WriteLine("styleValue: " + styleValue);
                if (styleValue is null) continue;
                
                var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                var style = type.GetCustomAttribute<StyleAttributeAttribute>();
                Console.WriteLine("style: " + style);
                if (style != null)
                {
                    builder.AppendLine(style + ": " + styleValue + ";");
                }
            }

            if (builder.Length == 0) return string.Empty; //return if there are no items

            builder.Insert(0, "{");
            builder.Insert(0, string.Join(", ", Grids));
            builder.AppendLine("}");

            return builder.ToString();
        }
    }
}
