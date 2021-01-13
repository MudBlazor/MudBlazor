﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudBlazor.Interop;

namespace MudBlazor
{
    public class ScrollEventArgs : EventArgs
    {

        /// <summary>
        /// The BoundingClientRect for the first child of the scrolled element
        /// </summary>
        public BoundingClientRect FirstChildBoundingClientRect { get; set; }

        /// <summary>
        /// The ScrollTop property gets or sets the number of pixels that an element's content is scrolled vertically
        /// </summary>
        public int ScrollTop { get; set; }

        /// <summary>
        /// The ScrollLeft property gets or sets the number of pixels that an element's content is scrolled from its left edge.
        /// </summary>
        public int ScrollLeft { get; set; }

        /// <summary>
        /// The ScrollHeight  property is a measurement of the height of an element's content, including content not visible on the screen due to overflow
        /// </summary>
        public int ScrollHeight { get; set; }

        /// <summary>
        /// The ScrollWidth property is a measurement of the width of an element's content, including content not visible on the screen due to overflow
        /// </summary>
        public int ScrollWidth { get; set; }

        /// <summary>
        /// Node name of the scrolled element
        /// </summary>
        public string NodeName { get; set; }

    }
}
