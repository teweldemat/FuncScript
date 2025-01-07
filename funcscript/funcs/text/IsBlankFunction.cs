using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Math
{
    public class IsBlankFunction : IFsFunction
    {
        public const string SYMBOL = "isBlank";
        private const int MIN_PARS_COUNT = 1;

        public CallType CallType => CallType.Prefix;

        public string Symbol => SYMBOL;

        public object EvaluateList(FsList pars)
        {
            if (pars.Length < MIN_PARS_COUNT)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol}: argument expected");

            var parameter = pars[0];

            if (parameter == null)
                return true;

            if (parameter is not string str)
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: string expected");

            return string.IsNullOrEmpty(str.Trim());
        }

        public string ParName(int index)
        {
            if (index == 0)
            {
                return "String";
            }
            return "";
        }
    }
}
