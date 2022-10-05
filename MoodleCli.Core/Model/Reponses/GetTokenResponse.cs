using System.Text.Json.Serialization;

namespace MoodleCli.Core.Model.Reponses
{
    internal class GetTokenResponse
    {
        [JsonPropertyName("token")]
        public string? Token { get; set; }
    }
}
