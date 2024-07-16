// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

#nullable enable
namespace MudBlazor
{
    /// <summary>
    /// A section (nav link) inside the MudPageContentNavigation
    /// </summary>
    public class MudPageContentSection
    {
        private readonly List<MudPageContentSection> _children = new();

        public int LevelSortingValue { get; private set; }

        public MudPageContentSection? Parent { get; }

        public int Level { get; set; }

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
        public bool Active { get; private set; }

        /// <summary>
        /// create a new instance with a title and id and level set to zero
        /// </summary>
        /// <param name="title">name of the section will be displayed in the navigation</param>
        /// <param name="id">id of the section. It will be appending to the current url, if the section becomes active</param>
        public MudPageContentSection(string title, string id)
            : this(title, id, 0, null)
        {
        }

        /// <summary>
        /// create a new instance with a title and id and level
        /// </summary>
        /// <param name="title">name of the section will be displayed in the navigation</param>
        /// <param name="id">id of the section. It will be appending to the current url, if the section becomes active</param>
        /// <param name="level">The level within the hierachy</param>
        /// <param name="parent">The parent of the section. null if there is no parent or no hierachy</param>
        public MudPageContentSection(string title, string id, int level, MudPageContentSection? parent)
        {
            Title = title;
            Id = id;
            Level = level;
            Parent = parent;
            Parent?._children.Add(this);
        }

        protected internal void Activate() => Active = true;

        protected internal void Deactive() => Active = false;

        internal void SetLevelStructure(int counter, int diff)
        {
            LevelSortingValue = counter;
            var levelDiff = diff / 10;
            var value = counter + levelDiff;
            foreach (var item in _children)
            {
                item.SetLevelStructure(value, levelDiff);
                value += levelDiff;
            }
        }
    }
}
