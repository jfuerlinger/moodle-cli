using MoodleCli.Core.Services;

namespace MoodleCli.Core.Test
{
    public class CompilerServiceTests
    {
        [Fact]
        public async Task Test_CompileValidCSharpCode_ShouldBeSuccessfull()
        {
            ICompilerService service = new CompilerService();
            Stream codeStream = GenerateStreamFromString("""
                        System.Console.WriteLine("Hello World");
                """);

            (int CntErrors, int CntWarnings, List<string> ErrorMessages) = await service.CompileAsync(codeStream);

            Assert.Equal(0, ErrorMessages.Count!);
            Assert.Equal(0, CntErrors);
            Assert.Equal(0, CntWarnings);
        }

        [Fact]
        public async Task Test_CompileInvalidCSharpCode_ShouldBeFailing()
        {
            ICompilerService service = new CompilerService();
            Stream codeStream = GenerateStreamFromString("""
                        System.Console.WriteLine("Hello World")
                """);

            (int CntErrors, int CntWarnings, List<string> ErrorMessages) = await service.CompileAsync(codeStream);

            Assert.Equal(1, ErrorMessages.Count!);
            Assert.Equal(1, CntErrors);
            Assert.Equal(0, CntWarnings);
        }

        private static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}