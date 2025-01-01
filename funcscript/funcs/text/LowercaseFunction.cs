using funcscript.core;
using funcscript.model;

namespace funcscript.funcs.text
{
    public class LowercaseFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;
        public string Symbol => "lowercase";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length == 0)
                throw new error.EvaluationTimeException($"{this.Symbol} requires at least one parameter.");

            var input = pars[0] as string;
            if (input == null)
                return null;

            return input.ToLower();
        }

        public string ParName(int index)
        {
            return index == 0 ? "string" : "";
        }
    }
}