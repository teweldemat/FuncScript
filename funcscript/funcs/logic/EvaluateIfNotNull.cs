using funcscript.core;
using funcscript.model;

namespace funcscript.funcs.logic
{
    public class EvaluateIfNotNull : IFsFunction
    {
        private const int MaxParameters = 2;

        public CallType CallType => CallType.Infix;

        public string Symbol => "?!";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != MaxParameters)
                throw new error.TypeMismatchError($"{Symbol} function expects exactly two parameters.");

            var val = pars[0];
            if (val == null)
                return null;
            var val2 = pars[1];
            return val2; 
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "Value",
                1 => "Null Replacement",
                _ => ""
            };
        }
    }
}