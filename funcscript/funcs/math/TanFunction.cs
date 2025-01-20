using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Math
{
    public class TanFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;

        public string Symbol => "Tan";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            if (pars.Length != 1)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"{this.Symbol}: expected 1 got {pars.Length}");

            var val = pars[0];
            if (val is int intValue)
            {
                return System.Math.Tan((double)intValue);
            }
            if (val is double doubleValue)
            {
                return System.Math.Tan(doubleValue);
            }
            if (val is long longValue)
            {
                return System.Math.Tan((double)longValue);
            }
            return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: number expected");
        }

        public string ParName(int index)
        {
            return "number";
        }
    }
}
