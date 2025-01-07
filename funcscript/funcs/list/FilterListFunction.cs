using FuncScript.Core;
using FuncScript.Model;
using System.Collections.Generic;

namespace FuncScript.Funcs.List
{
    public class FilterListFunction : IFsFunction
    {
        private const int MaxParameters = 2;

        public CallType CallType => CallType.Dual;

        public string Symbol => "Filter";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != MaxParameters)
                throw new Error.TypeMismatchError($"{this.Symbol} function: Invalid parameter count. Expected {MaxParameters}, but got {pars.Length}");

            var par0 = pars[0];
            var par1 = pars[1];

            return EvaluateInternal(par0, par1);
        }

        private object EvaluateInternal(object par0, object par1)
        {
            if (par0 == null)
                return null;

            if (!(par0 is FsList))
                throw new Error.TypeMismatchError($"{this.Symbol} function: The first parameter should be {this.ParName(0)}");

            if (!(par1 is IFsFunction))
                throw new Error.TypeMismatchError($"{this.Symbol} function: The second parameter should be {this.ParName(1)}");

            var func = (IFsFunction)par1;
            var lst = (FsList)par0;
            var res = new List<object>();

            for (int i = 0; i < lst.Length; i++)
            {
                var val = func.EvaluateList(new ArrayFsList(new object[] { lst[i], i }));
                if (val is bool && (bool)val)
                {
                    res.Add(lst[i]);
                }
            }

            return new ArrayFsList(res);
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "List",
                1 => "Filter Function",
                _ => "",
            };
        }
    }
}
