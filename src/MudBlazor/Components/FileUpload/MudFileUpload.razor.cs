// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MudBlazor.Interfaces;
using MudBlazor.Resources;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// A form component for uploading one or more files.  For <c>T</c>, use either <c>IBrowserFile</c> for a single file or <c>IReadOnlyList&lt;IBrowserFile&gt;</c> for multiple files.
    /// </summary>
    /// <typeparam name="T">Either <see cref="IBrowserFile"/> for a single file or <see cref="IReadOnlyList{IBrowserFile}">IReadOnlyList&lt;IBrowserFile&gt;</see> for multiple files.</typeparam>
    public partial class MudFileUpload<T> : MudFormComponent<T, string>, IActivatable
    {
        private readonly ParameterState<T?> _filesState;

        [Inject]
        private IJSRuntime JsRuntime { get; set; } = null!;

        [Inject]
        private InternalMudLocalizer Localizer { get; set; } = null!;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public MudFileUpload() : base(new DefaultConverter<T>())
        {
            using var registerScope = CreateRegisterScope();
            _filesState = registerScope.RegisterParameter<T?>(nameof(Files))
                .WithParameter(() => Files)
                .WithEventCallback(() => FilesChanged);
        }

        private readonly string _id = Identifier.Create();
        private readonly List<string> _validationErrors = [];

        protected string Classname =>
            new CssBuilder("mud-file-upload")
                .AddClass(Class)
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
        /// The custom content which, when clicked, opens the file picker.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public RenderFragment? ActivatorContent { get; set; }

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
        /// The maximum number of files retrieved during a call to <see cref="InputFileChangeEventArgs.GetMultipleFiles(int)"/>.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>10</c>.  This property does not limit the total number of uploaded files allowed; a limit should be validated manually, such as during the <see cref="FilesChanged"/> event.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public int MaximumFileCount { get; set; } = 10;

        /// <summary>
        /// The maximum file size in bytes.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c> (no limit). When a file exceeds this limit, the upload for that file will be prevented.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public long? MaxFileSize { get; set; }

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

        protected bool GetDisabledState() => Disabled || ParentDisabled || ParentReadOnly;

        private int _numberOfActiveFileInputs = 1;
        private string? GetInputClass(int fileInputIndex) => fileInputIndex == _numberOfActiveFileInputs
            ? InputClass
            : $"{InputClass} d-none";
        private string GetInputId(int fileInputIndex) => $"{_id}-{fileInputIndex}";
        private string GetActiveInputId() => $"{_id}-{_numberOfActiveFileInputs}";

        public async Task ClearAsync()
        {
            ValidationErrors.RemoveAll(_validationErrors.Contains);

            _validationErrors.Clear();
            _numberOfActiveFileInputs = 1;

            await NotifyValueChangedAsync(default);
            await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudInput.resetValue", GetActiveInputId());
        }

        /// <summary>
        /// Opens the file picker.
        /// </summary>
        public async Task OpenFilePickerAsync()
            => await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudFileUpload.openFilePicker", GetActiveInputId());

        /// <summary>
        /// Opens the file picker.
        /// </summary>
        /// <param name="activator">The object which raised the event.</param>
        /// <param name="args">The coordinates of the mouse when clicked.</param>
        public void Activate(object activator, MouseEventArgs args)
            => _ = OpenFilePickerAsync();

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

            if (!Error || !SuppressOnChangeWhenInvalid)
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

        protected override T? ReadValue() => _filesState.Value;

        protected override Task WriteValueAsync(T? value) => _filesState.SetValueAsync(value);

        protected override async Task ValidateValue()
        {
            await base.ValidateValue();

            ValidationErrors = [.. ValidationErrors, .. _validationErrors];
            Error = ValidationErrors.Count > 0;
            ErrorText = ValidationErrors.FirstOrDefault();
        }

        public override void ResetValidation()
        {
            _validationErrors.Clear();

            base.ResetValidation();
        }
    }
}
