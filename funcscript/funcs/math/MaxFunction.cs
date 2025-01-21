using FuncScript.Core;
using FuncScript.Model;
using System.Collections.Generic;

namespace FuncScript.Funcs.Math
{
    public class MaxFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;

        public string Symbol => "Max";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            if (pars.Length < 1)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"{this.Symbol}: expected at least 1 parameter, got {pars.Length}");

            double max = double.MinValue;
            bool foundNumber = false;

            foreach (var val in pars)
            {
                if (val is int intValue)
                {
                    max = System.Math.Max(max, (double)intValue);
                    foundNumber = true;
                }
                else if (val is double doubleValue)
                {
                    max = System.Math.Max(max, doubleValue);
                    foundNumber = true;
                }
                else if (val is long longValue)
                {
                    max = System.Math.Max(max, (double)longValue);
                    foundNumber = true;
                }
                else
                {
                    return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER,
                        $"{this.Symbol}: All parameters must be numbers.");
                }
            }

            if (!foundNumber)
            {
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER,
                    $"{this.Symbol}: No valid number found.");
            }

            return max;
        }

        public string ParName(int index)
        {
            return "number";
        }
    }
}
