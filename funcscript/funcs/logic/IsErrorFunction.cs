using funcscript.core;
using System;
using funcscript.model;

namespace funcscript.funcs.logic
{
    public class IsErrorFunction : IFsFunction
    {
        public const string SYMBOL = "isError";
        private const int MAX_PARS_COUNT = 1;

        public CallType CallType => CallType.Prefix;

        public string Symbol => SYMBOL;

        public object Evaluate(IFsDataProvider parent, IParameterList pars)
        {
            if (pars.Count != MAX_PARS_COUNT)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"{this.Symbol}: expected {MAX_PARS_COUNT} parameters, got {pars.Count}");

            var par0 = pars.GetParameter(parent, 0);

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