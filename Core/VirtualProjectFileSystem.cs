namespace BlazorRepl.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Razor.Language;

    internal class VirtualRazorProjectFileSystem : RazorProjectFileSystem
    {
        public override IEnumerable<RazorProjectItem> EnumerateItems(string basePath)
        {
            this.NormalizeAndEnsureValidPath(basePath);
            return Enumerable.Empty<RazorProjectItem>();
        }

        [Obsolete]
        public override RazorProjectItem GetItem(string path) => this.GetItem(path, fileKind: null);

        public override RazorProjectItem GetItem(string path, string fileKind)
        {
            this.NormalizeAndEnsureValidPath(path);
            return new NotFoundProjectItem(string.Empty, path);
        }
    }
}
