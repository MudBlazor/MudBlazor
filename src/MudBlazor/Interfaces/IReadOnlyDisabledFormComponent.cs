// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.Interfaces
{
    public interface IReadOnlyDisabledFormComponent : IFormComponent
    {
        public bool ReadOnly { get; set; }
        public bool Disabled { get; set; }
    }
}
