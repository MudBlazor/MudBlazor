// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

public class PointerEventsNoneOptions
{
    public bool EnableLogging { get; init; }

    public bool SubscribeDown { get; init; }

    public bool SubscribeUp { get; init; }

    public PointerEventsNoneOptions()
    {
    }

    public PointerEventsNoneOptions(
        bool enableLogging = false,
        bool subscribeDown = false,
        bool subscribeUp = false)
    {
        EnableLogging = enableLogging;
        SubscribeDown = subscribeDown;
        SubscribeUp = subscribeUp;
    }
}
