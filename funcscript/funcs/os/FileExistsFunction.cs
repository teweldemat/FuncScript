using FuncScript.Core;
using System;
using System.IO;
using FuncScript.Model;

namespace FuncScript.Funcs.OS
{
    internal class FileExistsFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;

        public string Symbol => "fileexists";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            const int MaxParameters = 1; // Moved the constant declaration here
            if (pars.Length != MaxParameters)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol} function: invalid parameter count. {MaxParameters} expected, got {pars.Length}");

            var par0 = pars[0];

            if (par0 == null || !(par0 is string))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"Function {this.Symbol}. Invalid parameter type, expected a string");

            var filePath = (string)par0;
            return File.Exists(filePath);
        }

        public string ParName(int index)
        {
            return index == 0 ? "file path" : null;
        }
    }
}
