using CliWrap;
using CliWrap.Buffered;
using MoodleCli.Core.Infrastructure;
using MoodleCli.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MoodleCli.Core.Services
{
    public class UnitTestService : IUnitTestService
    {
        public async Task<UnitTestResult> RunTestsAsync(string path)
        {
            try
            {

                var result = await Cli.Wrap("dotnet")
                    .WithArguments("test")
                    .WithWorkingDirectory(path)
                    .WithValidation(CommandResultValidation.None)
                    .ExecuteBufferedAsync();

                // Result contains:
                // -- result.StandardOutput  (string)
                // -- result.StandardError   (string)
                // -- result.ExitCode        (int)
                // -- result.StartTime       (DateTimeOffset)
                // -- result.ExitTime        (DateTimeOffset)
                // -- result.RunTime         (TimeSpan)

                return UnitTestResultParser.ParseResult(result.StandardOutput);
            } catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message);
                throw new Exception("There was an error executing the command!", ex);
            }
            
        }

        
    }
}
