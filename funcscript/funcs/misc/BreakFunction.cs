using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Misc
{
    public class BreakFunction : IFsFunction
    {
        public const string SYMBOL = "break";

        public CallType CallType => CallType.Prefix;

        public string Symbol => SYMBOL;

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            if (pars.Length != 1)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, "One parameter expected");

            var extraData = pars[0];
            return new FsError(FsError.CONTROL_BREAK, null, extraData);
        }

        public string ParName(int index)
        {
            return "ExtraData";
        }
    }
}