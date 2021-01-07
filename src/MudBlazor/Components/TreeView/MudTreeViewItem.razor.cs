using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MudBlazor
{
    public partial class MudTreeViewItem : MudComponentBase
    {
        private List<MudTreeViewItem> childItems = new List<MudTreeViewItem>();

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

        [Parameter] public RenderFragment IconContent { get; set; }

        [Parameter] public RenderFragment EndIconContent { get; set; }

        [Parameter] public IEnumerable<object> Items { get; set; }

        [Parameter]
        public bool Expanded { get; set; }

        [Parameter]
        public bool Activated { get; set; }

        [Parameter]
        public bool Selected { get; set; }

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
            set
            {
                if (value == Selected)
                    return;

                Selected = value;
                childItems.ForEach(c => c.IsChecked = value);

                SelectedChanged.InvokeAsync(value);
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender && Parent != null)
            {
                Parent.AddChild(this);
            }
            base.OnAfterRender(firstRender);
        }
        
        protected async Task OnItemClicked(MouseEventArgs ev)
        {
            if (MudTreeRoot.CanActivate)
            {
                await Activate();
            }

            if (HasChild && MudTreeRoot.ExpandOnClick)
            {
                Expanded = !Expanded;
                await ExpandedChanged.InvokeAsync(Expanded);
            }

            await OnClick.InvokeAsync(ev);
        }

        protected async void OnExpandClick(MouseEventArgs args)
        {
            if (HasChild)
            {
                Expanded = !Expanded;
                await ExpandedChanged.InvokeAsync(Expanded);
            }
        }

        public async Task Activate()
        {
            Activated = true;

            await MudTreeRoot.UpdateActivation(this);

            StateHasChanged();

            await ActivatedChanged.InvokeAsync(Activated);
        }

        public async Task Deactivate()
        {
            Activated = false;

            StateHasChanged();

            await ActivatedChanged.InvokeAsync(Activated);
        }

        internal void AddChild(MudTreeViewItem item) => childItems.Add(item);
    }
}
