using funcscript.core;

namespace funcscript.funcs.logic
{
    public class TicksToDateFunction : IFsFunction
    {
        private const int MaxParameters = 1;

        public CallType CallType => CallType.Prefix;

        public string Symbol => "TicksToDate";

        public object Evaluate(IFsDataProvider parent, IParameterList pars)
        {
            if (pars.Count > MaxParameters)
                throw new error.EvaluationTimeException($"{this.Symbol} function: Invalid parameter count. Expected a maximum of {MaxParameters}, but got {pars.Count}");

            var par0 = pars.GetParameter(parent, 0);

            if (par0 == null)
                return null;

            if (!(par0 is long))
                throw new error.TypeMismatchError($"Function {this.Symbol}: Type mismatch. Expected a long.");

            var ticks = (long)par0;

            return new DateTime(ticks);
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "Ticks",
                _ => ""
            };
        }
    }
}