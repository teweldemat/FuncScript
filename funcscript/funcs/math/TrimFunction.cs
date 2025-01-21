using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Text
{
    public class TrimFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;
        public string Symbol => "trim";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            if (pars.Length < 1)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol} requires one parameter: text.");

            var input = pars[0] as string;

            if (input == null)
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol} requires the parameter to be a string.");

            return input.Trim();
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "text",
                _ => ""
            };
        }
    }
}
