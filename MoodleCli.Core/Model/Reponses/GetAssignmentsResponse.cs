using System.Text.Json.Serialization;

using MoodleCli.Core.Infrastructure;

namespace MoodleCli.Core.Model.Reponses
{
    public class GetAssignmentsResponse
    {
        [JsonPropertyName("courses")]
        public Course[]? Courses { get; set; }

        public partial class Course
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("fullname")]
            public string? Fullname { get; set; }

            [JsonPropertyName("shortname")]
            public string? Shortname { get; set; }

            [JsonPropertyName("timemodified")]
            public long Timemodified { get; set; }

            [JsonPropertyName("assignments")]
            public Assignment[]? Assignments { get; set; }

            public partial class Assignment
            {
                [JsonPropertyName("id")]
                public int Id { get; set; }

                [JsonPropertyName("name")]
                public string? Name { get; set; }

                [JsonPropertyName("nosubmissions")]
                public long Nosubmissions { get; set; }

                [JsonPropertyName("duedate")]
                public long? DueDateUnixTimestamp { get; set; }

                [JsonIgnore]
                public DateTime? DueDate { get => DueDateUnixTimestamp?.UnixTimeStampToDateTime(); }

                [JsonPropertyName("submissionattachments")]
                public long Submissionattachments { get; set; }
            }
        }
    }
    

    //public enum Name { Enabled, Filetypeslist, Maxfilesubmissions, Maxsubmissionsizebytes };

    //public enum Plugin { Comments, Editpdf, File };

    //public enum Subtype { Assignfeedback, Assignsubmission };

}
