// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents the options for <see cref="IPopoverService"/>.
    /// </summary>
    public class PopoverOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether to check for the presence of a popover provider <see cref="MudPopoverProvider"/>.
        /// The default value is <c>true</c>.
        /// </summary>
        public bool CheckForPopoverProvider { get; set; } = true;

        /// <summary>
        /// Gets or sets the CSS class of the popover container.
        /// The default value is <c>mudblazor-main-content</c>.
        /// </summary>
        public string ContainerClass { get; set; } = "mudblazor-main-content";

        /// <summary>
        /// Gets or sets the FlipMargin for the popover.
        /// The default value is <c>0</c>.
        /// </summary>
        public int FlipMargin { get; set; } = 0;

        /// <summary>
        /// Gets the delay for batch popovers detachment.
        /// The default value is <c>0.5 seconds</c>.
        /// </summary>
        public TimeSpan QueueDelay { get; set; } = TimeSpan.FromSeconds(0.5);

        /// <summary>
        /// Gets or sets a value indicating whether to throw an exception when a duplicate <see cref="MudPopoverProvider"/> is encountered.
        /// The default value is <c>true</c>.
        /// </summary>
        public bool ThrowOnDuplicateProvider { get; set; } = true;

        /// <summary>
        /// Gets or sets the mode for displaying popovers.
        /// The default value is <c>PopoverMode.Default</c>.
        /// </summary>
        /// <remarks>
        /// This property determines the behavior of popovers. You can set it to either <see cref="PopoverMode.Default"/>
        /// to use the <see cref="IPopoverService"/> or <see cref="PopoverMode.Legacy"/> to use the old <see cref="IMudPopoverService"/>
        /// for backward compatibility.
        /// </remarks>
        public PopoverMode Mode { get; set; } = PopoverMode.Default;

        /// <summary>
        /// The size of the pool to use in order for generated objects to be reused. This is NOT a concurrency limit,
        /// but if the pool is empty then a new object will be created rather than waiting for an object to return to
        /// the pool.
        /// Defaults to 3000.
        /// <para/>
        /// <b>NB!</b> This setting is ignored in <see cref="PopoverMode.Default"/>.
        /// </summary>
        /// <remarks>
        /// For more info please visit: <see href="https://github.com/MarkCiliaVincenti/AsyncKeyedLock/wiki/How-to-use-AsyncKeyedLocker#pooling">AsyncKeyedLocker</see>.
        /// </remarks>
        public int PoolSize { get; set; } = 3000;

        /// <summary>
        /// The number of items to fill the pool with during initialization.
        /// Defaults to 100.
        /// <para/>
        /// <b>NB!</b> This setting is ignored in <see cref="PopoverMode.Default"/>.
        /// </summary>
        /// <remarks>
        /// For more info please visit: <see href="https://github.com/MarkCiliaVincenti/AsyncKeyedLock/wiki/How-to-use-AsyncKeyedLocker#pooling">AsyncKeyedLocker</see>.
        /// </remarks>
        public int PoolInitialFill { get; set; } = 100;
    }
}
