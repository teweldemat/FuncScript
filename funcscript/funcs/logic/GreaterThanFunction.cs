using FuncScript.Core;
using FuncScript.Model;
using System;

namespace FuncScript.Funcs.Logic
{
    public class GreaterThanFunction : IFsFunction
    {
        public CallType CallType => CallType.Infix;

        public string Symbol => ">";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            if (pars.Length != 2)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"{this.Symbol}: expected 2 got {pars.Length}");

            var par0 = pars[0];
            var par1 = pars[1];

            return EvaluateInternal(par0, par1);
        }

        private object EvaluateInternal(object par0, object par1)
        {
            if (par0 == null || par1 == null)
                return null;

            if (Helpers.IsNumeric(par0) && Helpers.IsNumeric(par1))
            {
                Helpers.ConvertToCommonNumericType(par0, par1, out par0, out par1);
            }

            if (par0.GetType() != par1.GetType())
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: Can't compare incompatible types");

            if (par0 is IComparable comparable)
                return comparable.CompareTo(par1) > 0;
            return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol} function can't compare these data types: {par0.GetType()}");
        }

        public string ParName(int index)
        {
            switch (index)
            {
                case 0:
                    return "Left Value";
                case 1:
                    return "Right Value";
                default:
                    return "";
            }
        }
    }
}
