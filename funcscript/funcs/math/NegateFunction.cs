using funcscript.core;
using funcscript.model;

namespace funcscript.funcs.math
{
    public class NegateFunction : IFsFunction
    {
        public const string SYMBOL = "neg";
        private const int MAX_PARS_COUNT = 1;

        public CallType CallType => CallType.Prefix;

        public string Symbol => SYMBOL;

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != MAX_PARS_COUNT)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, "One parameter expected");

            var param = pars[0];

            if (param is int intValue)
                return -intValue;
            if (param is long longValue)
                return -longValue;
            if (param is double doubleValue)
                return -doubleValue;

            return null;
        }

        public string ParName(int index)
        {
            return "Value";
        }
    }
}