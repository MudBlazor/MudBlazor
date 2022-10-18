// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
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

        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public T FileValue
        {
            get => _value;
            set
            {
                if (_value != null && _value.Equals(value))
                    return;
                _value = value;
            }
        }
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public EventCallback<T> FileValueChanged { get; set; }

        /// <summary>
        /// Called when the internal files are changed
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public EventCallback<InputFileChangeEventArgs> OnFilesChanged { get; set; }
        /// <summary>
        /// Renders the button that triggers the input. Required for functioning.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Appearance)]
        public RenderFragment<string> ButtonTemplate { get; set; }
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
        /// If true, multiple files can be uploaded
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public bool Multiple { get; set; }
        /// <summary>
        /// Sets the file types this input will accept
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Behavior)]
        public string Accept { get; set; }
        /// <summary>
        /// Css classes to apply to the internal InputFile
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FileUpload.Appearance)]
        public string InputClass { get; set; }

        private async Task OnChange(InputFileChangeEventArgs args)
        {
            if (Multiple && typeof(T) == typeof(IReadOnlyList<IBrowserFile>))
            {
                _value = (T)args.GetMultipleFiles();
            }
            else if (!Multiple && typeof(T) == typeof(IBrowserFile))
            {
                _value = (T)args.File;
            }

            await FileValueChanged.InvokeAsync(_value);
            FieldChanged(_value);
            await Validate();
            if (!Error || !SuppressOnChangeWhenInvalid) //only trigger FilesChanged if validation passes or SuppressOnChangeWhenInvalid is false
                await OnFilesChanged.InvokeAsync(args);
        }

        protected override void OnInitialized()
        {
            //if (!(typeof(T) == typeof(IReadOnlyList<IBrowserFile>) || typeof(T) == typeof(IBrowserFile)))
            //    throw new InvalidOperationException($"T must be of type {typeof(IReadOnlyList<IBrowserFile>)} or {typeof(IBrowserFile)}");
            base.OnInitialized();
        }
    }
}
