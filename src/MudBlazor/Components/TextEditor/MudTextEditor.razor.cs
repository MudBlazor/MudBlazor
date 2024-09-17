using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Interop;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudTextEditor : MudBaseInput<string>
    {
        [Inject] protected IJSRuntime jsRuntime { get; set; } = default!;
        [Parameter]
        public RenderFragment EditorContent { get; set; }

        [Parameter]
        public RenderFragment ToolbarContent { get; set; }

        //[Parameter]
        //public bool ReadOnly { get; set; }
        //    = false;

        //[Parameter]
        //public string Placeholder { get; set; }
        //    = "Compose an epic...";

        protected string Classname =>
           new CssBuilder("mud-input-control mud-input-input-control")
               .AddClass($"mud-input-{Variant.ToDescriptionString()}-with-label", !string.IsNullOrEmpty(Label))
               .AddClass(Class)
               .Build();

        protected string InputClassname =>
           new CssBuilder(
               MudInputCssHelper.GetClassname(this,
                   () =>  !string.IsNullOrEmpty(Text) || Adornment == Adornment.Start || !string.IsNullOrWhiteSpace(Placeholder) || ShrinkLabel))
            .Build();


        [Parameter]
        public string Theme { get; set; }
            = "snow";

        [Parameter]
        public string[] Formats { get; set; }
            = null;

        [Parameter]
        public string DebugLevel { get; set; }
            = "info";

        /// <summary>
        /// Support for normal css classes
        /// </summary>
        [Parameter]
        public string EditorCssClass { get; set; }
            = string.Empty;

        /// <summary>
        /// Support for normal css styles
        /// </summary>
        [Parameter]
        public string EditorCssStyle { get; set; }
            = string.Empty;

        /// <summary>
        /// Support for normal css classes
        /// </summary>
        [Parameter]
        public string ToolbarCSSClass { get; set; }
            = string.Empty;

        /// <summary>
        /// Support for normal css styles
        /// </summary>
        [Parameter]
        public string ToolbarCssStyle { get; set; }
            = string.Empty;

        [Parameter]
        public bool BottomToolbar { get; set; }

        private ElementReference QuillElement;
        private ElementReference ToolBar;
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await TextEditorInterop.CreateQuill(
                    jsRuntime,
                    QuillElement,
                    ToolBar,
                    ReadOnly,
                    Placeholder,
                    Theme,
                    Formats,
                    DebugLevel);
            }
        }

        public async Task<string> GetText()
        {
            return await TextEditorInterop.GetText(
                jsRuntime, QuillElement);
        }

        public async Task<string> GetHTML()
        {
            return await TextEditorInterop.GetHTML(
                jsRuntime, QuillElement);
        }

        public async Task<string> GetContent()
        {
            return await TextEditorInterop.GetContent(
                jsRuntime, QuillElement);
        }

        public async Task LoadContent(string Content)
        {
            var QuillDelta =
                await TextEditorInterop.LoadQuillContent(
                    jsRuntime, QuillElement, Content);
        }

        public async Task LoadHTMLContent(string quillHTMLContent)
        {
            var QuillDelta =
                await TextEditorInterop.LoadQuillHTMLContent(
                    jsRuntime, QuillElement, quillHTMLContent);
        }

        public async Task InsertImage(string ImageURL)
        {
            var QuillDelta =
                await TextEditorInterop.InsertQuillImage(
                    jsRuntime, QuillElement, ImageURL);
        }

        public async Task InsertText(string text)
        {
            var QuillDelta =
                await TextEditorInterop.InsertQuillText(
                    jsRuntime, QuillElement, text);
        }

        public async Task EnableEditor(bool mode)
        {
            var QuillDelta =
                await TextEditorInterop.EnableQuillEditor(
                    jsRuntime, QuillElement, mode);
        }

    }
}
