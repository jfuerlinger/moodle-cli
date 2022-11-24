using MoodleCli.Core.Model;
using MoodleCli.Core.Services;
using Spectre.Console;
using System.CommandLine;
using System.Text;

namespace MoodleCli.ConsoleApp.Commands.Tests
{
    internal class TestsRunSubCommand : Command
    {
        private readonly IUnitTestService _unitTestService;
        private readonly Option<string> _pathOption;

        public TestsRunSubCommand(IUnitTestService unitTestService) : base("run", "Executes unit tests in directory.")
        {
            _unitTestService = unitTestService;

            _pathOption = new Option<string>
                (name: "--path",
                description: "The path to the unit test project.");

            AddOption(_pathOption);

            this.SetHandler(Execute, _pathOption);
        }


        private async Task Execute(string path)
        {
            var result = await AnsiConsole
                            .Status()
                            .StartAsync("Execute tests...", async ctx => await _unitTestService.RunTestsAsync(path));


            var table = new Table();

            table.AddColumn("Total");
            table.AddColumn("Succeeded");
            table.AddColumn("Skipped");
            table.AddColumn("Failed");
            table.AddColumn("Duration");

            table.AddRow($"[yellow]{result.Total}[/]",
                    $"[green]{result.Succeeded}[/]",
                    $"[yellow]{result.Skipped}[/]",
                    $"[red]{result.Failed}[/]",
                    $"[blue]{result.DurationInMilliseconds} ms[/]");

            table.Columns.ToList().ForEach(column => { column.Alignment = Justify.Center; });

            AnsiConsole.Write(table);
        }
    }
}
