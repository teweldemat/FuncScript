using funcscript.core;
using funcscript.model;

namespace funcscript.funcs.logic
{
    public class ReplaceIfNull : IFsFunction
    {
        private const int MaxParameters = 2;

        public CallType CallType => CallType.Infix;

        public string Symbol => "??";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != MaxParameters)
                throw new error.TypeMismatchError($"{Symbol} function expects exactly {MaxParameters} parameters.");

            var val = pars[0];

            if (val != null)
                return val;

            var val2 = pars[1];
            return val2;
        }

        public string ParName(int index)
        {
            switch (index)
            {
                case 0:
                    return "Value";
                case 1:
                    return "Null Replacement";
                default:
                    return "";
            }
        }
    }
}