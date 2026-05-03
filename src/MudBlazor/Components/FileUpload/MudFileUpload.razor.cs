// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MudBlazor.Resources;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{

    /// <summary>
    /// A form component that lets users upload one or more files.
    /// </summary>
    /// <typeparam name="T">Use <see cref="IBrowserFile"/> for a single file or <see cref="IReadOnlyList{IBrowserFile}">IReadOnlyList&lt;IBrowserFile&gt;</see> for multiple files.</typeparam>
    public partial class MudFileUpload<T> : MudFormComponent<T, string>
    {
        private readonly ParameterState<T?> _filesState;
        private readonly ParameterState<bool> _draggingState;

        [Inject]
        private IJSRuntime JsRuntime { get; set; } = null!;

        [Inject]
        private InternalMudLocalizer Localizer { get; set; } = null!;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public MudFileUpload()
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
        private readonly List<string> _validationErrors = [];

        protected string Classname =>
            new CssBuilder("mud-file-upload")
                .AddClass(Class)
                .Build();

        protected string DragClass =>
            new CssBuilder("mud-file-upload-dragarea")
                .AddClass("mud-file-upload-dragarea-clickable", !GetDisabledState())
                .AddClass("mud-border-primary", _draggingState.Value)
                .Build();

        protected string InputClasses =>
            new CssBuilder(InputClass)
                .AddClass("mud-file-upload-dragover", DragAndDrop && (Hidden || CustomContent == null))
                .Build();

        /// <summary>
        /// The selected file or files.
        /// </summary>
        /// <remarks>
        /// When <c>T</c> is <see cref="IBrowserFile" />, a single file is provided.<br />
        /// When <c>T</c> is <see cref="IReadOnlyList{IBrowserFile}">IReadOnlyList&lt;IBrowserFile&gt;</see>, multiple files are provided.
        /// </remarks>
        [Parameter, ParameterState]
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
        /// Occurs when the user selects or drops files.
        /// </summary>
        /// <remarks>
        /// Not raised when <see cref="SuppressOnChangeWhenInvalid"/> is <c>true</c> and validation fails.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public EventCallback<InputFileChangeEventArgs> OnFilesChanged { get; set; }

        /// <summary>
        /// Occurs when a drag operation enters this component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public EventCallback<DragEventArgs> OnDragEnter { get; set; }

        /// <summary>
        /// Occurs when files are dropped onto this component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public EventCallback<DragEventArgs> OnDrop { get; set; }

        /// <summary>
        /// Occurs when a drag operation leaves this component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public EventCallback<DragEventArgs> OnDragLeave { get; set; }

        /// <summary>
        /// Occurs when a drag operation ends for this component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public EventCallback<DragEventArgs> OnDragEnd { get; set; }

        /// <summary>
        /// Appends new files to the existing selection.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  This applies when <c>T</c> is <see cref="IReadOnlyList{IBrowserFile}">IReadOnlyList&lt;IBrowserFile&gt;</see>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public bool AppendMultipleFiles { get; set; }

        /// <summary>
        /// The custom content used to render the upload UI.
        /// </summary>
        /// <remarks>
        /// The context is the current <see cref="MudFileUpload{T}"/> instance and can be used to call <see cref="OpenFilePickerAsync"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public RenderFragment<MudFileUpload<T>>? CustomContent { get; set; }

        /// <summary>
        /// The template used to render selected files.
        /// </summary>
        /// <remarks>
        /// When <c>null</c>, a default chip list is shown.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Appearance)]
        public RenderFragment<T?>? SelectedTemplate { get; set; }

        /// <summary>
        /// Prevents raising <see cref="OnFilesChanged"/> when validation fails during an upload.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public bool SuppressOnChangeWhenInvalid { get; set; }

        /// <summary>
        /// The accepted file types for the file picker.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c> for any file type.  Multiple file types must be separated by commas (e.g. <c>".png,.jpg"</c>).
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public string? Accept { get; set; }

        /// <summary>
        /// Hides the internal <see cref="InputFile"/> element.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.  When <c>false</c>, the input is visible.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Appearance)]
        public bool Hidden { get; set; } = true;

        /// <summary>
        /// Enables a drag-and-drop area inside the component.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  During drag operations, the internal <see cref="InputFile"/> element can be shown to capture drop events even when <see cref="Hidden"/> is <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public bool DragAndDrop { get; set; }

        /// <summary>
        /// Indicates whether a drag operation is active.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, the internal <see cref="InputFile"/> element is shown to capture the drop, which temporarily overrides <see cref="Hidden"/>.  When <c>false</c>, visibility follows <see cref="Hidden"/> again.  Drop-related events reset it to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public bool Dragging { get; set; }

        /// <summary>
        /// The CSS classes applied to the internal <see cref="InputFile"/> element.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  These classes apply when <see cref="Hidden"/> is <c>false</c>.  Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Appearance)]
        public string? InputClass { get; set; }

        /// <summary>
        /// The CSS styles applied to the internal <see cref="InputFile"/> element.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  These styles apply when <see cref="Hidden"/> is <c>false</c>.  Prefer <see cref="InputClass"/>.
        /// </remarks>
        [Obsolete("Prefer the InputClass property with CSS https://github.com/MudBlazor/MudBlazor/issues/12047")]
        [Parameter]
        [Category(CategoryTypes.FileUpload.Appearance)]
        public string? InputStyle { get; set; }

        /// <summary>
        /// The maximum number of files retrieved per selection.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>10</c>.  This does not limit the total number of files; enforce overall limits in <see cref="FilesChanged"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public int MaximumFileCount { get; set; } = 10;

        /// <summary>
        /// The maximum allowed file size in bytes.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c> (no limit).  Files exceeding this limit are rejected and a validation error is added.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public long? MaxFileSize { get; set; }

        /// <summary>
        /// Prevents the user from interacting with this component.
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
        /// Returns the filenames for the selected files.
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

        private async Task OnDragResetAsync()
        {
            await _draggingState.SetValueAsync(false);
        }

        private async Task OnDropAsync(DragEventArgs args)
        {
            await OnDragResetAsync();
            await OnDrop.InvokeAsync(args);
        }

        private async Task OnDragLeaveAsync(DragEventArgs args)
        {
            await OnDragResetAsync();
            await OnDragLeave.InvokeAsync(args);
        }

        private async Task OnDragEndAsync(DragEventArgs args)
        {
            await OnDragResetAsync();
            await OnDragEnd.InvokeAsync(args);
        }

        private Task OnDragAreaClickAsync()
        {
            if (GetDisabledState())
            {
                return Task.CompletedTask;
            }

            return OpenFilePickerAsync();
        }

        private async Task OnDragEnterAsync(DragEventArgs args)
        {
            if (GetDisabledState())
            {
                return;
            }

            await _draggingState.SetValueAsync(true);
            await OnDragEnter.InvokeAsync(args);
        }

        private Task OnFileChipCloseAsync(string filename)
        {
            if (GetDisabledState())
            {
                return Task.CompletedTask;
            }

            return RemoveFileAsync(filename);
        }

        private int _numberOfActiveFileInputs = 1;
        private string? GetInputClass(int fileInputIndex) => fileInputIndex == _numberOfActiveFileInputs
            ? InputClasses
            : $"{InputClasses} d-none";
        private string GetInputId(int fileInputIndex) => $"{_id}-{fileInputIndex}";
        private string GetActiveInputId() => $"{_id}-{_numberOfActiveFileInputs}";

        /// <summary>
        /// Removes all files with the specified name from <see cref="Files"/>.
        /// </summary>
        /// <param name="filename">The name of the file(s) to remove.</param>
        /// <returns>
        /// <c>true</c> if one or more files were removed; otherwise, <c>false</c>
        /// if no file with the specified name exists in the current selection.
        /// </returns>
        public async Task<bool> RemoveFileAsync(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                Logger.LogWarning("Attempted to remove a file with an invalid filename.");
                return false;
            }

            var current = _filesState.Value;

            switch (current)
            {
                case IBrowserFile singleFile when singleFile.Name == filename:
                    // Single selection with matching name: clear selection
                    await _filesState.SetValueAsync(default);
                    return true;

                case IReadOnlyList<IBrowserFile> fileList:
                    {
                        var updatedList = fileList.Where(f => f.Name != filename).ToList();
                        if (updatedList.Count == fileList.Count)
                        {
                            Logger.LogDebug("File '{Filename}' not found in the current selection.", filename);
                            return false;
                        }

                        if (updatedList.Count == 0)
                        {
                            // Treat removal of the last file as "no selection"
                            await _filesState.SetValueAsync(default);
                        }
                        else
                        {
                            // T is expected to be IReadOnlyList<IBrowserFile> here
                            await _filesState.SetValueAsync((T)(object)updatedList);
                        }

                        return true;
                    }

                case null:
                    Logger.LogDebug("No files are currently selected.");
                    return false;

                default:
                    Logger.LogWarning(
                        "File removal by name is not supported for the current files type '{Type}'.",
                        current.GetType().FullName);
                    return false;
            }
        }

        /// <summary>
        /// Removes the specified file instance from <see cref="Files"/>.
        /// </summary>
        /// <param name="file">The file instance to remove.</param>
        /// <returns>
        /// <c>true</c> if the specified file instance was removed; otherwise, <c>false</c>.
        /// </returns>
        public async Task<bool> RemoveFileAsync(IBrowserFile file)
        {
            if (file is null)
            {
                Logger.LogWarning("Attempted to remove a null file.");
                return false;
            }

            var current = _filesState.Value;

            switch (current)
            {
                case IBrowserFile singleFile when ReferenceEquals(singleFile, file):
                    // Single selection with the same instance: clear selection
                    await _filesState.SetValueAsync(default);
                    return true;

                case IReadOnlyList<IBrowserFile> fileList:
                    {
                        // Remove this specific instance (reference-based via List.Remove)
                        var updatedList = fileList.ToList();
                        if (!updatedList.Remove(file))
                        {
                            Logger.LogDebug("The specified file instance was not found in the current selection.");
                            return false;
                        }

                        if (updatedList.Count == 0)
                        {
                            await _filesState.SetValueAsync(default);
                        }
                        else
                        {
                            await _filesState.SetValueAsync((T)(object)updatedList);
                        }

                        return true;
                    }

                case null:
                    Logger.LogDebug("No files are currently selected.");
                    return false;

                default:
                    Logger.LogWarning(
                        "File removal by instance is not supported for the current files type '{Type}'.",
                        current.GetType().FullName);
                    return false;
            }
        }


        /// <summary>
        /// Clears the selected files and resets the internal file inputs.
        /// </summary>
        /// <remarks>
        /// This also removes validation errors produced by file size checks.
        /// </remarks>
        public async Task ClearAsync()
        {
            ValidationErrors.RemoveAll(_validationErrors.Contains);

            _validationErrors.Clear();
            _numberOfActiveFileInputs = 1;

            await NotifyValueChangedAsync(default);
            await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudInput.resetValue", GetActiveInputId());
        }

        /// <summary>
        /// Opens the native file picker for the active input.
        /// </summary>
        public async Task OpenFilePickerAsync()
            => await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudFileUpload.openFilePicker", GetActiveInputId());

        private async Task OnChangeAsync(InputFileChangeEventArgs args)
        {
            _numberOfActiveFileInputs++;

            if (GetDisabledState())
                return;

            await ProcessFileChangeAsync(args);
        }

        private async Task ProcessFileChangeAsync(InputFileChangeEventArgs args)
        {
            T? value;

            if (args.FileCount > MaximumFileCount)
            {
                // Notify the consumer about the exceeded file count
                _validationErrors.Add(Localizer[LanguageResource.MudFileUpload_MaximumFileCountExceeded, args.FileCount, MaximumFileCount]);
                await NotifyValueChangedAsync(default); // Reset the value to indicate no valid files were processed
                return;
            }

            if (typeof(T) == typeof(IReadOnlyList<IBrowserFile>))
            {
                value = (T?)(object)ProcessMultipleFiles(args.GetMultipleFiles(MaximumFileCount));
            }
            else if (typeof(T) == typeof(IBrowserFile))
            {
                value = (T?)ProcessSingleFile(args.FileCount == 1 ? args.File : null);
            }
            else
            {
                return;
            }

            await NotifyValueChangedAsync(value);

            if (!ErrorState.Value || !SuppressOnChangeWhenInvalid)
                await OnFilesChanged.InvokeAsync(args);
        }

        private IReadOnlyList<IBrowserFile> ProcessMultipleFiles(IReadOnlyCollection<IBrowserFile> files)
        {
            var validFiles = new List<IBrowserFile>();

            foreach (var file in files)
            {
                if (MaxFileSize.HasValue && file.Size > MaxFileSize.Value)
                {
                    _validationErrors.Add(Localizer[LanguageResource.MudFileUpload_FileSizeError, file.Name, MaxFileSize.Value.ToString()]);
                }
                else
                {
                    validFiles.Add(file);
                }
            }

            var newFiles = validFiles.AsReadOnly();

            if (AppendMultipleFiles && _filesState.Value is IReadOnlyList<IBrowserFile> oldFiles)
                return oldFiles.Concat(newFiles).ToList().AsReadOnly();

            return newFiles;
        }

        private IBrowserFile? ProcessSingleFile(IBrowserFile? file)
        {
            if (file == null)
                return null;

            if (MaxFileSize.HasValue && file.Size > MaxFileSize.Value)
            {
                _validationErrors.Add(Localizer[LanguageResource.MudFileUpload_FileSizeError, file.Name, MaxFileSize.Value.ToString()]);
                return null;
            }

            return file;
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

        /// <inheritdoc />
        protected override IConverter<T?, string?> GetDefaultConverter()
        {
            return new DefaultConverter<T>
            {
                Culture = GetCulture,
                Format = GetFormat
            };
        }

        protected internal override T? ReadValue => _filesState.Value;

        protected override Task SetValueCoreAsync(T? value) => _filesState.SetValueAsync(value);

        protected override async Task ValidateValue()
        {
            await base.ValidateValue();

            ValidationErrors = [.. ValidationErrors, .. _validationErrors];
            await ErrorState.SetValueAsync(ValidationErrors.Count > 0);
            await ErrorTextState.SetValueAsync(ValidationErrors.FirstOrDefault());
        }

        /// <inheritdoc />
        public override Task ResetValidationAsync()
        {
            _validationErrors.Clear();

            return base.ResetValidationAsync();
        }
    }
}
