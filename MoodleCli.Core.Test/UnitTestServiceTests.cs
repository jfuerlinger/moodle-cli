using MoodleCli.Core.Infrastructure;
using MoodleCli.Core.Services;

namespace MoodleCli.Core.Test
{
    public class UnitTestServiceTests
    {
        [Fact]
        public void UnitTestResultParser_ParseGermanExecutionResult_ShouldBeSuccessfull()
        {
            string dotnetTestOutput = """
                                  Wiederherzustellende Projekte werden ermittelt...
                  Alle Projekte sind für die Wiederherstellung auf dem neuesten Stand.
                  TestProject -> C:\Projekte\HTL\TestProject\bin\Debug\net7.0\TestProject.dll
                Testlauf für "C:\Projekte\HTL\TestProject\bin\Debug\net7.0\TestProject.dll" (.NETCoreApp,Version=v7.0)
                Microsoft (R) Testausführungs-Befehlszeilentool Version 17.4.0 (x64)
                Copyright (c) Microsoft Corporation. Alle Rechte vorbehalten.

                Die Testausführung wird gestartet, bitte warten...
                Insgesamt 1 Testdateien stimmten mit dem angegebenen Muster überein.
                [xUnit.net 00:00:00.38]     TestProject.UnitTest1.Test2 [FAIL]
                  Fehler TestProject.UnitTest1.Test2 [1 ms]
                  Fehlermeldung:
                   Assert.True() Failure
                Expected: True
                Actual:   False
                  Stapelverfolgung:
                     at TestProject.UnitTest1.Test2() in C:\Projekte\HTL\TestProject\UnitTest1.cs:line 14
                   at System.RuntimeMethodHandle.InvokeMethod(Object target, Void** arguments, Signature sig, Boolean isConstructor)
                   at System.Reflection.MethodInvoker.Invoke(Object obj, IntPtr* args, BindingFlags invokeAttr)

                Fehler!      : Fehler:     1,  erfolgreich:     1, übersprungen:     0, gesamt:     2, Dauer: 2 ms - TestProject.dll (net7.0)
                """;

            var result = UnitTestResultParser.ParseResult(dotnetTestOutput);

            Assert.Equal(1, result.Failed);
            Assert.Equal(1, result.Succeeded);
            Assert.Equal(0, result.Skipped);
            Assert.Equal(2, result.Total);
            Assert.Equal(2, result.DurationInMilliseconds);
        }

        [Fact]
        public void UnitTestResultParser_ParseEnglishExecutionResult_ShouldBeSuccessfull()
        {
            string dotnetTestOutput = """
                                    Determining projects to restore...
                  All projects are up-to-date for restore.
                  TestProject -> C:\Projekte\HTL\TestProject\bin\Debug\net7.0\TestProject.dll
                Test run for C:\Projekte\HTL\TestProject\bin\Debug\net7.0\TestProject.dll (.NETCoreApp,Version=v7.0)
                Microsoft (R) Test Execution Command Line Tool Version 17.4.0 (x64)
                Copyright (c) Microsoft Corporation.  All rights reserved.

                Starting test execution, please wait...
                A total of 1 test files matched the specified pattern.
                  Failed TestProject.UnitTest1.Test2 [1 ms]
                  Error Message:
                   Assert.True() Failure
                Expected: True
                Actual:   False
                  Stack Trace:
                     at TestProject.UnitTest1.Test2() in C:\Projekte\HTL\TestProject\UnitTest1.cs:line 14
                   at System.RuntimeMethodHandle.InvokeMethod(Object target, Void** arguments, Signature sig, Boolean isConstructor)
                   at System.Reflection.MethodInvoker.Invoke(Object obj, IntPtr* args, BindingFlags invokeAttr)

                Failed!  - Failed:     1, Passed:     1, Skipped:     0, Total:     2, Duration: 3 ms - TestProject.dll (net7.0)
                
                """;

            var result = UnitTestResultParser.ParseResult(dotnetTestOutput);

            Assert.Equal(1, result.Failed);
            Assert.Equal(1, result.Succeeded);
            Assert.Equal(0, result.Skipped);
            Assert.Equal(2, result.Total);
            Assert.Equal(3, result.DurationInMilliseconds);
        }
    }
}