// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.UnitTests.Utilities
{
    public class DisplayNameLabelClass
    {
        [Label("Date LabelAttribute")]
        public DateTime? Date { get; set; }
        [Label("Boolean LabelAttribute")]
        public bool Boolean { get; set; }
        [Label("String LabelAttribute")]
        public string String { get; set; }
    }
}
