using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Logic
{
    public class EvaluateIfNotNull : IFsFunction
    {
        public CallType CallType => CallType.Infix;

        public string Symbol => "?!";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != 2)
                throw new Error.TypeMismatchError($"{Symbol} function expects exactly two parameters.");

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
