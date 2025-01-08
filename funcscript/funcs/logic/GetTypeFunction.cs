using FuncScript.Core;
using System;
using FuncScript.Model;

namespace FuncScript.Funcs.OS
{
    internal class GetTypeFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;

        public string Symbol => "type";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            if (pars.Length != 1)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol} function: invalid parameter count. 1 expected, got {pars.Length}");

            var par0 = pars[0];
            var dataType = Helpers.GetFsDataType(par0);
            return dataType.ToString();
        }

        public string ParName(int index)
        {
            return index == 0 ? "value" : null;
        }
    }
}
