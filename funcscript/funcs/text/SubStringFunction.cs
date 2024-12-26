using funcscript.core;
using funcscript.model;

namespace funcscript.funcs.text
{
    public class SubStringFunction : IFsFunction
    {
        private const int MaxParameters = 3;

        public CallType CallType => CallType.Prefix;
        public string Symbol => "substring";

        public object Evaluate(IFsDataProvider parent, IParameterList pars)
        {
            if (pars.Count == 0)
                throw new error.EvaluationTimeException($"{this.Symbol} requires at least one parameter.");

            var par0 = pars.GetParameter(parent, 0);
            var par1 = pars.GetParameter(parent, 1);
            var par2 = pars.GetParameter(parent, 2);

            var str = par0 as string;
            if (str == null)
                return null;

            int index = Convert.ToInt32(par1 ?? 0);
            int count = Convert.ToInt32(par2 ?? str.Length);

            if (index < 0 || index >= str.Length) return "";
            if (count < 0 || index + count > str.Length) count = str.Length - index;

            return str.Substring(index, count);
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