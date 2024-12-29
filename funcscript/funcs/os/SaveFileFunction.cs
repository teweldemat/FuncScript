using funcscript.core;
using System;
using System.IO;
using funcscript.model;

namespace funcscript.funcs.os
{
    internal class SaveFileFunction : IFsFunction
    {
        private const int MaxParameters = 2;
        public CallType CallType => CallType.Prefix;

        public string Symbol => "SaveFile";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != MaxParameters)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"{this.Symbol} function: invalid parameter count. {MaxParameters} expected, got {pars.Length}");

            var par0 = pars[0];
            var par1 = pars[1];

            if (par0 == null || par1 == null)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"Function {this.Symbol}: parameters cannot be null");

            if (!(par0 is string) || !(par1 is string))
                return new FsError(FsError.ERROR_TYPE_MISMATCH,
                    $"Function {this.Symbol}: Type mismatch. Both parameters must be strings");

            var fileName = (string)par0;
            var content = (string)par1;

            try
            {
                var directory = Path.GetDirectoryName(fileName);
                if (!string.IsNullOrEmpty(directory))
                    Directory.CreateDirectory(directory);

                File.WriteAllText(fileName, content);
                Console.WriteLine($"Saved {fileName}");
                return content;
            }
            catch (Exception ex)
            {
                return new FsError(FsError.ERROR_TYPE_EVALUATION,
                    $"Function {this.Symbol}: Failed to save file '{fileName}'. Error: {ex.Message}");
            }
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "file name",
                1 => "content",
                _ => null
            };
        }
    }
}