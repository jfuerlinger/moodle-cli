using MoodleCli.Core.Model;
using MoodleCli.Core.Services;
using Spectre.Console;
using System.CommandLine;

namespace MoodleCli.ConsoleApp.Commands.Users
{
    internal class UsersMeSubCommand : Command
    {
        private readonly IMoodleService _moodleService;

        public UsersMeSubCommand(IMoodleService moodleService) : base("me", "Show infos to the current user.")
        {
            _moodleService = moodleService;

            this.SetHandler(Execute);
        }

        private async Task Execute()
        {
            User currentUser = await GetCurrentMoodleUser(_moodleService);
            if (currentUser == null)
            {
                throw new Exception("Can't retrieve infos to the current user!");
            }

            var usersCourses = await LoadCourses(_moodleService, currentUser);

            AnsiConsole.Markup($"Hello [red]{currentUser.FirstName} {currentUser.LastName}[/]!");
            AnsiConsole.Markup($"Your id is [red]{currentUser.Id}[/] and you're entitled in [red]{usersCourses.Count()} courses[/]");
            
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
