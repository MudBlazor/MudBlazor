namespace BlazorRepl.Client.Services
{
    using System;
    using System.IO;

    using Microsoft.CodeAnalysis.CSharp;

    public static class CodeFilesHelper
    {
        private const string ValidCodeFileExtension = ".razor";

        public static string NormalizeCodeFilePath(string path, out string error)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                error = "Path cannot be empty.";
                return null;
            }

            var fileName = Path.GetFileNameWithoutExtension(path);
            if (!SyntaxFacts.IsValidIdentifier(fileName))
            {
                error = $"'{fileName}' is not a valid file name. It should be a valid C# identifier.";
                return null;
            }

            var extension = Path.GetExtension(path);
            if (!string.IsNullOrEmpty(extension) &&
                !ValidCodeFileExtension.Equals(extension, StringComparison.InvariantCultureIgnoreCase))
            {
                error = $"Extension cannot be '{extension}'. Valid extensions: {ValidCodeFileExtension}";
                return null;
            }

            error = null;
            return fileName + ValidCodeFileExtension;
        }
    }
}
