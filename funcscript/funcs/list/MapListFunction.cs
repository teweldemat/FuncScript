using FuncScript.Core;
using FuncScript.Model;
using System.Collections.Generic;

namespace FuncScript.Funcs.List
{
    public class MapListFunction : IFsFunction
    {
        public CallType CallType => CallType.Dual;

        public string Symbol => "Map";

        public object EvaluateList(FsList pars)
        {
            const int expectedParameters = 2;
            if (pars.Length != expectedParameters)
                throw new Error.TypeMismatchError($"{this.Symbol} function: Invalid parameter count. Expected {expectedParameters}, but got {pars.Length}");

            var par0 = pars[0];
            var par1 = pars[1];

            return EvaluateInternal(par0, par1, false);
        }

        private object EvaluateInternal(object par0, object par1, bool dref)
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
                var item = lst[i];
                var parsList = new ArrayFsList(new object[] { item, i });
                res.Add(func.EvaluateList(parsList));
            }

            return new ArrayFsList(res);
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "List",
                1 => "Transform Function",
                _ => ""
            };
        }
    }
}
