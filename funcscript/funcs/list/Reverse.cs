using FuncScript.Core;
using FuncScript.Model;
using System.Collections.Generic;
using FuncScript.Funcs.Misc;

namespace FuncScript.Funcs.List
{
    public class ReverseListFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;

        public string Symbol => "Reverse";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            const int maxParameters = 1; // Updated

            if (pars.Length != maxParameters)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol} function: Invalid parameter count. Expected {maxParameters}, but got {pars.Length}");

            var par0 = pars[0];

            return EvaluateInternal(par0);
        }

        private object EvaluateInternal(object par0)
        {
            if (par0 == null)
                return null;

            if (!(par0 is FsList))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol} function: The parameter should be {this.ParName(0)}");

            var lst = (FsList)par0;
            var res = new List<object>();

            for (int i = lst.Length - 1; i >= 0; i--)
            {
                res.Add(lst[i]);
            }

            return new ArrayFsList(res);
        }

        public string ParName(int index)
        {
            if (index == 0)
                return "List";
            return "";
        }
    }
}
