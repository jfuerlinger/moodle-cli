using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace MoodleCli.Core.Services
{
    public class CompilerService : ICompilerService
    {
        public async Task<(int, int, List<string>)> CompileAsync(Stream stream)
        {
            return Compile(await new StreamReader(stream).ReadToEndAsync());
        }

        private static (int, int, List<string>) Compile(string sourceCode)
        {
            using var peStream = new MemoryStream();
            var result = GenerateCode(sourceCode).Emit(peStream);

            List<string> errorMessages = new();
            if (!result.Success)
            {
                Console.WriteLine("Compilation done with error.");

                var failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

                foreach (var diagnostic in failures)
                {
                    errorMessages.Add($"{diagnostic.Id}: {diagnostic.GetMessage()}");
                }

            }

            int cntErrors = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error).Count();
            int cntWarnings = result.Diagnostics.Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Warning).Count();

            return (cntErrors, cntWarnings, errorMessages);
        }

        private static CSharpCompilation GenerateCode(string sourceCode)
        {
            var codeString = SourceText.From(sourceCode);
            var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);

            var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(codeString, options);
            
            var metadataReferences = GetGlobalReferences();
            metadataReferences.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
            metadataReferences.Add(MetadataReference.CreateFromFile(typeof(Console).Assembly.Location));
            metadataReferences.Add(MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location));
            metadataReferences.Add(MetadataReference.CreateFromFile(typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly.Location));

            return CSharpCompilation.Create("Program.dll",
                new[] { parsedSyntaxTree },
                references: metadataReferences,
                options: new CSharpCompilationOptions(OutputKind.ConsoleApplication,
                    optimizationLevel: OptimizationLevel.Release,
                    assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
        }

        private static List<MetadataReference> GetGlobalReferences()
        {        
            //The location of the .NET assemblies
            var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
            
            List<MetadataReference> returnList = new()
            {
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath!, "mscorlib.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath!, "System.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath!, "System.Core.dll")),
                MetadataReference.CreateFromFile(Path.Combine(assemblyPath!, "System.Runtime.dll"))
            };

            return returnList;
        }
    }
}
