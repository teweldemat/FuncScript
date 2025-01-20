using NUnit.Framework;
using System;
using System.IO;
using System.Diagnostics;
using FsStudio.Server.FileSystem.Exec;

namespace FsStudio.Server.FileSystem.Tests
{
    [TestFixture]
    public class TestCommandLineFullIntegration
    {
        private string _tempFolder;
        private SessionManager _sessionManager;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(_tempFolder);
            _sessionManager = new SessionManager(null);
            _sessionManager.SetRootFolder(_tempFolder);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            if (Directory.Exists(_tempFolder))
                Directory.Delete(_tempFolder, true);
        }

        [Test]
        public void SimpleTest()
        {
            // Create a test file with a simple node
            var fileName = "test";
            _sessionManager.CreateFile("", fileName);
            var session = _sessionManager.CreateOrGetSession(fileName, true);
            session.CreateNode(null,"x","5+3",ExpressionType.FuncScript);
            Console.WriteLine(Environment.CurrentDirectory.ToString());
            // Execute the DLL with command line arguments
            var dll = "../../../../FsStudio.Server.FileSystem/bin/Debug/net8.0/FsStudio.Server.FileSystem.dll";
            var startInfo = new ProcessStartInfo("dotnet", $"{dll} --project:\"{_tempFolder}\" --file:test --exec-node:x")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            var process = Process.Start(startInfo);
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            // Verify the output contains our expected result
            StringAssert.Contains("----start-output----", output);
            StringAssert.Contains("8", output);
            StringAssert.Contains("----end-output----", output);
            Assert.That(process.ExitCode, Is.EqualTo(0));
        }
        
        [Test]
        public void ShellCommandTest()
        {
            // Create a test file with a simple node
            var fileName = "test";
            _sessionManager.CreateFile("", fileName);
            var session = _sessionManager.CreateOrGetSession(fileName, true);
            var exp = 
@"
    {a:'ls' log 'running'};
";
            session.CreateNode(null,"x",exp,ExpressionType.FuncScript);
            Console.WriteLine(Environment.CurrentDirectory.ToString());
            // Execute the DLL with command line arguments
            var dll = "../../../../FsStudio.Server.FileSystem/bin/Debug/net8.0/FsStudio.Server.FileSystem.dll";
            var startInfo = new ProcessStartInfo("dotnet", $"{dll} --project:\"{_tempFolder}\" --file:test --exec-node:x")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false
            };

            var process = Process.Start(startInfo);
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            Console.WriteLine(output);
            // Verify the output contains our expected result
            StringAssert.Contains("----start-output----", output);
            StringAssert.Contains("----end-output----", output);
            var count = output.Split("running").Length - 1;
            Assert.That(count,Is.EqualTo(1));
        }
        
        [Test]
        public void ShellCommandInProcTest()
        {
            // Create a test file with a simple node
            var fileName = "test";
            _sessionManager.CreateFile("", fileName);
            var session = _sessionManager.CreateOrGetSession(fileName, true);
            var exp = "{a:'ls' log 'running'}";
            session.CreateNode(null,"x",exp,ExpressionType.FuncScript);
            Console.WriteLine(Environment.CurrentDirectory.ToString());
            

            var args = new[]
            {
                $"--project:{_tempFolder}",
                $"--file:test",
                $"--exec-node:x",
            };

            using var sw = new StringWriter();
            var originalOut = Console.Out;
            Console.SetOut(sw);

            Program.Main(args);

            Console.SetOut(originalOut);
            var output = sw.ToString();
            // Verify the output contains our expected result
            StringAssert.Contains("----start-output----", output);
            StringAssert.Contains("----end-output----", output);
            var count = output.Split("running").Length - 1;
            Assert.That(count,Is.EqualTo(1));
        }
    }
}