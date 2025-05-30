using FuncScript.Core;
using FuncScript.Model;
using System;
using System.Linq;

namespace FuncScript.Funcs.List
{
    public class TakeFunction : IFsFunction
    {
        public CallType CallType => CallType.Dual;

        public string Symbol => "Take";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            const int MaxParameters = 2;
            if (pars.Length != MaxParameters)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{Symbol} function: Invalid parameter count. Expected {MaxParameters}, but got {pars.Length}");

            var par0 = pars[0];
            var par1 = pars[1];

            return EvaluateInternal(par0, par1);
        }

        private object EvaluateInternal(object par0, object par1)
        {
            if (par0 == null)
                return null;

            if (!(par0 is FsList))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{Symbol} function: The first parameter should be {ParName(0)}");

            if (!(par1 is int))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{Symbol} function: The second parameter should be {ParName(1)}");

            var lst = (FsList)par0;
            int n = (int)par1;

            if (n <= 0)
                return new ArrayFsList(new object[] { });

            if (n > lst.Length)
                n = lst.Length;

            return new ArrayFsList(lst.Take(n).ToArray());
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "List",
                1 => "Number",
                _ => ""
            };
        }
    }
}
