using MoodleCli.ConsoleApp.Commands.Assignments;
using MoodleCli.ConsoleApp.Commands.Courses;
using MoodleCli.ConsoleApp.Commands.Users;
using MoodleCli.Core.Model;
using MoodleCli.Core.Model.Reponses;
using MoodleCli.Core.Services;
using Spectre.Console;
using System.CommandLine;

namespace MoodleCli.ConsoleApp.Commands
{
    internal class MoodleCliRootCommand : RootCommand
    {
        private readonly IMoodleService _moodleService;
        private readonly ICompilerService _compilerService;

        public MoodleCliRootCommand(
            IMoodleService moodleService,
            ICompilerService compilerService) : base("Moodle CLI (by Josef Fürlinger)")
        {
            _moodleService = moodleService;
            _compilerService = compilerService;

            this.SetHandler(Execute);

            AddCommand(new CoursesCommand(moodleService));
            AddCommand(new AssignmentsCommand(moodleService));
            AddCommand(new UsersCommand(moodleService));
        }

        private async Task Execute()
        {
            User currentUser = await GetCurrentMoodleUser(_moodleService);
            Course[] courses = await LoadCourses(_moodleService, currentUser);

            Console.Clear();
            int courseId = LetUserChooseFromCourses(courses);
            Course selectedCourse = courses.Single(entry => entry.Id == courseId);
            AnsiConsole.Write(new Markup($"You choose the course [green]{selectedCourse.FullName}[/]!"));

            Console.Clear();
            Assignment[] assignments = await LoadAssignments(_moodleService, selectedCourse.Id);
            int assignmentId = LetUserChooseFromAssignments(assignments);

            Assignment selectedAssignment = assignments.Single(entry => entry.Id == assignmentId);
            AnsiConsole.Write(new Markup($"You choose the assignment [green]{selectedAssignment.Name}[/]!"));

            Console.Clear();
            SubmissionFile[] submissions = await LoadSubmissions(_moodleService, selectedAssignment);
            AnsiConsole.Write(new Markup($"Found [green]{submissions.Length} submissions[/]!"));

            Console.Clear();
            (SubmissionFile, Stream)[] downloadedFiles = await DownloadSubmissionFiles(_moodleService, submissions);

            Console.Clear();
            AnsiConsole.Write(await GenerateResultTableAsync(_compilerService, _moodleService, downloadedFiles));
        }

        private async static Task<Table> GenerateResultTableAsync(
            ICompilerService compilerService,
            IMoodleService moodleService,
            (SubmissionFile, Stream)[] downloadedFiles)
        {
            return await AnsiConsole
                 .Status()
                 .StartAsync("Compiling the files ...",
                 async ctx =>
                 {
                     await MapUserIdsToUserNamesAsync(moodleService, downloadedFiles);

                     var table = new Table();

                     table.AddColumn("Schüler");
                     table.AddColumn("Dateiname");
                     table.AddColumn(new TableColumn("Größe").Centered());
                     table.AddColumn(new TableColumn("Fehler").Centered());
                     table.AddColumn(new TableColumn("Warnungen").Centered());

                     foreach (var submission in downloadedFiles.OrderBy(entry => entry.Item1.UserFullName))
                     {
                         (int CntErrors, int CntWarnings, List<string> ErrorMessages) = await compilerService.CompileAsync(submission.Item2);

                         table.AddRow(
                               $"[blue]{GetPupilIdentity(submission.Item1)}[/]"
                             , $"{submission.Item1.Filename}"
                             , $"{submission.Item1.Size} B"
                             , $"{GetColoredMarkupForNumber(CntErrors, "red")}"
                             , $"{GetColoredMarkupForNumber(CntWarnings, "yellow")}");
                     }

                     return table;

                     static string GetPupilIdentity(SubmissionFile submissionFile)
                        => !string.IsNullOrEmpty(submissionFile.UserFullName) ?
                                submissionFile.UserFullName :
                                submissionFile.UserId.ToString();

                     static string GetColoredMarkupForNumber(int number, string color)
                     {
                         if (number == 0)
                         {
                             return $"[green]{number}[/]";
                         }
                         else
                         {
                             return $"[{color}]{number}[/]";
                         }
                     }
                 });
        }

