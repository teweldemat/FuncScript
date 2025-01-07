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

        public object EvaluateList(FsList pars)
        {
            const int MaxParameters = 1; // Moved the constant declaration here
            if (pars.Length != MaxParameters)
                throw new Error.EvaluationTimeException($"{this.Symbol} function: invalid parameter count. {MaxParameters} expected, got {pars.Length}");

            var par0 = pars[0];

            if (par0 == null || !(par0 is string))
                throw new Error.TypeMismatchError($"Function {this.Symbol}. Invalid parameter type, expected a string");

            var filePath = (string)par0;
            return File.Exists(filePath);
        }

        public string ParName(int index)
        {
            return index == 0 ? "file path" : null;
        }
    }
}
