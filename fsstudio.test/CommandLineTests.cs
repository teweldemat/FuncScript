using System.Text.Json;

namespace FsStudio.Server.FileSystem.Tests;

using NUnit.Framework;
using System;
using System.IO;

[TestFixture]
public class CommandLineTests
{
    [Test]
    public void TestNrlaisGenerate()
    {
        var project = "/Users/teweldema.tegegne/project/nrlais/fs/";
        var file = "nrlais-report";
        var nodeInst = "control.instruction";
        var instTextValue = "Number of parcels by kebele";
        var nodeModel = "control.model";
        var modelTextValue = "gpt-4o-mini";
        var execNode = "control.generate";

        var args = new[]
        {
            $"--project:{project}",
            $"--file:{file}",
            $"--exec-node:{execNode}",
            $"--set-node-text:{nodeInst}:{instTextValue}",
            $"--set-node-text:{nodeModel}:{modelTextValue}"
        };

        using var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        FsStudio.Server.FileSystem.Program.Main(args);

        Console.SetOut(originalOut);
        var output = sw.ToString();

        var startMarker = "----start-output----";
        var endMarker = "----end-output----";
        Assert.That(output, Does.Contain(startMarker));
        Assert.That(output, Does.Contain(endMarker));
        var startIndex = output.IndexOf(startMarker, StringComparison.Ordinal);
        var endIndex = output.IndexOf(endMarker, StringComparison.Ordinal);

        var jsonString = output.Substring(startIndex + startMarker.Length, endIndex - (startIndex + startMarker.Length)).Trim();
        Assert.DoesNotThrow(() => JsonDocument.Parse(jsonString));

    }
}