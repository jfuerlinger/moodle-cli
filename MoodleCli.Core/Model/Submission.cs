namespace MoodleCli.Core.Model
{
    public class SubmissionFile
    {
        public int SubmissionId { get; set; }
        public int CourseId { get; set; }
        public int UserId { get; set; }
        public string? UserFullName { get; set; }
        
        public string? Filename { get; set; }
        public string? Url { get; set; }
        public int Size { get; set; }
    }
}
