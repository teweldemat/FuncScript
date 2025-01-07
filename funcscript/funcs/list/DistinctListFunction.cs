using FuncScript.Core;
using FuncScript.Model;
using System.Collections.Generic;

namespace FuncScript.Funcs.List
{
    public class DistinctListFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;

        public string Symbol => "Distinct";

        public object EvaluateList(FsList pars)
        {
            const int MaxParameters = 1; // Move this declaration inside the method

            if (pars.Length != MaxParameters)
                throw new Error.EvaluationTimeException($"{this.Symbol} function: Invalid parameter count. Expected {MaxParameters}, but got {pars.Length}");

            var par0 = pars[0];

            if (par0 == null)
                return null;

            if (!(par0 is FsList))
                throw new Error.TypeMismatchError($"{this.Symbol} function: The parameter should be {this.ParName(0)}");

            var lst = (FsList)par0;

            var distinctValues = new HashSet<object>();
            var res = new List<object>();

            for (int i = 0; i < lst.Length; i++)
            {
                if (distinctValues.Add(lst[i]))
                {
                    res.Add(lst[i]);
                }
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
