using funcscript.core;
using funcscript.model;

namespace funcscript.funcs.text
{
    public class SubStringFunction : IFsFunction, IFsDref
    {
        public int MaxParsCount => 3;
        public CallType CallType => CallType.Prefix;
        public string Symbol => "substring";
        public int Precidence => 0;

        public object Evaluate(IFsDataProvider parent, IParameterList pars)
        {
            if (pars.Count == 0)
                throw new error.EvaluationTimeException($"{this.Symbol} requires at least one parameter.");

            var parBuilder = new CallRefBuilder(this, parent, pars);
            var par0 = parBuilder.GetParameter(0);
            var par1 = parBuilder.GetParameter(1);
            var par2 = parBuilder.GetParameter(2);

            if (par0 is ValueReferenceDelegate || par1 is ValueReferenceDelegate || par2 is ValueReferenceDelegate)
                return parBuilder.CreateRef();

            var str = par0 as string;
            if (str == null)
                return null;

            int index = Convert.ToInt32(par1 ?? 0);
            int count = Convert.ToInt32(par2 ?? str.Length);

            if (index < 0 || index >= str.Length) return "";
            if (count < 0 || index + count > str.Length) count = str.Length - index;

            return str.Substring(index, count);
        }

        public object DrefEvaluate(IParameterList pars)
        {
            var str = FuncScript.Dref(pars.GetParameter(null, 0),false) as string;
            if (str == null)
                return null;

            int index = Convert.ToInt32(FuncScript.Dref(pars.GetParameter(null, 1)) ?? 0);
            int count = Convert.ToInt32(FuncScript.Dref(pars.GetParameter(null, 2)) ?? str.Length);

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