using System.Text.Json.Serialization;

namespace MoodleCli.Core.Model.Reponses
{
    public class GetSubmissionsResponse
    {
        [JsonPropertyName("assignments")]
        public Assignment[]? Assignments { get; set; }

        public class Assignment
        {
            [JsonPropertyName("assignmentid")]
            public int Id { get; set; }

            [JsonPropertyName("submissions")]
            public Submission[]? Submissions { get; set; }

            public class Submission
            {
                [JsonPropertyName("id")]
                public int Id { get; set; }

                [JsonPropertyName("userid")]
                public int UserId { get; set; }

                [JsonPropertyName("plugins")]
                public Plugin[]? Plugins { get; set; }

                public class Plugin
                {
                    [JsonPropertyName("type")]
                    public string? Type { get; set; }

                    [JsonPropertyName("name")]
                    public string? Name { get; set; }

                    [JsonPropertyName("fileareas")]
                    public Filearea[]? Fileareas { get; set; }

                    public class Filearea
                    {
                        [JsonPropertyName("area")]
                        public string? Area { get; set; }

                        [JsonPropertyName("files")]
                        public File[]? Files { get; set; }

                        public class File
                        {
                            [JsonPropertyName("filename")]
                            public string? Name { get; set; }

                            [JsonPropertyName("fileurl")]
                            public string? Url { get; set; }

                            [JsonPropertyName("filesize")]
                            public int Size { get; set; }
                        }
                    }
                }
            }
        }
    }
}
