namespace MoodleCli.Core.Model
{
    public record UnitTestResult(int Succeeded, int Failed, int Skipped, int Total, int DurationInMilliseconds);
}
