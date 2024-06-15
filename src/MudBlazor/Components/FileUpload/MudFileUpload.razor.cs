﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MudBlazor.Interfaces;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// A form component for uploading files.
    /// </summary>
    /// <typeparam name="T">Either <see cref="IBrowserFile"/> for a single file or <see cref="IReadOnlyCollection{IBrowserFile}">IReadOnlyCollection&lt;IBrowserFile&gt;</see> for multiple files.</typeparam>
    public partial class MudFileUpload<T> : MudFormComponent<T, string>, IActivatable
    {
        [Inject]
        private IJSRuntime JsRuntime { get; set; } = null!;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public MudFileUpload() : base(new DefaultConverter<T>()) { }

        private readonly string _id = $"mud_fileupload_{Guid.NewGuid()}";

        protected string Classname =>
            new CssBuilder("mud-file-upload")
                .AddClass(Class)
                .Build();

        /// <summary>
        /// The uploaded file or files.
        /// </summary>
        /// <remarks>
        /// When <c>T</c> is <see cref="IBrowserFile" />, a single file is returned.<br />
        /// When <c>T</c> is <see cref="IReadOnlyCollection{IBrowserFile}">IReadOnlyCollection&lt;IBrowserFile&gt;</see>, multiple files are returned.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public T? Files
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
        /// Defaults to <c>false</c>. This applies when <c>T</c> is <see cref="IReadOnlyCollection{IBrowserFile}">IReadOnlyCollection&lt;IBrowserFile&gt;</see>.
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

        public async Task ClearAsync()
        {
            _value = default;
            await NotifyValueChangedAsync();
            await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudInput.resetValue", _id);
        }

        /// <summary>
        /// Opens the file picker.
        /// </summary>
        public async Task OpenFilePickerAsync()
            => await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudFileUpload.openFilePicker", _id);

        /// <summary>
        /// Opens the file picker.
        /// </summary>
        /// <param name="activator">The object which raised the event.</param>
        /// <param name="args">The coordinates of the mouse when clicked.</param>
        public void Activate(object activator, MouseEventArgs args)
            => _ = OpenFilePickerAsync();

        private async Task OnChangeAsync(InputFileChangeEventArgs args)
        {
            if (GetDisabledState())
            {
                return;
            }

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
            else
            {
                return;
            }

            await NotifyValueChangedAsync();

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

        private async Task NotifyValueChangedAsync()
        {
            Touched = true;
            await FilesChanged.InvokeAsync(_value);
            await BeginValidateAsync();
            FieldChanged(_value);
        }
    }
}
