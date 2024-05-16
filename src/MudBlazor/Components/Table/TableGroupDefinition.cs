// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MudBlazor
{
#nullable enable
    public class TableGroupDefinition<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T>
    {
        private bool _expandable;
        private bool _indentation;
        private TableContext<T>? _context;
        private TableGroupDefinition<T>? _innerGroup;

        public TableGroupDefinition()
        {
        }

        public TableGroupDefinition(Func<T, object> selector, TableGroupDefinition<T>? innerGroup = null)
        {
            Selector = selector;
            InnerGroup = innerGroup;
        }

        /// <summary>
        /// Gets or Sets the Group Name. It's useful for use on Header, for example.
        /// </summary>
        public string? GroupName { get; set; }

        /// <summary>
        /// The selector func to be used on .GroupBy() with LINQ.
        /// </summary>
        public Func<T, object>? Selector { get; set; }

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
        /// Gets or Sets if First Column cell must have Indentation.
        /// It must be set on First grouping level and works recursively.
        /// </summary>
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
        /// Gets or Sets if group header can Expand and Collapse its children.
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
        /// Gets or Sets if expandable group header is collapsed or expanded on initialization.
        /// </summary>
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
