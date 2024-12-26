using funcscript.core;

namespace funcscript.funcs.html
{
    internal class HtmlEncodeFunction : IFsFunction
    {
        public int MaxParsCount => 1;

        public CallType CallType => CallType.Infix;

        public string Symbol => "HEncode";

        public object Evaluate(IFsDataProvider parent, IParameterList pars)
        {
            var str = pars.GetParameter(parent, 0);
            return str == null ? null : System.Web.HttpUtility.HtmlEncode(str.ToString());
        }

        public string ParName(int index)
        {
            switch(index)
            {
                case 0:return "text";
                default:
                    return "";
            }
        }
    }
}