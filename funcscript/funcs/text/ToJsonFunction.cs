using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Text
{
    public class ToJsonFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;
        public string Symbol => "tojson";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            if (pars.Length < 1)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol} requires one parameter.");

            var par0 = pars[0];
            return Helpers.FormatToJson(par0);
        }

        public string ParName(int index)
        {
            return "value";
        }
    }
} 