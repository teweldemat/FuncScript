using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Math
{
    public class NegateFunction : IFsFunction
    {
        public const string SYMBOL = "neg";

        public CallType CallType => CallType.Prefix;

        public string Symbol => SYMBOL;

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != 1)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol}: one parameter expected");

            var param = pars[0];

            if (param is int intValue)
                return -intValue;
            if (param is long longValue)
                return -longValue;
            if (param is double doubleValue)
                return -doubleValue;

            return new FsError(FsError.ERROR_TYPE_MISMATCH, $"{this.Symbol}: number expected");
        }

        public string ParName(int index)
        {
            return "Value";
        }
    }
}
