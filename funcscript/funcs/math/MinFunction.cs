using FuncScript.Core;
using FuncScript.Model;
using System;

namespace FuncScript.Funcs.Math
{
    public class MinFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;

        public string Symbol => "Min";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            if (pars.Length < 1)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"{this.Symbol}: expected at least 1 got {pars.Length}");

            double? minValue = null;

            for (int i = 0; i < pars.Length; i++)
            {
                var val = pars[i];
                if (val is int intValue)
                {
                    minValue = minValue.HasValue ? System.Math.Min(minValue.Value, (double)intValue) : (double)intValue;
                }
                else if (val is double doubleValue)
                {
                    minValue = minValue.HasValue ? System.Math.Min(minValue.Value, doubleValue) : doubleValue;
                }
                else if (val is long longValue)
                {
                    minValue = minValue.HasValue ? System.Math.Min(minValue.Value, (double)longValue) : (double)longValue;
                }
                else
                {
                    return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: number expected");
                }
            }

            return minValue.HasValue ? minValue.Value : new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: no valid number provided");
        }

        public string ParName(int index)
        {
            return "number";
        }
    }
}
