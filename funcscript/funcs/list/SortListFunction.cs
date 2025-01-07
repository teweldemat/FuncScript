using FuncScript.Core;
using FuncScript.Model;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FuncScript.Funcs.List
{
    public class SortListFunction : IFsFunction
    {
        public CallType CallType => CallType.Dual;

        public string Symbol => "Sort";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != 2)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol} function: Invalid parameter count. Expected 2, but got {pars.Length}");

            var par0 = pars[0];
            var par1 = pars[1];

            return EvaluateInternal(par0, par1);
        }

        private object EvaluateInternal(object par0, object par1)
        {
            if (par0 == null)
                return null;

            if (!(par0 is FsList))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol} function: The first parameter should be {this.ParName(0)}");

            if (!(par1 is IFsFunction))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol} function: The second parameter should be {this.ParName(1)}");

            var func = (IFsFunction)par1;
            var lst = (FsList)par0;
            var res = new List<object>(lst);

            FsError error = null;
            try
            {
                res.Sort((x, y) =>
                {
                    var sortParamList = new ArrayFsList(new object[] { x, y });
                    var result = func.EvaluateList(sortParamList);

                    if (!(result is int))
                    {
                        error= new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol} function: The sorting function must return an integer");
                        throw new Exception();
                    }

                    return (int)result;
                });
            }
            catch
            {
                if (error != null)
                    return error;
                throw;
            }

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
