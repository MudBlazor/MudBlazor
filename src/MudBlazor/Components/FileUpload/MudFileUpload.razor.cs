// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudFileUpload<T> : MudFormComponent<T, string>
    {
        public MudFileUpload() : base(new DefaultConverter<T>()) { }

        private readonly string _id = $"mud_fileupload_{Guid.NewGuid()}";

        protected string Classname =>
            new CssBuilder("mud-file-upload")
                .AddClass(Class)
                .Build();

        /// <summary>
        /// The value of the MudFileUpload component.
        /// If T is <see cref="IBrowserFile">IBrowserFile</see>, it represents a single file.
        /// If T is <see cref="IReadOnlyCollection{IBrowserFile}">IReadOnlyList&lt;IBrowserFile&gt;</see>, it represents multiple files
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public T Files
        {
            get => _value;
            set
            {
                if (_value != null && _value.Equals(value))
                    return;
                _value = value;
            }
        }

        /// <summary>
        /// Triggered when the internal OnChange event fires
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public EventCallback<T> FilesChanged { get; set; }

        /// <summary>
        /// Called when the internal files are changed
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public EventCallback<InputFileChangeEventArgs> OnFilesChanged { get; set; }

        /// <summary>
        /// If true, when T is of type IReadOnlyList, additional files will be appended to the existing list
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public bool AppendMultipleFiles { get; set; }

        /// <summary>
        /// Renders the button that triggers the input. Required for functioning.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Appearance)]
        public RenderFragment<FileUploadButtonTemplateContext<T>> ButtonTemplate { get; set; }

        /// <summary>
        /// Renders the selected files, if desired.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Appearance)]
        public RenderFragment<T> SelectedTemplate { get; set; }

        /// <summary>
        /// If true, OnFilesChanged will not trigger if validation fails
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public bool SuppressOnChangeWhenInvalid { get; set; }

        /// <summary>
        /// Sets the file types this input will accept
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public string Accept { get; set; }

        /// <summary>
        /// If false, the inner FileInput will be visible
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Appearance)]
        public bool Hidden { get; set; } = true;

        /// <summary>
        /// Css classes to apply to the internal InputFile
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Appearance)]
        public string InputClass { get; set; }

        /// <summary>
        /// Style to apply to the internal InputFile
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Appearance)]
        public string InputStyle { get; set; }

        /// <summary>
        /// Represents the maximum number of files that can retrieved from the internal call to
        /// InputFileChangeEventArgs.GetMultipleFiles().
        /// It does not limit the total number of uploaded files
        /// when AppendMultipleFiles="true". A limit should be validated manually, for
        /// example in the FilesChanged event callback.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public int MaximumFileCount { get; set; } = 10;

        /// <summary>
        /// Disables the FileUpload
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public bool Disabled { get; set; }

        [CascadingParameter(Name = "ParentDisabled")]
        private bool ParentDisabled { get; set; }

        [CascadingParameter(Name = "ParentReadOnly")]
        private bool ParentReadOnly { get; set; }

        protected bool GetDisabledState() => Disabled || ParentDisabled || ParentReadOnly;

        public async Task ClearAsync()
        {
            _value = default;
            await NotifyValueChangedAsync();
        }

        private async Task OnChange(InputFileChangeEventArgs args)
        {
            if (GetDisabledState()) return;
            if (typeof(T) == typeof(IReadOnlyList<IBrowserFile>))
            {
                var newFiles = args.GetMultipleFiles(MaximumFileCount);
                if (AppendMultipleFiles && _value is IReadOnlyList<IBrowserFile> oldFiles)
                {
                    var allFiles = oldFiles.Concat(newFiles).ToList();
                    _value = (T)(object)allFiles.AsReadOnly();
                }
                else
                {
                    _value = (T)newFiles;
                }
            }
            else if (typeof(T) == typeof(IBrowserFile))
            {
                _value = args.FileCount == 1 ? (T)args.File : default;
            }
            else return;

            await NotifyValueChangedAsync();

            if (!Error
                || !SuppressOnChangeWhenInvalid) // only trigger FilesChanged if validation passes or SuppressOnChangeWhenInvalid is false
                await OnFilesChanged.InvokeAsync(args);
        }

        protected override void OnInitialized()
        {
            if (!(typeof(T) == typeof(IReadOnlyList<IBrowserFile>) || typeof(T) == typeof(IBrowserFile)))
                Logger.LogWarning("T must be of type {type1} or {type2}", typeof(IReadOnlyList<IBrowserFile>),
                    typeof(IBrowserFile));

            base.OnInitialized();
        }

        private async Task NotifyValueChangedAsync()
        {
            Touched = true;
            await FilesChanged.InvokeAsync(_value);
            await BeginValidateAsync();
            FieldChanged(_value);
        }
    }
}
