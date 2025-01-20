using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Misc
{
    public class ErrorFunction : IFsFunction
    {
        public const string SYMBOL = "error";

        public CallType CallType => CallType.Prefix;

        public string Symbol => SYMBOL;

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            if (pars.Length < 1 || pars.Length > 2)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, "One or two parameters expected");

            var param = pars[0];
            object extraData = null;

            if (pars.Length == 2)
                extraData = pars[1];

            if (param is string str)
                return new FsError(FsError.ERROR_DEFAULT, str, extraData);    

            return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, "Parameter is not a valid string object");
        }

        public string ParName(int index)
        {
            return "Error";
        }
    }
}
