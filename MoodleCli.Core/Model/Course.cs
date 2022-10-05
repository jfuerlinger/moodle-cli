using System.Text.Json.Serialization;

using MoodleCli.Core.Infrastructure;

namespace MoodleCli.Core.Model
{
    public class Course
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("shortname")]
        public string? ShortName { get; set; }

        [JsonPropertyName("fullname")]
        public string? FullName { get; set; }

        [JsonPropertyName("startdate")]
        public long? StartDateUnixTimestamp { get; set; }

        [JsonIgnore]
        public DateTime? StartDate { get => StartDateUnixTimestamp?.UnixTimeStampToDateTime(); }
    }
}
