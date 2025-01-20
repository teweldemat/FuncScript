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
    public class ShellResultAccumulator
    {
        public int ExitCode { get; set; }
        public object Output { get; set; }
    }
    public class ShellResult
    {
        public int ExitCode { get; set; }
        public List<ShellMessage> Output { get; set; }
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

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            if (pars.Length < 1 || pars.Length > 3)
                return new FsError(
                    FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"{Symbol} function: invalid parameter count. 1-3 expected, got {pars.Length}"
                );

            var cmdParam = pars[0];
            var reducerParam = pars.Length >= 2 ? pars[1] : null;
            var timeoutParam = pars.Length == 3 ? pars[2] : null;

            if (cmdParam is not string)
                return new FsError(
                    FsError.ERROR_TYPE_INVALID_PARAMETER,
                    $"Function {Symbol}: Type mismatch. First parameter (command) must be string."
                );

            if (reducerParam != null && reducerParam is not IFsFunction)
                return new FsError(
                    FsError.ERROR_TYPE_INVALID_PARAMETER,
                    $"Function {Symbol}: Type mismatch. Second parameter (reducer) must be a function."
                );

            var command = (string)cmdParam;
            int? timeoutMs = null;
            if (timeoutParam != null)
            {
                if (timeoutParam is int i)
                    timeoutMs = i;
                else
                    return new FsError(
                        FsError.ERROR_TYPE_INVALID_PARAMETER,
                        $"Function {Symbol}: Type mismatch. Third parameter (timeout) must be int."
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
                    object accumulator = null;
                    var reducer = reducerParam as IFsFunction;
                    var messages = new ConcurrentBag<(int index, bool error, string msg)>();
                    int counter = 0;
                    var syncLock = new object();
                    bool breakEncountered = false;

                    void ProcessOutput(string data, bool isError)
                    {
                        if (data == null || breakEncountered) return;

                        int myIndex = Interlocked.Increment(ref counter);
                        if (reducer != null)
                        {
                            var msg = new ShellMessage { Msg = data, Error = isError };
                            lock (syncLock)
                            {
                                if (!breakEncountered)
                                {
                                    accumulator = reducer.EvaluateList(context, new ArrayFsList(new object[] { msg, accumulator,myIndex }));

                                    if (accumulator is FsError err && err.ErrorType == FsError.CONTROL_BREAK)
                                    {
                                        accumulator = err.ErrorData;
                                        breakEncountered = true;
                                        try
                                        {
                                            process.Kill();
                                        }
                                        catch { /* Ignore kill exceptions */ }
                                    }
                                }
                            }
                        }
                        else
                        {
                            messages.Add((myIndex, isError, data));
                        }
                    }

                    process.OutputDataReceived += (_, args) => ProcessOutput(args.Data, false);
                    process.ErrorDataReceived += (_, args) => ProcessOutput(args.Data, true);

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

                    if (reducer != null)
                    {
                        var result = new ShellResultAccumulator
                        {
                            ExitCode = process.ExitCode,
                            Output = accumulator
                        };
                        return Helpers.NormalizeDataType(result);
                    }
                    else
                    {
                        var result = new ShellResult
                        {
                            ExitCode = process.ExitCode,
                            Output = messages.OrderBy(x => x.index)
                                .Select(o => new ShellMessage { Msg = o.msg, Error = o.error })
                                .ToList()
                        };

                        return Helpers.NormalizeDataType(result);
                    }
                }
            }
            catch (Exception ex)
            {
                return new FsError(ex);
            }
        }
    }
}
