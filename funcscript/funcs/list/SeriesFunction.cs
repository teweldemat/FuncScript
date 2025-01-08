using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.List
{
    public class SeriesFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;

        public string Symbol => "Series";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            if (pars.Length != 2)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol}: Two parameters expected");

            var par0 = pars[0];

            if (!(par0 is int))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: {ParName(0)} must be an integer");

            int start = (int)par0;

            var par1 = pars[1];

            if (!(par1 is int))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: {ParName(1)} must be an integer");

            int count = (int)par1;

            var ret = new List<int>();

            for (int i = 0; i < count; i++)
            {
                ret.Add(start + i);
            }

            return new ArrayFsList(ret);
        }

        public string ParName(int index)
        {
            switch(index)
            {
                case 0: return "start";
                case 1: return "count";
                default: return "";
            }
        }
    }
}
