using funcscript.core;
using System;
using System.IO;

namespace funcscript.funcs.os
{
    internal class IsFileFunction : IFsFunction
    {
        private const int ExpectedParameterCount = 1;

        public CallType CallType => CallType.Prefix;

        public string Symbol => "isfile";

        public object Evaluate(IFsDataProvider parent, IParameterList pars)
        {
            if (pars.Count != ExpectedParameterCount)
                throw new error.EvaluationTimeException($"{this.Symbol} function: invalid parameter count. {ExpectedParameterCount} expected, got {pars.Count}");

            var par0 = pars.GetParameter(parent, 0);
            if (par0 == null || !(par0 is string))
                throw new error.TypeMismatchError($"Function {this.Symbol}. Invalid parameter type, expected a string");

            var path = (string)par0;
            return File.Exists(path) && !Directory.Exists(path);
        }

        public string ParName(int index)
        {
            return index == 0 ? "file path" : null;
        }
    }
}