namespace BlazorRepl.Client.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using BlazorRepl.Core;
    using Microsoft.CodeAnalysis.CSharp;

    public static class CodeFilesHelper
    {
        private const string ValidCodeFileExtension = ".razor";
        private const int MainComponentFileContentMinLength = 10;

        public static string NormalizeCodeFilePath(string path, out string error)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                error = "Path cannot be empty.";
                return null;
            }

            var trimmedPath = path.Trim();

            var extension = Path.GetExtension(trimmedPath);
            if (!string.IsNullOrEmpty(extension) &&
                !ValidCodeFileExtension.Equals(extension, StringComparison.OrdinalIgnoreCase))
            {
                error = $"Extension cannot be '{extension}'. Valid extensions: {ValidCodeFileExtension}";
                return null;
            }

            var fileName = trimmedPath.Substring(0, trimmedPath.Length - extension.Length);
            if (!SyntaxFacts.IsValidIdentifier(fileName))
            {
                error = $"'{fileName}' is not a valid file name. It must be a valid C# identifier.";
                return null;
            }

            if (char.IsLower(fileName[0]))
            {
                error = $"'{fileName}' starts with a lowercase character. File names must start with an uppercase character or _.";
                return null;
            }

            error = null;
            return fileName + ValidCodeFileExtension;
        }

        public static string ValidateCodeFilesForSnippetCreation(IEnumerable<CodeFile> codeFiles)
        {
            if (codeFiles == null)
            {
                throw new ArgumentNullException(nameof(codeFiles));
            }

            var containsMainComponent = false;
            var filePaths = new HashSet<string>();
            var index = 0;
            foreach (var codeFile in codeFiles)
            {
                if (codeFile == null)
                {
                    return $"File #{index} - no file.";
                }

                if (string.IsNullOrWhiteSpace(codeFile.Path))
                {
                    return $"File #{index} - no file path.";
                }

                if (filePaths.Contains(codeFile.Path))
                {
                    return $"File '{codeFile.Path}' is duplicated.";
                }

                var extension = Path.GetExtension(codeFile.Path);
                if (!ValidCodeFileExtension.Equals(extension, StringComparison.Ordinal))
                {
                    return $"File '{codeFile.Path}' has invalid extension: {extension}. Valid extensions: {ValidCodeFileExtension}";
                }

                var fileName = codeFile.Path.Substring(0, codeFile.Path.Length - extension.Length);
                if (!SyntaxFacts.IsValidIdentifier(fileName))
                {
                    return $"'{fileName}' is not a valid file name. It must be a valid C# identifier.";
                }

                if (char.IsLower(fileName[0]))
                {
                    return $"'{fileName}' starts with a lowercase character. File names must start with an uppercase character or _.";
                }

                if (codeFile.Path == CoreConstants.MainComponentFilePath)
                {
                    if (string.IsNullOrWhiteSpace(codeFile.Content) || codeFile.Content.Trim().Length < MainComponentFileContentMinLength)
                    {
                        return $"Main component content should be at least {MainComponentFileContentMinLength} characters long.";
                    }

                    containsMainComponent = true;
                }

                filePaths.Add(codeFile.Path);
                index++;
            }

            if (!containsMainComponent)
            {
                return "No main component file provided.";
            }

            return null;
        }
    }
}
