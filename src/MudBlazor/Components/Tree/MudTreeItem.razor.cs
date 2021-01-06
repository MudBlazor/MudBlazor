using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MudBlazor
{
    public partial class MudTreeItem : MudComponentBase
    {
        private bool _selected;
        private List<MudTreeItem> childItems = new List<MudTreeItem>();

        protected string Classname =>
        new CssBuilder("mud-tree-item")
          .AddClass(Class)
        .Build();

        protected string ContentClassname =>
        new CssBuilder("mud-tree-item-content")
          .AddClass("mud-tree-item-activated", Activated && MudTreeRoot.CanActivate)
        .Build();

        public string TextClassname =>
        new CssBuilder("mud-tree-item-label")
            .AddClass(TextClass)
        .Build();
        
        [Parameter] public string Text { get; set; }

        [Parameter] public Typo TextTypo { get; set; } = Typo.body1;

        [Parameter] public string TextClass { get; set; }

        [Parameter] public string EndText { get; set; }

        [Parameter] public Typo EndTextTypo { get; set; } = Typo.body1;

        [Parameter] public string EndTextClass { get; set; }

        [CascadingParameter] MudTree MudTreeRoot { get; set; }

        [CascadingParameter] MudTreeItem Parent { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public RenderFragment Content { get; set; }

        [Parameter] public RenderFragment IconContent { get; set; }
        
        [Parameter]
        public bool Expanded { get; set; } = false;

        [Parameter]
        public bool Activated { get; set; } = false;

        [Parameter]
        public string Icon { get; set; }

        [Parameter]
        public Color IconColor { get; set; } = Color.Default;

        [Parameter]
        public EventCallback<bool> ActivatedChanged { get; set; }

        [Parameter]
        public EventCallback<bool> ExpandedChanged { get; set; }

        [Parameter]
        public bool Selected
        {
            get => _selected;
            set
            {
                if (value == _selected)
                    return;

                _selected = value;
                childItems.ForEach(c => c.Selected = value);
                SelectedChanged.InvokeAsync(_selected);
            }
        }

        [Parameter]
        public EventCallback<bool> SelectedChanged { get; set; }
        
        /// <summary>
        /// Tree item click event.
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

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

            if (ChildContent != null && MudTreeRoot.ExpandOnClick)
            {
                Expanded = !Expanded;
                await ExpandedChanged.InvokeAsync(Expanded);
            }

            await OnClick.InvokeAsync(ev);
        }

        protected async void OnExpandClick(MouseEventArgs args)
        {
            if (ChildContent != null)
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

        internal void AddChild(MudTreeItem item) => childItems.Add(item);
    }
}
