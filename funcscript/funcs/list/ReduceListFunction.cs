using FuncScript.Core;
using FuncScript.Model;
using System;

namespace FuncScript.Funcs.List
{
    public class ReduceListFunction : IFsFunction
    {
        public CallType CallType => CallType.Dual;

        public string Symbol => "Reduce";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length < 2)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"{this.Symbol}: expected at least 2 got {pars.Length}");

            var par0 = pars[0];
            if (par0 is null) return null;

            var par1 = pars[1];
            var par2 =pars.Length>2?pars[2]:null;

            return EvaluateInternal(par0, par1, par2);
        }

        private object EvaluateInternal(object par0, object par1, object par2)
        {
            if (!(par0 is FsList lst))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol} function: The first parameter should be {this.ParName(0)}");

            var func = par1 as IFsFunction;
            if (func == null)
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol} function: The second parameter didn't evaluate to a function");

            var total = par2;
            
            for (int i = 0; i < lst.Length; i++)
            {
                total = func.EvaluateList(new ArrayFsList(new object[] { lst[i], total, i }));
            }

            return FuncScript.NormalizeDataType(total);
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "List",
                1 => "Transform Function",
                2 => "Initial Value",
                _ => ""
            };
        }
    }
}
