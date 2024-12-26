using funcscript.core;

namespace funcscript.funcs.math
{
    public class IsBlankFunction : IFsFunction
    {
        public const string SYMBOL = "isBlank";
        public int MaxParsCount => 1;

        public CallType CallType => CallType.Prefix;

        public string Symbol => SYMBOL;

        public object Evaluate(IFsDataProvider parent, IParameterList pars)
        {
            if (pars.Count < 1)
                throw new error.TypeMismatchError($"{this.Symbol}: argument expected");

            var parameter = pars.GetParameter(parent, 0);

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