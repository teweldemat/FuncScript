using funcscript.core;
using funcscript.model;

namespace funcscript.funcs.list
{
    public class LengthFunction : IFsFunction
    {
        private const int MaxParameters = 1;

        public CallType CallType => CallType.Prefix;

        public string Symbol => "Len";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != MaxParameters)
                throw new error.TypeMismatchError($"{this.Symbol} function: Invalid parameter count. Expected {MaxParameters}, but got {pars.Length}");

            var par0 = pars[0];

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