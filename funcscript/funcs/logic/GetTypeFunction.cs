using FuncScript.Core;
using System;
using FuncScript.Model;

namespace FuncScript.Funcs.OS
{
    internal class GetTypeFunction : IFsFunction
    {
        private const int MaxParsCountValue = 1;

        public CallType CallType => CallType.Prefix;

        public string Symbol => "type";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != MaxParsCountValue)
                throw new Error.EvaluationTimeException($"{this.Symbol} function: invalid parameter count. {MaxParsCountValue} expected, got {pars.Length}");

            var par0 = pars[0];
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
