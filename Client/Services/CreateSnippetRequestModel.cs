namespace BlazorRepl.Client.Services
{
    using System.Collections.Generic;

    public class CreateSnippetRequestModel
    {
        public IEnumerable<SnippetFile> Files { get; set; } = new List<SnippetFile>();
    }
}