        private async static Task MapUserIdsToUserNamesAsync(
            IMoodleService moodleService,
            (SubmissionFile, Stream)[] downloadedFiles)
        {
            var userIds = downloadedFiles
                                    .Select(download => download.Item1.UserId)
                                    .Distinct()
                                    .ToArray();

            var userDetails = (await moodleService.GetUserDetailsByIdsAsync(userIds))
                                   .ToDictionary(userDetail => userDetail.Id, userDetail => userDetail.FullName);

            foreach (var file in downloadedFiles)
            {
                if (userDetails.ContainsKey(file.Item1.UserId))
                {
                    file.Item1.UserFullName = userDetails[file.Item1.UserId];
                }
            }
        }

        private static async Task<(SubmissionFile, Stream)[]> DownloadSubmissionFiles(IMoodleService moodleService, SubmissionFile[] submissions)
        {
            return await AnsiConsole
                .Status()
                .StartAsync("Downloading the files ...",
                async ctx => await DownloadSubmissionFilesAsync(moodleService, submissions));
        }

        private static async Task<(SubmissionFile, Stream)[]> DownloadSubmissionFilesAsync(IMoodleService moodleService, SubmissionFile[] submissions)
        {
            List<Task<(SubmissionFile, Stream)>> downloads = new List<Task<(SubmissionFile, Stream)>>();
            foreach (var submission in submissions)
            {
                downloads.Add(moodleService.DownloadSubmissionFileAsync(submission));
            }

            return await Task.WhenAll(downloads.ToArray());
        }

        private static async Task<SubmissionFile[]> LoadSubmissions(IMoodleService moodleService, Assignment selectedAssignment)
        {
            return await AnsiConsole
                .Status()
                .StartAsync("Loading submissions...", async ctx => await moodleService.GetSubmissionsForAssignmentAsync(selectedAssignment.Id));
        }

        private static int LetUserChooseFromAssignments(Assignment[] assignments)
        {
            return AnsiConsole.Prompt(
                new SelectionPrompt<int>() { Converter = (int id) => assignments.Single(assigment => assigment.Id == id).Name! }
                    .Title("Which [green]assignment[/] do you want to choose?")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more assignments)[/]")
                    .AddChoices(assignments.Select(entry => entry.Id)));
        }

        private static async Task<Assignment[]> LoadAssignments(IMoodleService moodleService, int courseId)
        {
            return await AnsiConsole
                            .Status()
                            .StartAsync("Loading assignments...", async ctx => await moodleService.GetAssignmentsForCourse(courseId));
        }

        private static int LetUserChooseFromCourses(Course[] courses)
        {
            return AnsiConsole.Prompt(
                            new SelectionPrompt<int>() { Converter = (int courseId) => courses.Single(course => course.Id == courseId).ShortName! }
                                .Title("Which [green]course[/] do you want to choose?")
                                .PageSize(10)
                                .MoreChoicesText("[grey](Move up and down to reveal more courses)[/]")
                                .AddChoices(courses.Select(entry => entry.Id)));
        }

        private static async Task<Course[]> LoadCourses(IMoodleService moodleService, User currentUser)
        {
            return await AnsiConsole
                .Status()
                .StartAsync("Loading courses...", async ctx => await moodleService.GetUsersCoursesAsync(currentUser.Id));
        }

        private static async Task<User> GetCurrentMoodleUser(IMoodleService moodleService)
        {
            return await AnsiConsole
                .Status()
                .StartAsync("Loading user data...",
                async ctx => await moodleService.GetCurrentUsersInfos() ?? throw new Exception("There was a problem loading the user details"));
        }
    }
}
