// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor
{
    /// <summary>
    /// Stores a global MudGridItem CSS configuration
    /// The top level properties are default, but can be overridden by the xs, sm, md, lg, xl, xx breakpoint properties
    /// Passing this object to the Configuration property of a MudGridItem will apply these properties to the grid by its Id
    /// </summary>
    public class MatrixItemConfiguration
    {
        [Category(CategoryTypes.Item.Behavior)]
        [StyleAttribute("grid-column-start")]
        public string ColumnStart { get; set; }

        [Category(CategoryTypes.Item.Behavior)]
        [StyleAttribute("grid-column-end")]
        public string ColumnEnd { get; set; }

        [Category(CategoryTypes.Item.Behavior)]
        [StyleAttribute("grid-column")]
        public string Column { get; set; }

        [Category(CategoryTypes.Item.Behavior)]
        [StyleAttribute("grid-row-start")]
        public string RowStart { get; set; }

        [Category(CategoryTypes.Item.Behavior)]
        [StyleAttribute("grow-row-end")]
        public string RowEnd { get; set; }

        [Category(CategoryTypes.Item.Behavior)]
        [StyleAttribute("grid-row")]
        public string Row { get; set; }

        [Category(CategoryTypes.Item.Behavior)]
        [StyleAttribute("grid-area")]
        public string Area { get; set; }

        [Category(CategoryTypes.Item.Behavior)]
        [StyleAttribute("justify-self")]
        public Justify JustifySelf { get; set; }

        [Category(CategoryTypes.Item.Behavior)]
        [StyleAttribute("align-self")]
        public Align AlignSelf { get; set; }
        
        /// <summary>
        /// The number of columns spanned by this grid item.
        /// </summary>
        [Category(CategoryTypes.Item.Behavior)]
        public int ColumnSpan { get; set; }
        
        /// <summary>
        /// The number of rows spanned by this grid item.
        /// </summary>
        [Category(CategoryTypes.Item.Behavior)]
        public int RowSpan { get; set; }
        
        public MatrixItemConfiguration xs { get; set; }
        public MatrixItemConfiguration sm { get; set; }
        public MatrixItemConfiguration md { get; set; }
        public MatrixItemConfiguration lg { get; set; }
        public MatrixItemConfiguration xl { get; set; }
        public MatrixItemConfiguration xx { get; set; }
    }
}
