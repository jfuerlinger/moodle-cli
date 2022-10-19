using MoodleCli.Core.Model;
using MoodleCli.Core.Model.Reponses;

namespace MoodleCli.Core.Services
{
    public interface IMoodleService : IDisposable
    {
        Task<(SubmissionFile, Stream)> DownloadSubmissionFileAsync(SubmissionFile submissionFile);
        Task<Assignment[]> GetAssignmentsForCourse(int courseId);
        Task<User?> GetCurrentUsersInfos();
        Task<SubmissionFile[]> GetSubmissionsForAssignmentAsync(int assignmentId);
        Task<UserDetails[]> GetUserDetailsByIdsAsync(int[] ids);
        Task<Course[]> GetUsersCoursesAsync(int userId);
    }
}