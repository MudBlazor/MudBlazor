// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor
{
    /// <summary>
    /// The position of the Chat Bubble Arrow
    /// </summary>
    public enum ChatArrowPosition
    {
        /// <summary>
        /// The arrow is attached to the top.
        /// </summary>
        [Description("top")]
        Top,

        /// <summary>
        /// The arrow is attached to the middle.
        /// </summary>
        [Description("middle")]
        Middle,

        /// <summary>
        /// The arrow is attached to the bottom.
        /// </summary>
        [Description("bottom")]
        Bottom,

        /// <summary>
        /// The arrow is not shown.
        /// </summary>
        [Description("none")]
        None,
    }
}
