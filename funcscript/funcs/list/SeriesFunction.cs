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
            var par1 = pars[1];

            // Get count first (same for both types)
            long count;
            if (par1 is int intCount)
                count = intCount;
            else if (par1 is long longCount)
                count = longCount;
            else if (par1 is double doubleCount)
                count = (long)doubleCount;
            else
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, 
                    $"{this.Symbol}: {ParName(1)} must be a number (integer or double)");

            // Handle int start value
            if (par0 is int intStart)
            {
                var ret = new List<int>();
                for (long i = 0; i < count; i++)
                {
                    ret.Add(intStart + (int)i);
                }
                return new ArrayFsList(ret);
            }
            // Handle long start value
            else if (par0 is long longStart)
            {
                var ret = new List<long>();
                for (long i = 0; i < count; i++)
                {
                    ret.Add(longStart + i);
                }
                return new ArrayFsList(ret);
            }
            // Handle double start value
            else if (par0 is double doubleStart)
            {
                var ret = new List<double>();
                for (long i = 0; i < count; i++)
                {
                    ret.Add(doubleStart + i);
                }
                return new ArrayFsList(ret);
            }
            else
            {
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, 
                    $"{this.Symbol}: {ParName(0)} must be a number (integer or double)");
            }
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
