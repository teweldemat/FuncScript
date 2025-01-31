using funcscript.core;
using funcscript.model;

namespace funcscript.funcs.math
{
    public class IsBlankFunction : IFsFunction
    {
        public const string SYMBOL = "isBlank";
        private const int MAX_PARS_COUNT = 1;

        public CallType CallType => CallType.Prefix;

        public string Symbol => SYMBOL;

        public object EvaluateList(FsList pars)
        {
            if (pars.Length < MAX_PARS_COUNT)
                throw new error.TypeMismatchError($"{this.Symbol}: argument expected");

            var parameter = pars[0];

            if (parameter == null)
                return true;

            if (parameter is not string str)
                throw new error.TypeMismatchError($"{this.Symbol}: string expected");

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