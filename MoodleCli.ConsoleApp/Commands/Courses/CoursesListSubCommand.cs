using MoodleCli.Core.Model;
using MoodleCli.Core.Services;
using Spectre.Console;
using System.CommandLine;

namespace MoodleCli.ConsoleApp.Commands.Courses
{
    internal class CoursesListSubCommand : Command
    {
        public CoursesListSubCommand(IMoodleService moodleService) : base("list", "List all the Moodle courses entitled by the user.")
        {
            this.SetHandler(async () =>
             {
                 User currentUser = await GetCurrentMoodleUser(moodleService);
                 Course[] courses = await LoadCourses(moodleService, currentUser);
                 var table = new Table();

                 table.AddColumn("ID");
                 table.AddColumn("Name");


                 foreach (var course in courses
                                         .OrderByDescending(entry => entry.StartDate)
                                         .ThenBy(course => course.ShortName))
                 {
                     table.AddRow(new string[] { course.Id.ToString(), course.ShortName! });
                 }

                 AnsiConsole.Write(table);
             });
        }

        private static async Task<User> GetCurrentMoodleUser(IMoodleService moodleService)
        {
            return await AnsiConsole
                .Status()
                .StartAsync("Loading user data...",
                async ctx => await moodleService.GetCurrentUsersInfos() ?? throw new Exception("There was a problem loading the user details"));
        }

        private static async Task<Course[]> LoadCourses(IMoodleService moodleService, User currentUser)
        {
            return await AnsiConsole
                .Status()
                .StartAsync("Loading courses...", async ctx => await moodleService.GetUsersCoursesAsync(currentUser.Id));
        }
    }
}
