// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable
internal interface IMudRadioGroup
{
    //This interface need to throw exception properly.
    void CheckGenericTypeMatch(object selectItem);
}
