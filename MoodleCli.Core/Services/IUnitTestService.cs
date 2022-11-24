using MoodleCli.Core.Model;

namespace MoodleCli.Core.Services
{
    public interface IUnitTestService
    {
        Task<UnitTestResult> RunTestsAsync(string path);
    }
}