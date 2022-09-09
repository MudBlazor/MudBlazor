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
        [Display(Name ="Date DisplayName")]
        public DateTime? Date { get; set; }
        [Display(Name = "Boolean DisplayName")]
        public bool Boolean { get; set; }
        [Display(Name = "String DisplayName")]
        public string String { get; set; }
    }
}
