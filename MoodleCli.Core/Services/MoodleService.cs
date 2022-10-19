using MoodleCli.Core.Model;
using MoodleCli.Core.Model.Reponses;
using System;
using System.Net.Http.Json;
using System.Web;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MoodleCli.Core.Services
{
    public class MoodleService : IMoodleService
    {
        private readonly HttpClient _httpClient;
        private readonly string _username;
        private readonly string _password;
        private string? _token;
        private object _;

        public MoodleService(string username, string password)
        {
            _httpClient = new HttpClient() { BaseAddress = new Uri("https://edufs.edu.htl-leonding.ac.at/moodle/") }; ;
            _username = username;
            _password = password;
        }

        public void Dispose() => _httpClient.Dispose();

        public async Task<User?> GetCurrentUsersInfos()
        {
            return await FetchDataAsync<User>("core_webservice_get_site_info",
                 new Dictionary<string, string>() { });
        }

        public async Task<(SubmissionFile, Stream)> DownloadSubmissionFileAsync(SubmissionFile submissionFile)
        {
            string url = submissionFile.Url!.Replace(_httpClient.BaseAddress!.ToString(), string.Empty);
            return (submissionFile, await _httpClient.GetStreamAsync($"{url}?token={_token}"));
        }

        public async Task<UserDetails[]> GetUserDetailsByIdsAsync(int[] ids)
        {
            var parameters = new Dictionary<string, string>
            {
                { "field", "id" }
            };

            for (int i = 0; i < ids.Length; i++)
            {
                parameters.Add($"values[{i}]", ids[i].ToString());
            }

            var users = await FetchDataAsync<UserDetails[]>(
                "core_user_get_users_by_field",
                parameters);

            return users!;
        }

        public async Task<SubmissionFile[]> GetSubmissionsForAssignmentAsync(int assignmentId)
        {
            var response = await FetchDataAsync<GetSubmissionsResponse>(
                "mod_assign_get_submissions",
                new Dictionary<string, string> { { "assignmentids[]", assignmentId.ToString() } });

            var submissions = response!.Assignments!
                    .SelectMany(entry => entry.Submissions!);

            List<SubmissionFile> result = new();
            foreach (var submission in submissions)
            {
                foreach (var plugin in submission.Plugins!.Where(plugin => plugin.Type == "file"))
                {
                    foreach (var filearea in plugin.Fileareas!.Where(area => area.Area == "submission_files"))
                    {
                        foreach (var file in filearea.Files!)
                        {
                            result.Add(new SubmissionFile
                            {
                                UserId = submission.UserId,
                                SubmissionId = submission.Id,
                                Filename = file.Name,
                                Size = file.Size,
                                Url = file.Url
                            });
                        }
                    }
                }
            }

            return result.ToArray();
        }

        public async Task<Assignment[]> GetAssignmentsForCourse(int courseId)
        {
            var response = await FetchDataAsync<GetAssignmentsResponse>(
                "mod_assign_get_assignments",
                new Dictionary<string, string> { { "courseids[]", courseId.ToString() } });


            return response!.Courses!
                    .SelectMany(entry => entry.Assignments!)
                    .Select(assignment => new Assignment { Id = assignment.Id, Name = assignment.Name, DueDate = assignment.DueDate })
                    .Where(assignment => assignment.DueDate <= DateTime.Now)
                    .OrderByDescending(assignment => assignment.DueDate)
                .ToArray()
                ?? throw new Exception("Unable to fetch the assignments of the course!");
        }

        public async Task<Course[]> GetUsersCoursesAsync(int userId)
        {
            var courses = await FetchDataAsync<Course[]>(
                "core_enrol_get_users_courses",
                new Dictionary<string, string> { { "userid", userId.ToString() } });

            return courses?
                .OrderByDescending(c => c.StartDate)
                .ToArray()
                ?? throw new Exception("Unable to fetch the users courses!");
        }

        private async Task<T?> FetchDataAsync<T>(string wsFunction, Dictionary<string, string> parameters, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(_token))
            {
                await AkquireTokenAsync();
            }

            string url = $"webservice/rest/server.php?wsfunction={wsFunction}&moodlewsrestformat=json&wstoken={_token}";
            if (parameters.Any())
            {
                url = $"{url}&{string.Join("&", parameters.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"))}";
            }

            return await _httpClient.GetFromJsonAsync<T>(url, cancellationToken);
        }

        private async Task AkquireTokenAsync()
        {
            string url = $"login/token.php?username={HttpUtility.UrlEncode(_username)}&password={HttpUtility.UrlEncode(_password)}&service=moodle_mobile_app";
            var tokenRespone = await _httpClient.GetFromJsonAsync<GetTokenResponse>(url);
            _token = tokenRespone?.Token ?? throw new Exception("Unable to fetch the token from Moodle!");
        }
    }
}
