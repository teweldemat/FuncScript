using funcscript.core;
using funcscript.model;

namespace funcscript.funcs.html
{
    internal class HtmlEncodeFunction : IFsFunction
    {
        private const int MaxParameters = 1;

        public CallType CallType => CallType.Infix;

        public string Symbol => "HEncode";

        public object EvaluateList(FsList pars)
        {
            var str = pars.Length > 0 ? pars[0] : null;
            return str == null ? null : System.Web.HttpUtility.HtmlEncode(str.ToString());
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