using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Math
{
    public class SineFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;

        public string Symbol => "Sin";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            if (pars.Length != 1)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"{this.Symbol}: expected 1 got {pars.Length}");

            var val = pars[0];

            if (val is int intValue)
            {
                return System.Math.Sin((double)intValue);
            }

            if (val is double doubleValue)
            {
                return System.Math.Sin(doubleValue);
            }

            if (val is long longValue)
            {
                return System.Math.Sin((double)longValue);
            }

            return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: A number was expected.");
        }

        public string ParName(int index)
        {
            return "number";
        }
    }

    public class CosineFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;

        public string Symbol => "Cos";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            if (pars.Length != 1)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"{this.Symbol}: expected 1 got {pars.Length}");

            var val = pars[0];
            if (val is int intValue)
            {
                return System.Math.Cos((double)intValue);
            }
            if (val is double doubleValue)
            {
                return System.Math.Cos(doubleValue);
            }
            if (val is long longValue)
            {
                return System.Math.Cos((double)longValue);
            }
            return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: number expected");
        }

        public string ParName(int index)
        {
            return "number";
        }
    }
}
