﻿using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudTreeViewItem<T> : MudComponentBase
    {
        private string _text;
        private bool _disabled;
        private bool _isChecked, _isSelected, _isServerLoaded;
        private Converter<T> _converter = new DefaultConverter<T>();
        private readonly List<MudTreeViewItem<T>> _childItems = new();

        protected string Classname =>
        new CssBuilder("mud-treeview-item")
          .AddClass(Class)
        .Build();

        protected string ContentClassname =>
        new CssBuilder("mud-treeview-item-content")
          .AddClass("cursor-pointer", MudTreeRoot?.IsSelectable == true || MudTreeRoot?.ExpandOnClick == true && HasChild)
          .AddClass($"mud-treeview-item-selected", _isSelected)
        .Build();

        public string TextClassname =>
        new CssBuilder("mud-treeview-item-label")
            .AddClass(TextClass)
        .Build();


        [CascadingParameter] MudTreeView<T> MudTreeRoot { get; set; }

        [CascadingParameter] MudTreeViewItem<T> Parent { get; set; }

        /// <summary>
        /// Custom checked icon, leave null for default.
        /// </summary>
        [Parameter] public string CheckedIcon { get; set; } = Icons.Material.Filled.CheckBox;

        /// <summary>
        /// Custom unchecked icon, leave null for default.
        /// </summary>
        [Parameter] public string UncheckedIcon { get; set; } = Icons.Material.Filled.CheckBoxOutlineBlank;

        /// <summary>
        /// Value of the treeviewitem. Acts as the displayed text if no text is set.
        /// </summary>
        [Parameter] public T Value { get; set; }

        [Parameter] public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;

        /// <summary>
        /// The text to display
        /// </summary>
        [Parameter]
        public string Text
        {
            get => string.IsNullOrEmpty(_text) ? _converter.Set(Value) : _text;
            set => _text = value;
        }

        /// <summary>
        /// Tyopography for the text.
        /// </summary>
        [Parameter] public Typo TextTypo { get; set; } = Typo.body1;

        /// <summary>
        /// User class names for the text, separated by space.
        /// </summary>
        [Parameter] public string TextClass { get; set; }

        /// <summary>
        /// The text at the end of the item.
        /// </summary>
        [Parameter] public string EndText { get; set; }

        /// <summary>
        /// Tyopography for the endtext.
        /// </summary>
        [Parameter] public Typo EndTextTypo { get; set; } = Typo.body1;

        /// <summary>
        /// User class names for the endtext, separated by space.
        /// </summary>
        [Parameter] public string EndTextClass { get; set; }

        /// <summary>
        /// If true, treeviewitem will be disabled.
        /// </summary>
        [Parameter]
        public bool Disabled
        {
            get => _disabled || (MudTreeRoot?.Disabled ?? false);
            set => _disabled = value;
        }

        /// <summary>
        /// Child content of component used to create sub levels.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Content of the item, if used completly replaced the default rendering.
        /// </summary>
        [Parameter] public RenderFragment Content { get; set; }

        [Parameter] public HashSet<T> Items { get; set; }

        /// <summary>
        /// Command executed when the user clicks on the CommitEdit Button.
        /// </summary>
        [Parameter] public ICommand Command { get; set; }

        /// <summary>
        /// Expand or collapse treeview item when it has children. Two-way bindable. Note: if you directly set this to
        /// true or false (instead of using two-way binding) it will force the item's expansion state.
        /// </summary>
        [Parameter] public bool Expanded { get; set; }

        /// <summary>
        /// Called whenever expanded changed.
        /// </summary>
        [Parameter] public EventCallback<bool> ExpandedChanged { get; set; }

        [Parameter]
        public bool Activated
        {
            get => _isSelected;
            set
            {
                _ = MudTreeRoot?.UpdateSelected(this, value);
            }
        }

        [Parameter]
        public bool Selected
        {
            get => _isChecked;
            set
            {
                if (_isChecked == value)
                    return;

                _isChecked = value;
                MudTreeRoot?.UpdateSelectedItems();
                SelectedChanged.InvokeAsync(_isChecked);
            }
        }

        /// <summary>
        /// Icon placed before the text if set.
        /// </summary>
        [Parameter] public string Icon { get; set; }

        /// <summary>
        /// The color of the icon. It supports the theme colors.
        /// </summary>
        [Parameter] public Color IconColor { get; set; } = Color.Default;

        /// <summary>
        /// Icon placed after the text if set.
        /// </summary>
        [Parameter] public string EndIcon { get; set; }

        /// <summary>
        /// The color of the icon. It supports the theme colors.
        /// </summary>
        [Parameter] public Color EndIconColor { get; set; } = Color.Default;

        /// <summary>
        /// The expand/collapse icon.
        /// </summary>
        [Parameter] public string ExpandedIcon { get; set; } = Icons.Material.Filled.ChevronRight;

        /// <summary>
        /// The color of the expand/collapse button. It supports the theme colors.
        /// </summary>
        [Parameter] public Color ExpandedIconColor { get; set; } = Color.Default;

        /// <summary>
        /// The loading icon.
        /// </summary>
        [Parameter] public string LoadingIcon { get; set; } = Icons.Material.Filled.Loop;

        /// <summary>
        /// The color of the loading. It supports the theme colors.
        /// </summary>
        [Parameter] public Color LoadingIconColor { get; set; } = Color.Default;

        /// <summary>
        /// Called whenever the activated value changed.
        /// </summary>
        [Parameter] public EventCallback<bool> ActivatedChanged { get; set; }

        /// <summary>
        /// Called whenever the selected value changed.
        /// </summary>
        [Parameter] public EventCallback<bool> SelectedChanged { get; set; }

        /// <summary>
        /// Tree item click event.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

        public bool Loading { get; set; }

        bool HasChild => ChildContent != null ||
            (MudTreeRoot != null && Items != null && Items.Count != 0) ||
            (MudTreeRoot?.ServerData != null && !_isServerLoaded && (Items == null || Items.Count == 0));

        protected bool IsChecked
        {
            get => Selected;
            set { _ = SelectItem(value, this); }
        }

        protected bool ArrowExpanded
        {
            get => Expanded;
            set
            {
                if (value == Expanded)
                    return;

                Expanded = value;
                ExpandedChanged.InvokeAsync(value);
            }
        }

        protected override void OnInitialized()
        {
            if (Parent != null)
            {
                Parent.AddChild(this);
            }
            else
            {
                MudTreeRoot?.AddChild(this);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && _isSelected)
            {
                await MudTreeRoot.UpdateSelected(this, _isSelected);
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        protected async Task OnItemClicked(MouseEventArgs ev)
        {
            if (MudTreeRoot?.IsSelectable ?? false)
            {
                await MudTreeRoot.UpdateSelected(this, !_isSelected);
            }

            if (HasChild && (MudTreeRoot?.ExpandOnClick ?? false))
            {
                Expanded = !Expanded;
                TryInvokeServerLoadFunc();
                await ExpandedChanged.InvokeAsync(Expanded);
            }

            await OnClick.InvokeAsync(ev);
            if (Command?.CanExecute(Value) ?? false)
            {
                Command.Execute(Value);
            }
        }

        protected Task OnItemExpanded(bool expanded)
        {
            if (Expanded == expanded)
                return Task.CompletedTask;

            Expanded = expanded;
            TryInvokeServerLoadFunc();
            return ExpandedChanged.InvokeAsync(expanded);
        }

        internal Task Select(bool value)
        {
            if (_isSelected == value)
                return Task.CompletedTask;

            _isSelected = value;

            StateHasChanged();

            return ActivatedChanged.InvokeAsync(_isSelected);
        }

        internal async Task SelectItem(bool value, MudTreeViewItem<T> source = null)
        {
            if (value == _isChecked)
                return;

            _isChecked = value;
            _childItems.ForEach(async c => await c.SelectItem(value, source));

            StateHasChanged();

            await SelectedChanged.InvokeAsync(_isChecked);

            if (source == this)
            {
                if (MudTreeRoot != null)
                {
                    await MudTreeRoot.UpdateSelectedItems();
                }
            }
        }

        private void AddChild(MudTreeViewItem<T> item) => _childItems.Add(item);

        internal IEnumerable<MudTreeViewItem<T>> GetSelectedItems()
        {
            if (_isChecked)
                yield return this;

            foreach (var treeItem in _childItems)
            {
                foreach (var selected in treeItem.GetSelectedItems())
                {
                    yield return selected;
                }
            }
        }

        internal async void TryInvokeServerLoadFunc()
        {
            if (Expanded && (Items == null || Items.Count == 0) && MudTreeRoot?.ServerData != null)
            {
                Loading = true;
                StateHasChanged();

                Items = await MudTreeRoot.ServerData(Value);

                Loading = false;
                _isServerLoaded = true;

                StateHasChanged();
            }
        }
    }
}
