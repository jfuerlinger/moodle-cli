namespace MoodleCli.Core.Services
{
    public interface ICompilerService
    {
        Task<(int, int, List<string>)> CompileAsync(Stream stream);
    }
}