using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Text
{
    public class FindTextFunction : IFsFunction
    {
        public const string SYMBOL = "find";
        private const int MAX_PARS_COUNT = 3;

        public CallType CallType => CallType.Prefix;

        public string Symbol => SYMBOL;

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            if (pars.Length < 2 || pars.Length > 3)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol}: Two or three parameters expected");

            var par0 = pars[0];
            var par1 = pars[1];
            var par2 = pars.Length > 2 ? pars[2] : null;

            if (par0 == null || par1 == null)
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: Two strings and optionally an index expected as parameters");

            if (!(par0 is string text))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: first parameter should be string");
            if (!(par1 is string search))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: second parameter should be string");

            int startIndex = 0;
            if (par2 != null && !(par2 is int))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: third parameter should be an integer");
            if (par2 != null)
                startIndex = (int)par2;

            if (startIndex < 0 || startIndex >= text.Length)
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: index is out of range");

            return text.IndexOf(search, startIndex);
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "Text",
                1 => "Search",
                2 => "StartIndex",
                _ => ""
            };
        }
    }
}
