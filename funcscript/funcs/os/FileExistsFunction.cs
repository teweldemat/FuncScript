using funcscript.core;
using System;
using System.IO;
using funcscript.model;

namespace funcscript.funcs.os
{
    internal class FileExistsFunction : IFsFunction
    {
        private const int MaxParameters = 1;

        public CallType CallType => CallType.Prefix;

        public string Symbol => "fileexists";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != MaxParameters)
                throw new error.EvaluationTimeException($"{this.Symbol} function: invalid parameter count. {MaxParameters} expected, got {pars.Length}");

            var par0 = pars[0];

            if (par0 == null || !(par0 is string))
                throw new error.TypeMismatchError($"Function {this.Symbol}. Invalid parameter type, expected a string");

            var filePath = (string)par0;
            return File.Exists(filePath);
        }

        public string ParName(int index)
        {
            return index == 0 ? "file path" : null;
        }
    }
}