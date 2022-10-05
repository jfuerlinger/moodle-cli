using System.Text.Json.Serialization;

namespace MoodleCli.Core.Model
{
    public class User
    {
        [JsonPropertyName("userid")]
        public int Id { get; set; }

        [JsonPropertyName("firstname")]
        public string? FirstName { get; set; }

        [JsonPropertyName("lastname")]
        public string? LastName { get; set; }

        [JsonPropertyName("fullname")]
        public string? FullName { get; set; }
    }
}
