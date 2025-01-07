using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Logic
{
    public class IsErrorFunction : IFsFunction
    {
        public const string SYMBOL = "isError";
        private const int MAX_PARS_COUNT = 1;

        public CallType CallType => CallType.Prefix;

        public string Symbol => SYMBOL;

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != MAX_PARS_COUNT)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"{this.Symbol}: expected {MAX_PARS_COUNT} parameters, got {pars.Length}");

            var par0 = pars[0];

            if (par0 == null)
                return false;

            return par0 is FsError;
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "Object",
                _ => ""
            };
        }
    }
}
