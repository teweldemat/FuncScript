using FuncScript.Core;
using FuncScript.Model;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

namespace FuncScript.Funcs.OS
{
    public class ShellResult
    {
        public int ExitCode { get; set; }
        public List<ShellMessage> Output { get; set; } = new List<ShellMessage>();
    }

    public class ShellMessage
    {
        public string Msg { get; set; }
        public bool Error { get; set; }
    }

    internal class ShellFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;
        public string Symbol => "shell";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length < 1 || pars.Length > 2)
                return new FsError(
                    FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"{Symbol} function: invalid parameter count. 1 or 2 expected, got {pars.Length}"
                );

            var cmdParam = pars[0];
            var timeoutParam = pars.Length == 2 ? pars[1] : null;

            if (cmdParam is not string)
                return new FsError(
                    FsError.ERROR_TYPE_MISMATCH,
                    $"Function {Symbol}: Type mismatch. First parameter (command) must be string."
                );

            var command = (string)cmdParam;
            int? timeoutMs = null;
            if (timeoutParam != null)
            {
                if (timeoutParam is int i)
                    timeoutMs = i;
                else
                    return new FsError(
                        FsError.ERROR_TYPE_MISMATCH,
                        $"Function {Symbol}: Type mismatch. Second parameter (timeout) must be int."
                    );
            }

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = Environment.OSVersion.Platform == PlatformID.Win32NT ? "cmd.exe" : "/bin/bash",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                psi.Arguments = Environment.OSVersion.Platform == PlatformID.Win32NT 
                    ? $"/c {command}" 
                    : $"-c \"{command}\"";

                using (var process = new Process { StartInfo = psi })
                {
                    var messages = new ConcurrentBag<(int index, bool error, string msg)>();
                    int counter = 0;

                    process.OutputDataReceived += (_, args) =>
                    {
                        if (args.Data != null)
                        {
                            int myIndex = System.Threading.Interlocked.Increment(ref counter);
                            messages.Add((myIndex, false, args.Data));
                        }
                    };

                    process.ErrorDataReceived += (_, args) =>
                    {
                        if (args.Data != null)
                        {
                            int myIndex = System.Threading.Interlocked.Increment(ref counter);
                            messages.Add((myIndex, true, args.Data));
                        }
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    if (timeoutMs.HasValue)
                    {
                        if (!process.WaitForExit(timeoutMs.Value))
                        {
                            process.Kill();
                            return new FsError(
                                FsError.ERROR_TYPE_EVALUATION,
                                $"Function {Symbol}: Command timed out after {timeoutMs.Value} ms and was terminated."
                            );
                        }
                    }
                    else
                    {
                        process.WaitForExit();
                    }

                    var exitCode = process.ExitCode;
                    var ordered = messages.OrderBy(x => x.index);

                    var result = new ShellResult
                    {
                        ExitCode = exitCode,
                        Output = ordered
                            .Select(o => new ShellMessage { Msg = o.msg, Error = o.error })
                            .ToList()
                    };

                    return FuncScript.NormalizeDataType(result);
                }
            }
            catch (Exception ex)
            {
                return new FsError(
                    FsError.ERROR_TYPE_EVALUATION,
                    $"Function {Symbol}: Failed to execute shell command. Error: {ex.Message}"
                );
            }
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "command",
                1 => "timeout (ms)",
                _ => null
            };
        }
    }
}
