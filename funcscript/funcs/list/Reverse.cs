using funcscript.core;
using funcscript.model;
using System.Collections.Generic;
using funcscript.funcs.misc;

namespace funcscript.funcs.list
{
    public class ReverseListFunction : IFsFunction
    {
        private const int MaxParameters = 1;

        public CallType CallType => CallType.Prefix;

        public string Symbol => "Reverse";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != MaxParameters)
                throw new error.TypeMismatchError($"{this.Symbol} function: Invalid parameter count. Expected {MaxParameters}, but got {pars.Length}");

            var par0 = pars[0];

            return EvaluateInternal(par0);
        }

        private object EvaluateInternal(object par0)
        {
            if (par0 == null)
                return null;

            if (!(par0 is FsList))
                throw new error.TypeMismatchError($"{this.Symbol} function: The parameter should be {this.ParName(0)}");

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