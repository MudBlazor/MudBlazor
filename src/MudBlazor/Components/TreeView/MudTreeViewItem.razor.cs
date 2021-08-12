using System.Collections.Generic;
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
        private bool _isSelected, _isActivated, _isServerLoaded;
        private Converter<T> _converter = new DefaultConverter<T>();
        private readonly List<MudTreeViewItem<T>> _childItems = new();

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

        [Parameter]
        public string Text
        {
            get => string.IsNullOrEmpty(_text) ? _converter.Set(Value) : _text;
            set => _text = value;
        }

        [Parameter]
        public T Value { get; set; }

        [Parameter] public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;

        [Parameter] public Typo TextTypo { get; set; } = Typo.body1;

        [Parameter] public string TextClass { get; set; }

        [Parameter] public string EndText { get; set; }

        [Parameter] public Typo EndTextTypo { get; set; } = Typo.body1;

        [Parameter] public string EndTextClass { get; set; }

        [CascadingParameter] MudTreeView<T> MudTreeRoot { get; set; }

        [CascadingParameter] MudTreeViewItem<T> Parent { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public RenderFragment Content { get; set; }

        [Parameter] public HashSet<T> Items { get; set; }

        /// <summary>
        /// Command executed when the user clicks on the CommitEdit Button.
        /// </summary>
        [Parameter] public ICommand Command { get; set; }

        [Parameter]
        public bool Expanded { get; set; }

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

        public bool Loading { get; set; }

        bool HasChild => ChildContent != null ||
            (MudTreeRoot != null && Items != null && Items.Count != 0) ||
            (MudTreeRoot?.ServerData != null && !_isServerLoaded && (Items == null || Items.Count == 0));

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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && _isActivated)
            {
                await MudTreeRoot.UpdateActivatedItem(this, _isActivated);
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        protected async Task OnItemClicked(MouseEventArgs ev)
        {
            if (MudTreeRoot?.CanActivate ?? false)
            {
                await MudTreeRoot.UpdateActivatedItem(this, !_isActivated);
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

        internal async Task Activate(bool value)
        {
            if (_isActivated == value)
                return;

            _isActivated = value;

            StateHasChanged();

            await ActivatedChanged.InvokeAsync(_isActivated);
        }

        internal async Task Select(bool value, MudTreeViewItem<T> source = null)
        {
            if (value == _isSelected)
                return;

            _isSelected = value;
            _childItems.ForEach(async c => await c.Select(value, source));

            StateHasChanged();

            await SelectedChanged.InvokeAsync(_isSelected);

            if (source == this)
            {
                await MudTreeRoot?.UpdateSelectedItems();
            }
        }

        private void AddChild(MudTreeViewItem<T> item) => _childItems.Add(item);

        internal IEnumerable<MudTreeViewItem<T>> GetSelectedItems()
        {
            if (_isSelected)
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
