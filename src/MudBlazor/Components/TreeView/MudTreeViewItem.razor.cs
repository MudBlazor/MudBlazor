using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudTreeViewItem : MudComponentBase
    {
        private bool _isSelected, _isActivated, _isExpanded;
        private readonly List<MudTreeViewItem> childItems = new List<MudTreeViewItem>();

        protected string Classname =>
        new CssBuilder("mud-treeview-item")
          .AddClass(Class)
        .Build();

        protected string ContentClassname =>
        new CssBuilder("mud-treeview-item-content")
          .AddClass("mud-treeview-item-activated", Activated && MudTreeRoot.CanActivate)
        .Build();

        public string TextClassname =>
        new CssBuilder("mud-treeview-item-label")
            .AddClass(TextClass)
        .Build();

        [Parameter] public string Text { get; set; }

        [Parameter] public Typo TextTypo { get; set; } = Typo.body1;

        [Parameter] public string TextClass { get; set; }

        [Parameter] public string EndText { get; set; }

        [Parameter] public Typo EndTextTypo { get; set; } = Typo.body1;

        [Parameter] public string EndTextClass { get; set; }

        [CascadingParameter] MudTreeView MudTreeRoot { get; set; }

        [CascadingParameter] MudTreeViewItem Parent { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public RenderFragment Content { get; set; }

        [Parameter] public IEnumerable<object> Items { get; set; }

        [Parameter]
        public bool Expanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded == value)
                    return;

                _isExpanded = value;
                ExpandedChanged.InvokeAsync(_isExpanded);
            }
        }

        [Parameter]
        public bool Activated
        {
            get => _isActivated;
            set
            {
                _ = MudTreeRoot?.UpdateActivatedItem(this, value);
            }
        }

        [Parameter]
        public bool Selected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value)
                    return;

                _isSelected = value;
                MudTreeRoot?.UpdateSelectedItems();
                SelectedChanged.InvokeAsync(_isSelected);
            }
        }

        [Parameter]
        public string Icon { get; set; }

        [Parameter]
        public Color IconColor { get; set; } = Color.Default;

        [Parameter]
        public string EndIcon { get; set; }

        [Parameter]
        public Color EndIconColor { get; set; } = Color.Default;

        [Parameter]
        public EventCallback<bool> ActivatedChanged { get; set; }

        [Parameter]
        public EventCallback<bool> ExpandedChanged { get; set; }

        [Parameter]
        public EventCallback<bool> SelectedChanged { get; set; }

        /// <summary>
        /// Tree item click event.
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        bool HasChild => ChildContent != null || (MudTreeRoot != null && Items != null && Items.Count() != 0);

        protected bool IsChecked
        {
            get => Selected;
            set { _ = Select(value, this); }
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

        protected async Task OnItemClicked(MouseEventArgs ev)
        {
            if (MudTreeRoot?.CanActivate ?? false)
            {
                await MudTreeRoot.UpdateActivatedItem(this, !_isActivated);
            }

            if (HasChild && (MudTreeRoot?.ExpandOnClick ?? false))
            {
                _isExpanded = !_isExpanded;
                await ExpandedChanged.InvokeAsync(_isExpanded);
            }

            await OnClick.InvokeAsync(ev);
        }

        internal async Task Activate(bool value)
        {
            if (_isActivated == value)
                return;

            _isActivated = value;

            StateHasChanged();

            await ActivatedChanged.InvokeAsync(_isActivated);
        }

        internal async Task Select(bool value, MudTreeViewItem source = null)
        {
            if (value == _isSelected)
                return;

            _isSelected = value;
            childItems.ForEach(async c => await c.Select(value, source));

            StateHasChanged();

            await SelectedChanged.InvokeAsync(_isSelected);

            if (source == this)
            {
                await MudTreeRoot?.UpdateSelectedItems();
            }
        }

        private void AddChild(MudTreeViewItem item) => childItems.Add(item);

        internal IEnumerable<MudTreeViewItem> GetSelectedItems()
        {
            if (_isSelected)
                yield return this;

            foreach (var treeItem in childItems)
            {
                foreach (var selected in treeItem.GetSelectedItems())
                {
                    yield return selected;
                }
            }
        }
    }
}
