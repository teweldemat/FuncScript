using FuncScript.Core;
using System;
using System.IO;
using FuncScript.Model;

namespace FuncScript.Funcs.OS
{
    internal class AppendTextFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;

        public string Symbol => "AppendText";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            if (pars.Length != 2)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"{this.Symbol} function: invalid parameter count. 2 expected, got {pars.Length}");

            var par0 = pars[0];
            var par1 = pars[1];

            if (par0 == null || par1 == null)
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER,
                    $"Function {this.Symbol}: parameters cannot be null");

            if (!(par0 is string) || !(par1 is string))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER,
                    $"Function {this.Symbol}: Type mismatch. Both parameters must be strings");

            var fileName = (string)par0;
            var content = (string)par1;

            try
            {
                var directory = Path.GetDirectoryName(fileName);
                if (!string.IsNullOrEmpty(directory))
                {
                    if (!Directory.Exists(directory))
                        return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER,
                            $"Function {this.Symbol}: directory {directory} doesn't exist");
                }

                // Append the text to the file
                File.AppendAllText(fileName, content);
                Console.WriteLine($"Appended to {fileName}");
                return content;
            }
            catch (Exception ex)
            {
                return new FsError(ex);
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
