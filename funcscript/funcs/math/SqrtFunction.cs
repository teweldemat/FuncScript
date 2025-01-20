using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Math
{
    public class SqrtFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;

        public string Symbol => "Sqrt";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            // Check if exactly one parameter is provided
            if (pars.Length != 1)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"{this.Symbol}: expected 1 got {pars.Length}");

            var val = pars[0];

            // Handle different numeric types
            if (val is int intValue)
            {
                if (intValue < 0)
                    return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: cannot compute square root of a negative number.");
                return System.Math.Sqrt(intValue);
            }

            if (val is double doubleValue)
            {
                if (doubleValue < 0)
                    return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: cannot compute square root of a negative number.");
                return System.Math.Sqrt(doubleValue);
            }

            if (val is long longValue)
            {
                if (longValue < 0)
                    return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: cannot compute square root of a negative number.");
                return System.Math.Sqrt(longValue);
            }

            return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: A number was expected.");
        }

        public string ParName(int index)
        {
            return "number";
        }
    }
}
