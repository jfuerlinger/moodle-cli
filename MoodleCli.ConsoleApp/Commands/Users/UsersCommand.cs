using MoodleCli.Core.Services;
using System.CommandLine;

namespace MoodleCli.ConsoleApp.Commands.Users
{
    internal class UsersCommand : Command
    {
        public UsersCommand(IMoodleService moodleService) : base("users", "Provides user specific functionality.")
        {
            AddCommand(new UsersMeSubCommand(moodleService));
        }
    }
}
