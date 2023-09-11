// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MudBlazor;

#nullable enable
public class MudDragAndDropIndexChangedEventArgs : EventArgs
{
    public string ZoneIdentifier { get; }

    public int Index { get; }

    public string OldZoneIdentifier { get; }

    public MudDragAndDropIndexChangedEventArgs(string zoneIdentifier, string oldZoneIdentifier, int index)
    {
        ZoneIdentifier = zoneIdentifier;
        Index = index;
        OldZoneIdentifier = oldZoneIdentifier;
    }
}
