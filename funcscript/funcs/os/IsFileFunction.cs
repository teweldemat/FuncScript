using FuncScript.Core;
using System;
using System.IO;
using FuncScript.Model;

namespace FuncScript.Funcs.OS
{
    internal class IsFileFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;

        public string Symbol => "isfile";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != 1)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol} function: invalid parameter count. 1 expected, got {pars.Length}");

            var par0 = pars[0];
            if (par0 == null || !(par0 is string))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"Function {this.Symbol}. Invalid parameter type, expected a string");

            var path = (string)par0;
            return File.Exists(path) && !Directory.Exists(path);
        }

        public string ParName(int index)
        {
            return index == 0 ? "file path" : null;
        }
    }
}
