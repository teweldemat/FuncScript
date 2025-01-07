using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Text
{
    public class ReplaceAllFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;
        public string Symbol => "replaceall";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length < 3)
                throw new Error.EvaluationTimeException($"{this.Symbol} requires exactly three parameters: input string, search string, and replacement string.");

            var input = pars[0] as string;
            var search = pars[1] as string;
            var replacement = pars[2] as string;

            if (input == null || search == null || replacement == null)
                throw new Error.EvaluationTimeException($"{this.Symbol} parameters must all be strings.");

            return input.Replace(search, replacement);
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "input",
                1 => "search",
                2 => "replacement",
                _ => ""
            };
        }
    }
}
