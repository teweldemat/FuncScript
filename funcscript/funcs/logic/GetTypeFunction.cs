using funcscript.core;
using System;

namespace funcscript.funcs.os
{
    internal class GetTypeFunction : IFsFunction
    {
        private const int MaxParsCountValue = 1;

        public CallType CallType => CallType.Prefix;

        public string Symbol => "type";

        public object Evaluate(IFsDataProvider parent, IParameterList pars)
        {
            if (pars.Count != MaxParsCountValue)
                throw new error.EvaluationTimeException($"{this.Symbol} function: invalid parameter count. {MaxParsCountValue} expected, got {pars.Count}");

            var par0 = pars.GetParameter(parent, 0);
            if (par0 == null)
                return "null";

            // Get the data type using FuncScript.GetFsDataType and return its string representation
            var dataType = FuncScript.GetFsDataType(par0);
            return dataType.ToString();
        }

        public string ParName(int index)
        {
            return index == 0 ? "value" : null;
        }
    }
}