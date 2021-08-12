// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor
{
    /// <summary>
    /// A section (nav link) inside the MudPageContentNavigation
    /// </summary>
    public class MudPageContentSection
    {
        /// <summary>
        /// create a new instance with a title and and id
        /// </summary>
        /// <param name="title">name of the section will be displayed in the navigation</param>
        /// <param name="id">id of the section. It will be appending to the current url, if the section becomes active</param>
        public MudPageContentSection(string title, string id)
        {
            Title = title;
            Id = id;
        }

        /// <summary>
        /// The Title of the section. This will be displayed in the navigation
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Id of the section. It will be appending to the current url, if the section becomes active
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Indicating if the section is currently in the middle of the viewport
        /// </summary>
        public bool IsActive { get; private set; }

        protected internal void Activate() => IsActive = true;
        protected internal void Deactive() => IsActive = false;
    }
}
