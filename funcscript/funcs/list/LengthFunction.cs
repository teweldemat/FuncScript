using funcscript.core;
using funcscript.model;

namespace funcscript.funcs.list
{
    public class LengthFunction : IFsFunction
    {
        public int MaxParsCount => 1;

        public CallType CallType => CallType.Prefix;

        public string Symbol => "Len";

        public object Evaluate(IFsDataProvider parent, IParameterList pars)
        {
            if (pars.Count != this.MaxParsCount)
                throw new error.TypeMismatchError($"{this.Symbol} function: Invalid parameter count. Expected {this.MaxParsCount}, but got {pars.Count}");

            var par0 = pars.GetParameter(parent, 0);

            return EvaluateInternal(par0);
        }

        private object EvaluateInternal(object par0)
        {
            return par0 switch
            {
                null => 0,
                FsList list => list.Length,
                string s => s.Length,
                _ => throw new error.TypeMismatchError($"{this.Symbol} function doesn't apply to {par0.GetType()}")
            };
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "List or String",
                _ => ""
            };
        }
    }
}