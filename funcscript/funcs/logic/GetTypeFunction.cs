using FuncScript.Core;
using System;
using FuncScript.Model;

namespace FuncScript.Funcs.OS
{
    internal class GetTypeFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;

        public string Symbol => "type";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != 1)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol} function: invalid parameter count. 1 expected, got {pars.Length}");

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
