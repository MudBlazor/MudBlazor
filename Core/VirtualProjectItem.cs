namespace BlazorRepl.Core
{
    using System.IO;
    using Microsoft.AspNetCore.Razor.Language;

    internal class VirtualProjectItem : RazorProjectItem
    {
        private readonly byte[] content;

        public VirtualProjectItem(
            string basePath,
            string filePath,
            string physicalPath,
            string relativePhysicalPath,
            string fileKind,
            byte[] content)
        {
            this.BasePath = basePath;
            this.FilePath = filePath;
            this.PhysicalPath = physicalPath;
            this.RelativePhysicalPath = relativePhysicalPath;
            this.content = content;

            // Base class will detect based on file-extension.
            this.FileKind = fileKind ?? base.FileKind;
        }

        public override string BasePath { get; }

        public override string RelativePhysicalPath { get; }

        public override string FileKind { get; }

        public override string FilePath { get; }

        public override string PhysicalPath { get; }

        public override bool Exists => true;

        public override Stream Read() => new MemoryStream(this.content);
    }
}
