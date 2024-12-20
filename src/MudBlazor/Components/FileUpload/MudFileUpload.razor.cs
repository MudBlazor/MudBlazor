// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// A form component for uploading one or more files.  For <c>T</c>, use either <c>IBrowserFile</c> for a single file or <c>IReadOnlyList&lt;IBrowserFile&gt;</c> for multiple files.
    /// </summary>
    /// <typeparam name="T">Either <see cref="IBrowserFile"/> for a single file or <see cref="IReadOnlyList{IBrowserFile}">IReadOnlyList&lt;IBrowserFile&gt;</see> for multiple files.</typeparam>
    public partial class MudFileUpload<T> : MudFormComponent<T, string>
    {
        private readonly ParameterState<T?> _filesState;
        private readonly ParameterState<bool> _draggingState;

        [Inject]
        private IJSRuntime JsRuntime { get; set; } = null!;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public MudFileUpload() : base(new DefaultConverter<T>())
        {
            using var registerScope = CreateRegisterScope();
            _filesState = registerScope.RegisterParameter<T?>(nameof(Files))
                .WithParameter(() => Files)
                .WithEventCallback(() => FilesChanged);
            _draggingState = registerScope.RegisterParameter<bool>(nameof(Dragging))
                .WithParameter(() => Dragging)
                .WithEventCallback(() => DraggingChanged);
        }

        private readonly string _id = Identifier.Create();

        protected string Classname =>
            new CssBuilder("mud-file-upload")
                .AddClass(Class)
                .Build();

        protected string DragClass =>
            new CssBuilder("mud-file-upload-dragarea")
                .AddClass("relative d-flex rounded-lg border-2 border-dashed pa-4 mud-width-full mud-height-full justify-center align-center flex-column")
                .AddClass("mud-border-primary", _draggingState.Value)
                .Build();

        protected string InputClasses =>
            new CssBuilder(InputClass)
                .AddClass("mud-file-upload-draggover", DragAndDrop)
                .Build();

        /// <summary>
        /// The uploaded file or files.
        /// </summary>
        /// <remarks>
        /// When <c>T</c> is <see cref="IBrowserFile" />, a single file is returned.<br />
        /// When <c>T</c> is <see cref="IReadOnlyList{IBrowserFile}">IReadOnlyList&lt;IBrowserFile&gt;</see>, multiple files are returned.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public T? Files { get; set; }

        /// <summary>
        /// Occurs when <see cref="Files"/> has changed.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public EventCallback<T?> FilesChanged { get; set; }

        /// <summary>
        /// Occurs when <see cref="Dragging"/> has changed.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public EventCallback<bool> DraggingChanged { get; set; }

        /// <summary>
        /// Occurs when the internal files have changed.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public EventCallback<InputFileChangeEventArgs> OnFilesChanged { get; set; }

        /// <summary>
        /// Appends additional files to the existing list.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>. This applies when <c>T</c> is <see cref="IReadOnlyList{IBrowserFile}">IReadOnlyList&lt;IBrowserFile&gt;</see>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public bool AppendMultipleFiles { get; set; }

        /// <summary>
        /// The custom content which includes Context to Open the picker.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public RenderFragment<MudFileUpload<T>>? ActivatorContent { get; set; }

        /// <summary>
        /// The template used for selected files.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Appearance)]
        public RenderFragment<T?>? SelectedTemplate { get; set; }

        /// <summary>
        /// Prevents raising <see cref="OnFilesChanged"/> if validation fails during an upload.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public bool SuppressOnChangeWhenInvalid { get; set; }

        /// <summary>
        /// The accepted file extensions, separated by commas.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c> for any file type.  Multiple file extensions must be separated by commas (e.g. <c>".png, .jpg"</c>).
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public string? Accept { get; set; }

        /// <summary>
        /// Hides the inner <see cref="InputFile"/> component.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.  When <c>false</c>, files can be uploaded via drag-and-drop.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Appearance)]
        public bool Hidden { get; set; } = true;

        /// <summary>
        /// Enables a drag and drop zone inside the MudFileUpload        
        /// </summary>
        /// <remarks>
        /// Defaults to false
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public bool DragAndDrop { get; set; }

        /// <summary>
        /// Enables the file input to be visible for the ondrop event.
        /// </summary>
        /// <remarks>
        /// Once the input is visible, ondrop, ondragleave, ondragend will turn Dragging to false, hiding the input again.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public bool Dragging { get; set; }

        /// <summary>
        /// The CSS classes applied to the internal <see cref="InputFile"/>.
        /// </summary>
        /// <remarks>
        /// These styles apply when <see cref="Hidden"/> is <c>false</c>. Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Appearance)]
        public string? InputClass { get; set; }

        /// <summary>
        /// The CSS styles applied to the internal <see cref="InputFile"/>.
        /// </summary>
        /// <remarks>
        /// These styles apply when <see cref="Hidden"/> is <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Appearance)]
        public string? InputStyle { get; set; }

        /// <summary>
        /// The CSS styles applied to the internal <see cref="MudPaper"/> drag and drop area.
        /// </summary>
        /// <remarks>
        /// These styles apply when <see cref="DragAndDrop"/> is <c>true</c> and no custom ActivatorContent has been created.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Appearance)]
        public string? DragStyle { get; set; }

        /// <summary>
        /// The maximum number of files retrieved during a call to <see cref="InputFileChangeEventArgs.GetMultipleFiles(int)"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>10</c>.  This property does not limit the total number of uploaded files allowed; a limit should be validated manually, such as during the <see cref="FilesChanged"/> event.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public int MaximumFileCount { get; set; } = 10;

        /// <summary>
        /// Prevents the user from uploading files.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public bool Disabled { get; set; }

        [CascadingParameter(Name = "ParentDisabled")]
        private bool ParentDisabled { get; set; }

        [CascadingParameter(Name = "ParentReadOnly")]
        private bool ParentReadOnly { get; set; }

        /// <summary>
        /// The uploaded file or filenames.
        /// </summary>
        /// <remarks>
        /// When <c>T</c> is <see cref="IBrowserFile" />, a single filename is returned.<br />
        /// When <c>T</c> is <see cref="IReadOnlyList{IBrowserFile}">IReadOnlyList&lt;IBrowserFile&gt;</see>, multiple filenames are returned.
        /// </remarks>
        public IReadOnlyList<string> GetFilenames()
        {
            if (EqualityComparer<T>.Default.Equals(_filesState.Value, default))
            {
                return [];
            }
            return _filesState.Value switch
            {
                IBrowserFile singleFile => [singleFile.Name],
                IReadOnlyList<IBrowserFile> fileList => fileList.Select(f => f.Name).ToList(),
                _ => []
            };
        }

        protected bool GetDisabledState() => Disabled || ParentDisabled || ParentReadOnly;

        private int _numberOfActiveFileInputs = 1;
        private string? GetInputClass(int fileInputIndex) => fileInputIndex == _numberOfActiveFileInputs
            ? InputClasses
            : $"{InputClasses} d-none";
        private string GetInputId(int fileInputIndex) => $"{_id}-{fileInputIndex}";
        private string GetActiveInputId() => $"{_id}-{_numberOfActiveFileInputs}";

        /// <summary>
        /// Removes a file from <see cref="Files"/> by its filename if T is an  <see cref="IBrowserFile" /> or <see cref="IReadOnlyList{IBrowserFile}">IReadOnlyList&lt;IBrowserFile&gt;</see>.
        /// </summary>
        /// <param name="filename">The name of the file to remove.</param>
        public async Task RemoveFile(string filename)
        {
            switch (_filesState.Value)
            {
                case IBrowserFile singleFile when singleFile.Name == filename:
                    await _filesState.SetValueAsync(default); // Remove the single file by setting Files to null/default
                    break;

                case IReadOnlyList<IBrowserFile> fileList:
                    var updatedList = fileList.Where(file => file.Name != filename).ToList();
                    await _filesState.SetValueAsync((T)(object)updatedList); // Cast to T to update Files
                    break;
            }
        }

        public async Task ClearAsync()
        {
            _numberOfActiveFileInputs = 1;
            await NotifyValueChangedAsync(default);
            await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudInput.resetValue", GetActiveInputId());
        }

        /// <summary>
        /// Opens the file picker.
        /// </summary>
        public async Task OpenFilePickerAsync()
            => await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudFileUpload.openFilePicker", GetActiveInputId());

        private async Task OnChangeAsync(InputFileChangeEventArgs args)
        {
            _numberOfActiveFileInputs++;

            if (GetDisabledState())
            {
                return;
            }

            T? value;
            if (typeof(T) == typeof(IReadOnlyList<IBrowserFile>))
            {
                var newFiles = args.GetMultipleFiles(MaximumFileCount);
                if (AppendMultipleFiles && _filesState.Value is IReadOnlyList<IBrowserFile> oldFiles)
                {
                    var allFiles = oldFiles.Concat(newFiles).ToList();
                    value = (T)(object)allFiles.AsReadOnly();
                }
                else
                {
                    value = (T)newFiles;
                }
            }
            else if (typeof(T) == typeof(IBrowserFile))
            {
                value = args.FileCount == 1 ? (T)args.File : default;
            }
            else
            {
                return;
            }

            await NotifyValueChangedAsync(value);

            if (!Error || !SuppressOnChangeWhenInvalid) // only trigger FilesChanged if validation passes or SuppressOnChangeWhenInvalid is false
            {
                await OnFilesChanged.InvokeAsync(args);
            }
        }

        protected override void OnInitialized()
        {
            if (!(typeof(T) == typeof(IReadOnlyList<IBrowserFile>) || typeof(T) == typeof(IBrowserFile)))
            {
                Logger.LogWarning("T must be of type {type1} or {type2}", typeof(IReadOnlyList<IBrowserFile>), typeof(IBrowserFile));
            }

            base.OnInitialized();
        }

        private async Task NotifyValueChangedAsync(T? value)
        {
            Touched = true;
            await _filesState.SetValueAsync(value);
            await BeginValidateAsync();
            FieldChanged(value);
        }

        protected override T? ReadValue() => _filesState.Value;

        protected override Task WriteValueAsync(T? value) => _filesState.SetValueAsync(value);
    }
}
