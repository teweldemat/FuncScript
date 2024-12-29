using funcscript.core;
using funcscript.model;

namespace funcscript.funcs.text
{
    public class SubStringFunction : IFsFunction
    {
        private const int MaxParameters = 3;

        public CallType CallType => CallType.Prefix;
        public string Symbol => "substring";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length == 0)
                throw new error.EvaluationTimeException($"{this.Symbol} requires at least one parameter.");

            var par0 = pars[0] as string;
            if (par0 == null)
                return null;

            var par1 = pars.Length > 1 ? pars[1] : null;
            var par2 = pars.Length > 2 ? pars[2] : null;

            int index = Convert.ToInt32(par1 ?? 0);
            int count = Convert.ToInt32(par2 ?? par0.Length);

            if (index < 0 || index >= par0.Length) return "";
            if (count < 0 || index + count > par0.Length) count = par0.Length - index;

            return par0.Substring(index, count);
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "string",
                1 => "index",
                2 => "count",
                _ => ""
            };
        }
    }
}