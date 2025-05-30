using FuncScript.Core;
using FuncScript.Model;
using System.Collections.Generic;

namespace FuncScript.Funcs.List
{
    public class FilterListFunction : IFsFunction
    {
        public CallType CallType => CallType.Dual;

        public string Symbol => "Filter";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            if (pars.Length != 2)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol} function: Invalid parameter count. Expected 2, but got {pars.Length}");

            var par0 = pars[0];
            var par1 = pars[1];

        
            if (par0 == null)
                return null;

            if (!(par0 is FsList))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol} function: The first parameter should be {this.ParName(0)}");

            if (!(par1 is IFsFunction))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol} function: The second parameter should be {this.ParName(1)}");

            var func = (IFsFunction)par1;
            var lst = (FsList)par0;
            var res = new List<object>();

            for (int i = 0; i < lst.Length; i++)
            {
                var val = func.EvaluateList(context,new ArrayFsList(new object[] { lst[i], i }));
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
