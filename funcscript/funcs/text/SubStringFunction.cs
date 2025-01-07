using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Text
{
    public class SubStringFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;
        public string Symbol => "substring";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length == 0)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol} requires at least one parameter.");

            var par0 = pars[0] as string;
            if (par0 == null)
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: first parameter must be a string.");

            var par1 = pars.Length > 1 ? pars[1] : null;
            var par2 = pars.Length > 2 ? pars[2] : null;

            int index = Convert.ToInt32(par1 ?? 0);
            int count = Convert.ToInt32(par2 ?? par0.Length);

            if (index < 0 || index >= par0.Length)
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: index is out of range.");

            if (count < 0 || index + count > par0.Length)
                count = par0.Length - index;

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
