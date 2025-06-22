using System.Text.Json.Serialization;

namespace MudBlazor.Docs.Models
{
    public class GitHubUser
    {
        [JsonPropertyName("login")]
        public string Login { get; set; }

        [JsonPropertyName("avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("bio")]
        public string Bio { get; set; }
    }
}
