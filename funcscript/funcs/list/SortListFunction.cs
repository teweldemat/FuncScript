using FuncScript.Core;
using FuncScript.Model;
using System.Collections.Generic;

namespace FuncScript.Funcs.List
{
    public class SortListFunction : IFsFunction
    {
        private const int MaxParameters = 2;

        public CallType CallType => CallType.Dual;

        public string Symbol => "Sort";

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
            var res = new List<object>(lst);

            res.Sort((x, y) =>
            {
                var sortParamList = new ArrayFsList(new object[] { x, y });
                var result = func.EvaluateList(sortParamList);

                if (!(result is int))
                    throw new Error.EvaluationTimeException($"{this.Symbol} function: The sorting function must return an integer");

                return (int)result;
            });

            return new ArrayFsList(res);
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "List",
                1 => "Sorting Function",
                _ => ""
            };
        }
    }
}
