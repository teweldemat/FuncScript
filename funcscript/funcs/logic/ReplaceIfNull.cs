using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Logic
{
    public class ReplaceIfNull : IFsFunction
    {
        public CallType CallType => CallType.Infix;

        public string Symbol => "??";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != 2)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{Symbol}: function expects exactly 2 parameters.");

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
