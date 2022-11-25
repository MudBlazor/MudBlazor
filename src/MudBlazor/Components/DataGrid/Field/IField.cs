// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;

namespace MudBlazor
{
    public interface IField
    {
        string Name { get; }

        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type Type { get; }
    }
}
