namespace BlazorRepl.Client.Services
{
    using System.Collections.Generic;

    public class CreateSnippetRequestModel
    {
        public IEnumerable<CreateSnippetFileRequestModel> Files { get; set; } = new List<CreateSnippetFileRequestModel>();
    }
}
