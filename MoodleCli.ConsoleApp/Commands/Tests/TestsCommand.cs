using MoodleCli.Core.Services;
using System.CommandLine;

namespace MoodleCli.ConsoleApp.Commands.Tests
{
    internal class TestsCommand : Command
    {
        public TestsCommand(IUnitTestService unitTestService) : base("tests", "Provides unit test functionality.")
        {
            AddCommand(new TestsRunSubCommand(unitTestService));
        }
    }
}
