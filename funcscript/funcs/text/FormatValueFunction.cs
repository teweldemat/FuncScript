using FuncScript.Core;
using System;
using System.Text;
using FuncScript.Model;

namespace FuncScript.Funcs.Text
{
    public class FormatValueFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;
        public string Symbol => "format";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length < 1)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol} requires at least one parameter.");

            var par0 = pars[0];
            var par1 = pars.Length > 1 ? pars[1] : null;

            string format = par1 as string;
            var sb = new StringBuilder();
            FuncScript.Format(sb, par0, format);
            return sb.ToString();
        }

        public string ParName(int index)
        {
            return index == 0 ? "value" : "format";
        }
    }
}
