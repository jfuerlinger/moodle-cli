using MoodleCli.Core.Model.Reponses;
using MoodleCli.Core.Services;
using Spectre.Console;
using System.CommandLine;

namespace MoodleCli.ConsoleApp.Commands.Assignments
{
    internal class AssignmentsListSubCommand : Command
    {
        private readonly Option<int> _courseIdOption;

        public AssignmentsListSubCommand(IMoodleService moodleService) : base("list", "List all the assignments for a Moodle course.")
        {
            _courseIdOption = new Option<int>
                (name: "--courseid",
                description: "The id of a Moodle course.");

            AddOption(_courseIdOption);

            this.SetHandler(async (int courseId) =>
            {
                Assignment[] assignments = await LoadAssignments(moodleService, courseId);

                var table = new Table();

                table.AddColumn("ID");
                table.AddColumn("Name");
                table.AddColumn("DueDate");


                foreach (var assignment in assignments
                                        .OrderByDescending(assignment => assignment.DueDate)
                                        .ThenBy(assignment => assignment.Name))
                {
                    table.AddRow(new string[] { assignment.Id.ToString(), assignment.Name!, assignment.DueDate?.ToString("dd.MM.yyyy")! }); ;
                }

                Console.Clear();
                AnsiConsole.Write(table);

            }, _courseIdOption);
        }

        private static async Task<Assignment[]> LoadAssignments(IMoodleService moodleService, int courseId)
        {
            return await AnsiConsole
                            .Status()
                            .StartAsync("Loading assignments...", async ctx => await moodleService.GetAssignmentsForCourse(courseId));
        }

    }
}
