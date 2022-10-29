using MoodleCli.ConsoleApp.Commands;
using MoodleCli.Core.Services;
using System.CommandLine;

namespace MoodleCli.ConsoleApp
{
    internal class Program
    {
        public static string? MoodleUser => Environment.GetEnvironmentVariable("MOODLE_USER");
        public static string? MoodlePassword => Environment.GetEnvironmentVariable("MOODLE_PASSWORD");

        static async Task<int> Main(string[] args)
        {
            using IMoodleService moodleService = new MoodleService(
             username: MoodleUser ?? throw new ArgumentNullException("MOODLE_USER"),
             password: MoodlePassword ?? throw new ArgumentNullException("MOODLE_PASSWORD"));
            ICompilerService compilerService = new CompilerService();

            return await new MoodleCliRootCommand(moodleService, compilerService).InvokeAsync(args);
        }
    }
}