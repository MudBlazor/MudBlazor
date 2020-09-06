namespace BlazorRepl.Client.Pages
{
    using System;
    using System.Collections.Generic;
    using System.IO;
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
        private const string MainComponentCodePrefix = "@page \"/__main\"\n";
        private const string MainUserPagePath = "/__main";

        private DotNetObjectReference<Repl> dotNetInstance;
        private string errorMessage;
        private CodeFile activeCodeFile;

        [Inject]
        public SnippetsService SnippetsService { get; set; }

        [Inject]
        public CompilationService CompilationService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [CascadingParameter]
        public PageNotifications PageNotificationsComponent { get; set; }

        [Parameter]
        public string SnippetId { get; set; }

        public CodeEditor CodeEditorComponent { get; set; }

        public IDictionary<string, CodeFile> CodeFiles { get; set; } = new Dictionary<string, CodeFile>();

        public IList<string> CodeFileNames => this.CodeFiles.Keys.ToList();

        public string CodeEditorContent => this.activeCodeFile?.Content;

        public bool SaveSnippetPopupVisible { get; set; }

        public string Preset { get; set; } = "basic";

        public IReadOnlyCollection<CompilationDiagnostic> Diagnostics { get; set; } = Array.Empty<CompilationDiagnostic>();

        public bool AreDiagnosticsShown { get; set; }

        public string LoaderText { get; set; }

        public bool Loading { get; set; }

        public async Task CompileAsync()
        {
            this.Loading = true;
            this.LoaderText = "Processing";

            await Task.Delay(10); // Ensure rendering has time to be called

            CompileToAssemblyResult compilationResult = null;
            CodeFile mainComponent = null;
            string originalMainComponentContent = null;
            try
            {
                await this.UpdateActiveCodeFileContentAsync();

                // Add the necessary main component code prefix and store the original content so we can revert right after compilation.
                if (this.CodeFiles.TryGetValue(CoreConstants.MainComponentFilePath, out mainComponent))
                {
                    originalMainComponentContent = mainComponent.Content;
                    mainComponent.Content = MainComponentCodePrefix + originalMainComponentContent;
                }

                compilationResult = await this.CompilationService.CompileToAssembly(
                    this.CodeFiles.Values,
                    this.Preset,
                    this.UpdateLoaderTextAsync);

                this.Diagnostics = compilationResult.Diagnostics.OrderByDescending(x => x.Severity).ThenBy(x => x.Code).ToList();
                this.AreDiagnosticsShown = true;
            }
            catch (Exception)
            {
                this.PageNotificationsComponent.AddNotification(NotificationType.Error, content: "Error while compiling the code.");
            }
            finally
            {
                if (mainComponent != null)
                {
                    mainComponent.Content = originalMainComponentContent;
                }

                this.Loading = false;
            }

            if (compilationResult?.AssemblyBytes?.Length > 0)
            {
                await this.JsRuntime.InvokeVoidAsync("App.Repl.updateUserAssemblyInCacheStorage", compilationResult.AssemblyBytes);

                // TODO: Add error page in iframe
                await this.JsRuntime.InvokeVoidAsync("App.reloadIFrame", "user-page-window", MainUserPagePath);
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

            if (!string.IsNullOrWhiteSpace(this.SnippetId))
            {
                try
                {
                    this.CodeFiles = (await this.SnippetsService.GetSnippetContentAsync(this.SnippetId)).ToDictionary(f => f.Path, f => f);
                    if (!this.CodeFiles.Any())
                    {
                        this.errorMessage = "No files in snippet.";
                    }
                    else
                    {
                        this.activeCodeFile = this.CodeFiles.First().Value;
                    }
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

            if (!this.CodeFiles.Any())
            {
                this.activeCodeFile = new CodeFile
                {
                    Path = CoreConstants.MainComponentFilePath,
                    Content = CoreConstants.MainComponentDefaultFileContent,
                };
                this.CodeFiles.Add(CoreConstants.MainComponentFilePath, this.activeCodeFile);
            }

            await this.JsRuntime.InvokeVoidAsync(
                "App.Repl.updateUserAssemblyInCacheStorage",
                Convert.FromBase64String(CoreConstants.DefaultUserComponentsAssemblyBytes));

            await base.OnInitializedAsync();
        }

        private async Task HandleTabActivateAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            await this.UpdateActiveCodeFileContentAsync();

            if (this.CodeFiles.TryGetValue(name, out var codeFile))
            {
                this.activeCodeFile = codeFile;

                await this.CodeEditorComponent.FocusAsync();
            }
        }

        private void HandleTabClose(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            this.CodeFiles.Remove(name);
        }

        private void HandleTabCreate(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            var nameWithoutExtension = Path.GetFileNameWithoutExtension(name);

            this.CodeFiles.TryAdd(name, new CodeFile { Path = name, Content = $"<h1>{nameWithoutExtension}</h1>" });
        }

        private async Task UpdateActiveCodeFileContentAsync()
        {
            if (this.activeCodeFile == null)
            {
                this.PageNotificationsComponent.AddNotification(NotificationType.Error, "No active file to update.");
                return;
            }

            this.activeCodeFile.Content = await this.CodeEditorComponent.GetCodeAsync();
        }

        private Task UpdateLoaderTextAsync(string loaderText)
        {
            this.LoaderText = loaderText;

            this.StateHasChanged();

            return Task.Delay(10); // Ensure rendering has time to be called
        }
    }
}
