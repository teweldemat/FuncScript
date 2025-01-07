using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Logic
{
    [FunctionAlias("!")]
    public class NotFunction : IFsFunction
    {
        public const string SYMBOL = "not";

        public CallType CallType => CallType.Prefix;

        public string Symbol => SYMBOL;

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != 1)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"{this.Symbol}: one parameter expected");

            var par0 = pars[0];

            if (par0 == null)
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER,
                    $"{this.Symbol}: non-null parameter expected");

            if (par0 is bool)
                return !(bool)par0;
            
            return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER,
                $"{this.Symbol}: boolean expected");
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "Boolean",
                _ => ""
            };
        }
    }
}
