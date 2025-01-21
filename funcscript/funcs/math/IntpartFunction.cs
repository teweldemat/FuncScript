using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Math
{
    public class IntpartFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;

        public string Symbol => "Intpart";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            if (pars.Length != 1)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"{this.Symbol}: expected 1 got {pars.Length}");

            var val = pars[0];
            if (val is int intValue)
            {
                return intValue; // Integer part of an integer is the integer itself.
            }
            if (val is double doubleValue)
            {
                return (int)doubleValue; // Cast to int truncates to the integer part.
            }
            if (val is long longValue)
            {
                return (int)longValue; // Cast to int, assuming values are within the range of int.
            }
            return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: number expected");
        }

        public string ParName(int index)
        {
            return "number";
        }
    }
}
