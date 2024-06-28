// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// A definition of a group within a <see cref="MudTable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of item being grouped.</typeparam>
    public class TableGroupDefinition<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T>
    {
        private bool _expandable;
        private bool _indentation;
        private TableContext<T>? _context;
        private TableGroupDefinition<T>? _innerGroup;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public TableGroupDefinition()
        {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="selector">The function which selects items for this group.</param>
        /// <param name="innerGroup">The group nested within this group.</param>
        public TableGroupDefinition(Func<T, object> selector, TableGroupDefinition<T>? innerGroup = null)
        {
            Selector = selector;
            InnerGroup = innerGroup;
        }

        /// <summary>
        /// The label for this group.
        /// </summary>
        public string? GroupName { get; set; }

        /// <summary>
        /// The function which selects items for this group.
        /// </summary>
        /// <remarks>
        /// Typically used during a LINQ <c>GroupBy()</c> call to group items.
        /// </remarks>
        public Func<T, object>? Selector { get; set; }

        /// <summary>
        /// The group definition within this definition.
        /// </summary>
        public TableGroupDefinition<T>? InnerGroup
        {
            get => _innerGroup;
            set
            {
                if (_innerGroup is not null)
                {
                    _innerGroup.Parent = null;
                    _innerGroup.Context = null;
                }

                _innerGroup = value;

                if (_innerGroup is not null)
                {
                    _innerGroup.Parent = this;
                    _innerGroup.Indentation = Indentation;
                    _innerGroup.Context = Context;
                }
            }
        }

        /// <summary>
        /// Indents the first column cell for this group and child groups.
        /// </summary>
        /// <remarks>
        /// When set, all child group definitions are also updated.  Must be set for the first grouping level.
        /// </remarks>
        public bool Indentation
        {
            get => _indentation;
            set
            {
                _indentation = value;
                if (InnerGroup is not null)
                {
                    InnerGroup.Indentation = value;
                }
            }
        }

        /// <summary>
        /// Allows this group to show or hide data rows.
        /// </summary>
        public bool Expandable
        {
            get => _expandable;
            set
            {
                _expandable = value;
                if (!_expandable)
                {
                    Context?.GroupRows.Where(gr => gr.GroupDefinition == this).ToList().ForEach(gr => gr.Expanded = IsInitiallyExpanded);
                }
            }
        }

        /// <summary>
        /// Shows data rows for this group when the table is first displayed.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        public bool IsInitiallyExpanded { get; set; } = true;

        internal TableGroupDefinition<T>? Parent { get; private set; }

        internal bool IsParentExpandable
        {
            get
            {
                if (Parent?.Expandable ?? false)
                {
                    return true;
                }

                return Parent?.IsParentExpandable ?? false;
            }
        }

        internal bool IsThisOrParentExpandable
        {
            get
            {
                if (Expandable)
                {
                    return Expandable;
                }

                return Parent?.IsThisOrParentExpandable ?? false;
            }
        }

        internal int Level
        {
            get
            {
                if (Parent is null)
                {
                    return 1;
                }

                return Parent.Level + 1;
            }
        }

        internal TableContext<T>? Context
        {
            get => _context;
            set
            {
                _context = value;
                if (_innerGroup is not null)
                {
                    _innerGroup.Context = _context;
                }
            }
        }
    }
}
