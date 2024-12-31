using funcscript.core;
using funcscript.model;
using System;
using System.Diagnostics;
using System.Text;

namespace funcscript.funcs.os
{
    internal class ShellFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;
        public string Symbol => "shell";

        private const int MaxParameters = 2;

        public object EvaluateList(FsList pars)
        {
            if (pars.Length < 1 || pars.Length > MaxParameters)
                return new FsError(
                    FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"{Symbol} function: invalid parameter count. 1 or {MaxParameters} expected, got {pars.Length}"
                );

            var cmdParam = pars[0];
            var timeoutParam = pars.Length == 2 ? pars[1] : null;

            if (!(cmdParam is string))
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
                    var stdoutBuilder = new StringBuilder();
                    process.Start();
                    
                    stdoutBuilder.AppendLine(process.StandardOutput.ReadToEnd());
                    stdoutBuilder.AppendLine(process.StandardError.ReadToEnd());

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

                    return stdoutBuilder.ToString();
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