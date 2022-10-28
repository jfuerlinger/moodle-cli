using MoodleCli.Core.Services;
using System.CommandLine;

namespace MoodleCli.ConsoleApp.Commands.Courses
{
    internal class CoursesCommand : Command
    {
        public CoursesCommand(IMoodleService moodleService) : base("courses", "Operates on Moodle Coures entitled by the user.")
        {
            AddCommand(new CoursesListCommand(moodleService));
        }
    }
}
