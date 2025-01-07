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
