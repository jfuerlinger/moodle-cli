using MoodleCli.Core.Model;
using System.Text.RegularExpressions;

namespace MoodleCli.Core.Infrastructure
{
    public partial class UnitTestResultParser
    {
        [GeneratedRegex(@"(?:Failed|Fehler):[ ]*([0-9]+)?,\s+(?:Passed|erfolgreich):\s*([0-9]+)?,\s+(?:Skipped|übersprungen):\s+([0-9]+),\s+(?:Total|gesamt):\s+([0-9]+),\s+(?:Duration|Dauer):\s+([0-9]+) ms",
            RegexOptions.IgnoreCase)]
        private static partial Regex UnitTestResultRegexExpression();

        public static UnitTestResult ParseResult(string content)
        {
            try
            {
                MatchCollection matches =
                    UnitTestResultRegexExpression()
                        .Matches(content);

                var match = matches.Single();

                return new UnitTestResult(
                    Failed: Convert.ToInt32(match.Groups[1].Value),
                    Succeeded: Convert.ToInt32(match.Groups[2].Value),
                    Skipped: Convert.ToInt32(match.Groups[3].Value),
                    Total: Convert.ToInt32(match.Groups[4].Value),
                    DurationInMilliseconds: Convert.ToInt32(match.Groups[5].Value));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Content='{content}'");
                throw new Exception($"Error parsing the dotnet test result!", e);
            }
        }
    }
}
