// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MudBlazor
{
    public class TableGroupDefinition<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T>
    {
        public TableGroupDefinition()
        {
        }

        public TableGroupDefinition(Func<T, object> selector, TableGroupDefinition<T> innerGroup = null)
        {
            Selector = selector;
            InnerGroup = innerGroup;
        }

        /// <summary>
        /// Gets or Sets the Group Name. It's useful for use on Header, for example.
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// The selector func to be used on .GroupBy() with LINQ.
        /// </summary>
        public Func<T, object> Selector { get; set; }

        private TableGroupDefinition<T> _innerGroup;
        public TableGroupDefinition<T> InnerGroup
        {
            get => _innerGroup;
            set
            {
                if (_innerGroup != null)
                {
                    _innerGroup.Parent = null;
                    _innerGroup.Context = null;
                }

                _innerGroup = value;

                if (_innerGroup != null)
                {
                    _innerGroup.Parent = this;
                    _innerGroup.Indentation = Indentation;
                    _innerGroup.Context = Context;
                }
            }
        }

        internal TableGroupDefinition<T> Parent { get; private set; }

        private bool _indentation;
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
                if (InnerGroup != null)
                    InnerGroup.Indentation = value;
            }

        }

        private bool _expandable = false;
        /// <summary>
        /// Gets or Sets if group header can Expand and Collapse its children.
        /// </summary>
        public bool Expandable
        {
            get => _expandable;
            set
            {
                _expandable = value;
                if (_expandable == false)
                    Context?.GroupRows.Where(gr => gr.GroupDefinition == this).ToList().ForEach(gr => gr.IsExpanded = IsInitiallyExpanded);
            }
        }

        private bool _isInitiallyExpanded = true;
        /// <summary>
        /// Gets or Sets if expandable group header is collapsed or expanded on initialization.
        /// </summary>
        public bool IsInitiallyExpanded
        {
            get => _isInitiallyExpanded;
            set
            {
                _isInitiallyExpanded = value;
            }

        }

        internal bool IsParentExpandable
        {
            get
            {
                if (Parent?.Expandable ?? false)
                    return true;
                else
                    return Parent?.IsParentExpandable ?? false;
            }
        }

        internal bool IsThisOrParentExpandable
        {
            get
            {
                if (Expandable)
                    return Expandable;
                else
                    return Parent?.IsThisOrParentExpandable ?? false;
            }
        }

        internal int Level
        {
            get
            {
                if (Parent == null)
                    return 1;
                else
                    return Parent.Level + 1;
            }
        }

        private TableContext<T> _context;
        internal TableContext<T> Context
        {
            get => _context;
            set
            {
                _context = value;
                if (_innerGroup != null)
                {
                    _innerGroup.Context = _context;
                }

            }
        }

    }
}
