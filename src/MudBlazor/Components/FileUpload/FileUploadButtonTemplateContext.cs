// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Threading.Tasks;

namespace MudBlazor;

public class FileUploadButtonTemplateContext<T>
{
    public string Id { get; }
    public FileUploadButtonTemplateActions Actions { get; }

    private readonly MudFileUpload<T> _fileUpload;

    public FileUploadButtonTemplateContext(MudFileUpload<T> fileUpload, string id)
    {
        _fileUpload = fileUpload;
        Id = id;
        Actions = new FileUploadButtonTemplateActions
        {
            ClearAsync = async () => await _fileUpload.ClearAsync()
        };
    }

    public class FileUploadButtonTemplateActions
    {
        public Func<Task> ClearAsync { get; init; } = null!;
    }

    public override string ToString()
        => Id;
}
