using funcscript.core;
using funcscript.model;
using System;

namespace funcscript.funcs.list
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
            var par2 = pars[2];

            return EvaluateInternal(par0, par1, par2, false);
        }

        private object EvaluateInternal(object par0, object par1, object par2, bool dref)
        {
            if (!(par0 is FsList lst))
                throw new error.TypeMismatchError($"{this.Symbol} function: The first parameter should be {this.ParName(0)}");

            var func = par1 as IFsFunction;
            if (func == null)
                throw new error.TypeMismatchError($"{this.Symbol} function: The second parameter didn't evaluate to a function");

            var total = par2;
            
            for (int i = 0; i < lst.Length; i++)
            {
                total = func.EvaluateList(new ArrayFsList(new object[] { total, lst[i], i }));
            }

            return FuncScript.NormalizeDataType(total);
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