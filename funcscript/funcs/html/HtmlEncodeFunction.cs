using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Html
{
    internal class HtmlEncodeFunction : IFsFunction
    {
        public CallType CallType => CallType.Infix;

        public string Symbol => "HEncode";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != 1)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol}: one parameter expected");

            var str = pars[0];
            if (str == null)
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: non-null string expected");

            return System.Web.HttpUtility.HtmlEncode(str.ToString());
        }

        public string ParName(int index)
        {
            switch(index)
            {
                case 0: return "text";
                default:
                    return "";
            }
        }
    }
}
