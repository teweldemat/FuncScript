using funcscript.core;
using funcscript.model;

namespace funcscript.funcs.logic
{
    [FunctionAlias("!")]
    public class NotFunction : IFsFunction
    {
        public const string SYMBOL = "not";
        private const int MAX_PARS_COUNT = 1;

        public CallType CallType => CallType.Prefix;

        public string Symbol => SYMBOL;

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != MAX_PARS_COUNT)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"{this.Symbol}: expected {MAX_PARS_COUNT} parameters got {pars.Length}");

            var par0 = pars[0];

            if (par0 == null)
                return new FsError(FsError.ERROR_TYPE_MISMATCH,
                    $"Function {this.Symbol} doesn't apply on null data");

            if (par0 is bool)
                return !(bool)par0;
            return new FsError(FsError.ERROR_TYPE_MISMATCH,
                $"Function {this.Symbol} doesn't apply to data type: {par0.GetType()}");
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