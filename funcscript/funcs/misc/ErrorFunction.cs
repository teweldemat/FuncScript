using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Misc
{
    public class ErrorFunction : IFsFunction
    {
        public const string SYMBOL = "error";

        public CallType CallType => CallType.Prefix;

        public string Symbol => SYMBOL;

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != 1)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, "One parameter expected");

            var param = pars[0];

            if (param is string str)
                return new FsError(FsError.ERROR_DEFAULT, str);    

            return new FsError(FsError.ERROR_TYPE_MISMATCH, "Parameter is not a valid string object");
        }

        public string ParName(int index)
        {
            return "Error";
        }
    }
}
