// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.Threading.Tasks;

namespace MudBlazor;

#nullable enable
public class FileUploadButtonTemplateContext<T>
{
    public string Id { get; }

    public FileUploadButtonTemplateActions Actions { get; }

    public FileUploadButtonTemplateContext(MudFileUpload<T> fileUpload, string id)
    {
        Id = id;
        Actions = new FileUploadButtonTemplateActions
        {
            ClearAsync = fileUpload.ClearAsync
        };
    }

    public override string ToString() => Id;

    public class FileUploadButtonTemplateActions
    {
        public Func<Task> ClearAsync { get; init; } = null!;
    }
}
