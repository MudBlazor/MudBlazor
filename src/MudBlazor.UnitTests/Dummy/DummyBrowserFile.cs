// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
using System;
using System.IO;
using System.Threading;
using Microsoft.AspNetCore.Components.Forms;

namespace MudBlazor.UnitTests.Dummy;

public class DummyBrowserFile : IBrowserFile
{
    public string Name { get; set;  }
    public DateTimeOffset LastModified { get; set; }
    public long Size { get; set; }
    public string ContentType { get; set; }
    public byte[] Content { get; set; }

    public DummyBrowserFile(string name, DateTimeOffset lastModified, long size, string contentType, byte[] content)
    {
        Name = name;
        LastModified = lastModified;
        Size = size;
        ContentType = contentType;
        Content = content;
    }

    public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = new())
        => new MemoryStream(Content);
}
