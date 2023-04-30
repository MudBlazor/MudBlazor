using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;
using MudBlazor.Interfaces;

namespace MudBlazor
{
    public partial class MudTreeViewItem<T> : MudComponentBase
    {
        private string _text;
        private bool _disabled;
        private bool _canExpand = true;
        private bool _isChecked, _isSelected, _isServerLoaded;
        private Converter<T> _converter = new DefaultConverter<T>();
        private readonly List<MudTreeViewItem<T>> _childItems = new();

        protected string Classname =>
        new CssBuilder("mud-treeview-item")
            .AddClass("mud-treeview-select-none", MudTreeRoot?.ExpandOnDoubleClick == true)
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
        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public string CheckedIcon { get; set; } = Icons.Material.Filled.CheckBox;

        /// <summary>
        /// Custom unchecked icon, leave null for default.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public string UncheckedIcon { get; set; } = Icons.Material.Filled.CheckBoxOutlineBlank;

        /// <summary>
        /// Value of the treeviewitem. Acts as the displayed text if no text is set.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Data)]
        public T Value { get; set; }

        [Parameter]
        [Category(CategoryTypes.TreeView.Behavior)]
        public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;

        /// <summary>
        /// The text to display
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Behavior)]
        public string Text
        {
            get => string.IsNullOrEmpty(_text) ? _converter.Set(Value) : _text;
            set => _text = value;
        }

        /// <summary>
        /// Tyopography for the text.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public Typo TextTypo { get; set; } = Typo.body1;

        /// <summary>
        /// User class names for the text, separated by space.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public string TextClass { get; set; }

        /// <summary>
        /// The text at the end of the item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Behavior)]
        public string EndText { get; set; }

        /// <summary>
        /// Tyopography for the endtext.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public Typo EndTextTypo { get; set; } = Typo.body1;

        /// <summary>
        /// User class names for the endtext, separated by space.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public string EndTextClass { get; set; }

        /// <summary>
        /// If true, treeviewitem will be disabled.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Behavior)]
        public bool Disabled
        {
            get => _disabled || (MudTreeRoot?.Disabled ?? false);
            set => _disabled = value;
        }

        /// <summary>
        /// If false, TreeViewItem will not be able to expand.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Behavior)]
        public bool CanExpand
        {
            get => _canExpand;
            set => _canExpand = value;
        }

        /// <summary>
        /// Child content of component used to create sub levels.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Data)]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Content of the item, if used completly replaced the default rendering.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Behavior)]
        public RenderFragment Content { get; set; }

        /// <summary>
        /// Content of the item body, if used replaced the text, end text and end icon rendering.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Behavior)]
        public RenderFragment<MudTreeViewItem<T>> BodyContent { get; set; }

        [Parameter]
        [Category(CategoryTypes.TreeView.Data)]
        public HashSet<T> Items { get; set; }

        /// <summary>
        /// Command executed when the user clicks on the CommitEdit Button.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.ClickAction)]
        [Obsolete($"Use {nameof(OnClick)} instead. This will be removed in v7.")]
        public ICommand Command { get; set; }

        /// <summary>
        /// Expand or collapse treeview item when it has children. Two-way bindable. Note: if you directly set this to
        /// true or false (instead of using two-way binding) it will force the item's expansion state.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Expanding)]
        public bool Expanded { get; set; }

        /// <summary>
        /// Called whenever expanded changed.
        /// </summary>
        [Parameter] public EventCallback<bool> ExpandedChanged { get; set; }

        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
        public bool Activated
        {
            get => _isSelected;
            set
            {
                _ = MudTreeRoot?.UpdateSelected(this, value);
            }
        }

        [Parameter]
        [Category(CategoryTypes.TreeView.Selecting)]
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
        [Parameter]
        [Category(CategoryTypes.TreeView.Behavior)]
        public string Icon { get; set; }

        /// <summary>
        /// The color of the icon. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public Color IconColor { get; set; } = Color.Default;

        /// <summary>
        /// Icon placed after the text if set.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Behavior)]
        public string EndIcon { get; set; }

        /// <summary>
        /// The color of the icon. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public Color EndIconColor { get; set; } = Color.Default;

        /// <summary>
        /// The expand/collapse icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Expanding)]
        public string ExpandedIcon { get; set; } = Icons.Material.Filled.ChevronRight;

        /// <summary>
        /// The color of the expand/collapse button. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Expanding)]
        public Color ExpandedIconColor { get; set; } = Color.Default;

        /// <summary>
        /// The loading icon.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public string LoadingIcon { get; set; } = Icons.Material.Filled.Loop;

        /// <summary>
        /// The color of the loading. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.TreeView.Appearance)]
        public Color LoadingIconColor { get; set; } = Color.Default;

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

        /// <summary>
        /// Tree item double click event.
        /// </summary>
        [Parameter] public EventCallback<MouseEventArgs> OnDoubleClick { get; set; }

        public bool Loading { get; set; }

        bool HasChild => ChildContent != null ||
             (MudTreeRoot != null && Items != null && Items.Count != 0) ||
             (MudTreeRoot?.ServerData != null && _canExpand && !_isServerLoaded && (Items == null || Items.Count == 0));

        protected bool IsChecked
        {
            get => Selected;
            set { _ = SelectItem(value, this); }
        }

        protected internal bool ArrowExpanded
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
                await TryInvokeServerLoadFunc();
                await ExpandedChanged.InvokeAsync(Expanded);
            }

            await OnClick.InvokeAsync(ev);
#pragma warning disable CS0618
            if (Command?.CanExecute(Value) ?? false)
            {
                Command.Execute(Value);
            }
#pragma warning restore CS0618
        }

        protected async Task OnItemDoubleClicked(MouseEventArgs ev)
        {
            if (MudTreeRoot?.IsSelectable ?? false)
            {
                await MudTreeRoot.UpdateSelected(this, !_isSelected);
            }

            if (HasChild && (MudTreeRoot?.ExpandOnDoubleClick ?? false))
            {
                Expanded = !Expanded;
                await TryInvokeServerLoadFunc();
                await ExpandedChanged.InvokeAsync(Expanded);
            }

            await OnDoubleClick.InvokeAsync(ev);
        }
        protected internal async Task OnItemExpanded(bool expanded)
        {
            if (Expanded != expanded) {
                Expanded = expanded;
                await TryInvokeServerLoadFunc();
                await ExpandedChanged.InvokeAsync(expanded);
            }
        }

        /// <summary>
        /// Clear the tree items, and try to reload from server.
        /// </summary>
        public async Task ReloadAsync()
        {
            if (Items != null)
            {
                Items.Clear();
            }
            await TryInvokeServerLoadFunc();

            if (Parent != null)
            {
                Parent.StateHasChanged();
            }
            else if (MudTreeRoot != null)
            {
                ((IMudStateHasChanged)MudTreeRoot).StateHasChanged();
            }
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

        internal async Task TryInvokeServerLoadFunc()
        {
            if (Expanded && (Items == null || Items.Count == 0) && _canExpand && MudTreeRoot?.ServerData != null)
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
