using funcscript.core;
using funcscript.model;

namespace funcscript.funcs.logic
{
    [FunctionAlias("!")]
    public class NotFunction : IFsFunction
    {
        public const string SYMBOL = "not";
        public int MaxParsCount => 1;

        public CallType CallType => CallType.Prefix;

        public string Symbol => SYMBOL;

        public object Evaluate(IFsDataProvider parent, IParameterList pars)
        {
            if (pars.Count != this.MaxParsCount)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"{this.Symbol}: expected {this.MaxParsCount} parameters got {pars.Count}");

            var par0 = pars.GetParameter(parent, 0);

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