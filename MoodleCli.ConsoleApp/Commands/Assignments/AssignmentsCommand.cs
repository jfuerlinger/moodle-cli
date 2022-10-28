using MoodleCli.Core.Services;
using System.CommandLine;

namespace MoodleCli.ConsoleApp.Commands.Assignments
{
    internal class AssignmentsCommand : Command
    {
        public AssignmentsCommand(IMoodleService moodleService) 
            : base("assignments", "Operates on assignments of a Moodle course.")
        {
            AddCommand(new AssignmentsListSubCommand(moodleService));
        }
    }
}
