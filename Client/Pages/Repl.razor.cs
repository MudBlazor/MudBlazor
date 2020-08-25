namespace BlazorRepl.Client.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BlazorRepl.Client.Components;
    using BlazorRepl.Client.Components.Models;
    using BlazorRepl.Client.Services;
    using BlazorRepl.Core;
    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    public partial class Repl : IDisposable
    {
        private const string BasicUserComponentCodePrefix =
    @"@page ""/__main""
@using System.ComponentModel.DataAnnotations
@using System.Linq
@using System.Net.Http
@using System.Net.Http.Json
@using Microsoft.AspNetCore.Components.Forms
@using Microsoft.AspNetCore.Components.Routing
@using Microsoft.AspNetCore.Components.Web
@using Microsoft.JSInterop
";

        private DotNetObjectReference<Repl> dotNetInstance;
        private string errorMessage;

        [Inject]
        public SnippetsService SnippetsService { get; set; }

        [Inject]
        public ComponentCompilationService CompilationService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [CascadingParameter]
        public PageNotifications PageNotificationsComponent { get; set; }

        [Parameter]
        public string SnippetId { get; set; }

        public CodeEditor CodeEditorComponent { get; set; }

        public ICollection<ComponentFile> ComponentFiles { get; set; } = new List<ComponentFile>();

        public IList<string> ComponentFileNames => this.ComponentFiles?.Select(cf => cf.Name).ToList() ?? new List<string>();

        public string CodeEditorContent { get; set; }

        public bool SaveSnippetPopupVisible { get; set; }

        public string Preset { get; set; } = "basic";

        public string UserComponentCodePrefix => BasicUserComponentCodePrefix;

        public IReadOnlyCollection<CompilationDiagnostic> Diagnostics { get; set; } = Array.Empty<CompilationDiagnostic>();

        public bool AreDiagnosticsShown { get; set; }

        public string LoaderText { get; set; }

        public bool Loading { get; set; }

        public int UserComponentCodeStartLine => this.UserComponentCodePrefix.Count(ch => ch == '\n');

        public void HandleTabActivate(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            var componentFile = this.ComponentFiles.FirstOrDefault(cf => cf.Name == name);
            if (componentFile == null)
            {
                return;
            }

            this.CodeEditorContent = componentFile.Content;
        }

        public void HandleTabClose(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            var componentFile = this.ComponentFiles.FirstOrDefault(cf => cf.Name == name);
            if (componentFile == null)
            {
                return;
            }

            this.ComponentFiles.Remove(componentFile);
        }

        public void HandleTabCreate(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            this.ComponentFiles.Add(new ComponentFile { Name = name, Content = string.Empty });
        }

        public async Task CompileAsync()
        {
            this.Loading = true;
            this.LoaderText = "Processing";

            await Task.Delay(10); // Ensure rendering has time to be called

            CompileToAssemblyResult result = null;
            try
            {
                var code = await this.CodeEditorComponent.GetCodeAsync();

                result = await this.CompilationService.CompileToAssembly(
                    new[]
                    {
                        new ComponentFile { Name = "__Main.razor", Content = this.UserComponentCodePrefix + code },
//                        new ComponentFile
//                        {
//                            Name = "UserPage2.razor",
//                            Content = @"
//@using System.ComponentModel.DataAnnotations
//@using System.Linq
//@using System.Net.Http
//@using System.Net.Http.Json
//@using Microsoft.AspNetCore.Components.Forms
//@using Microsoft.AspNetCore.Components.Routing
//@using Microsoft.AspNetCore.Components.Web
//@using Microsoft.JSInterop

//<h1>Counter Demo</h1>

//<button class=""btn btn-primary"" @onclick=""@Increment"">Increment</button>

//<div>Counter: @Counter</div>

//@code {
//    public int Counter { get; set; }

//    public void Increment()
//    {
//        if (Counter < 10)
//        {
//            Counter++;
//        }
//        else if (Counter < 100) 
//        {
//            Counter += 10;
//        }
//        else 
//        {
//            Counter += 100;
//        }
//    }
//}",

//                        },
                    },
                    this.Preset,
                    this.UpdateLoaderTextAsync);

                this.Diagnostics = result.Diagnostics.OrderByDescending(x => x.Severity).ThenBy(x => x.Code).ToList();
                this.AreDiagnosticsShown = true;
            }
            catch (Exception)
            {
                this.PageNotificationsComponent.AddNotification(NotificationType.Error, content: "Error while compiling the code.");
            }
            finally
            {
                this.Loading = false;
            }

            if (result?.AssemblyBytes?.Length > 0)
            {
                await this.JsRuntime.InvokeVoidAsync("App.Repl.updateUserAssemblyInCacheStorage", result.AssemblyBytes);

                // TODO: Add error page in iframe
                await this.JsRuntime.InvokeVoidAsync("App.reloadIFrame", "user-page-window");
            }
        }

        public void ShowSaveSnippetPopup() => this.SaveSnippetPopupVisible = true;

        [JSInvokable]
        public async Task TriggerCompileAsync()
        {
            await this.CompileAsync();

            this.StateHasChanged();
        }

        public void Dispose()
        {
            this.dotNetInstance?.Dispose();

            _ = this.JsRuntime.InvokeVoidAsync("App.Repl.dispose");
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                this.dotNetInstance = DotNetObjectReference.Create(this);

                await this.JsRuntime.InvokeVoidAsync(
                    "App.Repl.init",
                    "user-code-editor-container",
                    "user-page-window-container",
                    "user-code-editor",
                    this.dotNetInstance);
            }

            if (!string.IsNullOrWhiteSpace(this.errorMessage) && this.PageNotificationsComponent != null)
            {
                this.PageNotificationsComponent.AddNotification(NotificationType.Error, content: this.errorMessage);

                this.errorMessage = null;
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        protected override async Task OnInitializedAsync()
        {
            this.PageNotificationsComponent?.Clear();

            this.ComponentFiles = new List<ComponentFile>
            {
                new ComponentFile { Name = "__Main.razor", Content = "<h1> Some Test Content</h1>" },
                new ComponentFile { Name = "File2.razor", Content = "<h1> Some Test Content 2</h1>" },
                new ComponentFile { Name = "File3.razor", Content = "<h1> Some Test Content 3</h1>" },
                new ComponentFile { Name = "File4.razor", Content = "<h1> Some Test Content 4</h1>" },
                new ComponentFile { Name = "File5.razor", Content = "<h1> Some Test Content 5</h1>" },
                new ComponentFile { Name = "File6.razor", Content = "<h1> Some Test Content 6</h1>" },
            };

            if (!string.IsNullOrWhiteSpace(this.SnippetId))
            {
                try
                {
                    this.ComponentFiles = (await this.SnippetsService.GetSnippetContentAsync(this.SnippetId)).ToList();
                }
                catch (ArgumentException)
                {
                    this.errorMessage = "Invalid Snippet ID.";
                }
                catch (Exception)
                {
                    this.errorMessage = "Unable to get snippet content. Please try again later.";
                }
            }

            //const string DefaultUserPageAssemblyBytes =
            //    "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AAAAABglAAAAAAAAAAAAAAAAX0NvckRsbE1haW4AbXNjb3JlZS5kbGwAAAAAAP8lACAAEAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAIAAADAAAADg1AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==";

            //await this.JsRuntime.InvokeVoidAsync(
            //    "App.Repl.updateUserAssemblyInCacheStorage",
            //    Convert.FromBase64String(DefaultUserPageAssemblyBytes));

            await base.OnInitializedAsync();
        }

        private Task UpdateLoaderTextAsync(string loaderText)
        {
            this.LoaderText = loaderText;

            this.StateHasChanged();

            return Task.Delay(10); // Ensure rendering has time to be called
        }
    }
}
