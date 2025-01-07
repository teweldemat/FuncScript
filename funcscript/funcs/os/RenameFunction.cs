using funcscript.core;
using System;
using System.IO;
using funcscript.model;

namespace funcscript.funcs.os
{
    internal class RenameFunction : IFsFunction
    {
        private const int MaxParameters = 2;
        public CallType CallType => CallType.Prefix;
        public string Symbol => "RenameFile";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != MaxParameters)
                return new FsError(
                    FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"{Symbol} function: invalid parameter count. {MaxParameters} expected, got {pars.Length}");

            var par0 = pars[0];
            var par1 = pars[1];

            if (par0 == null || par1 == null)
                return new FsError(
                    FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"Function {Symbol}: parameters cannot be null");

            if (!(par0 is string) || !(par1 is string))
                return new FsError(
                    FsError.ERROR_TYPE_MISMATCH,
                    $"Function {Symbol}: Type mismatch. Both parameters must be strings");

            var oldPath = (string)par0;
            var newName = (string)par1;

            try
            {
                if (!File.Exists(oldPath) && !Directory.Exists(oldPath))
                    return new FsError(
                        FsError.ERROR_TYPE_EVALUATION,
                        $"Function {Symbol}: '{oldPath}' does not exist.");

                var directory = Path.GetDirectoryName(oldPath);
                var newPath = Path.Combine(directory ?? "", newName);
                if (oldPath == newPath)
                    return newPath;
                if (File.Exists(oldPath))
                {
                    File.Move(oldPath, newPath);
                    Console.WriteLine($"Renamed file to '{newPath}'");
                }
                else
                {
                    Directory.Move(oldPath, newPath);
                    Console.WriteLine($"Renamed directory to '{newPath}'");
                }

                return newPath;
            }
            catch (Exception ex)
            {
                return new FsError(
                    FsError.ERROR_TYPE_EVALUATION,
                    $"Function {Symbol}: Failed to rename '{oldPath}'. Error: {ex.Message}");
            }
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "oldPath",
                1 => "newName",
                _ => null
            };
        }
    }
}