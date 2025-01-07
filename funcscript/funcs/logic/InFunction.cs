using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Logic
{
    public class InFunction : IFsFunction
    {
        public CallType CallType => CallType.Infix;

        public string Symbol => "in";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != 2)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{Symbol} function: Invalid parameter count. Expected 2, but got {pars.Length}");

            var par0 = pars[0];
            var par1 = pars[1];

            if (par1 == null)
                return null;

            if (!(par1 is FsList))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{Symbol} function: {ParName(1)} should be a list");

            bool par0Numeric = FuncScript.IsNumeric(par0);

            foreach (var val in (FsList)par1)
            {
                if (val == null)
                    continue;

                object left, right;

                if (par0Numeric && FuncScript.IsNumeric(val))
                {
                    FuncScript.ConvertToCommonNumericType(par0, val, out left, out right);
                }
                else
                {
                    left = par0;
                    right = val;
                }

                if (left == null && right == null)
                    return true;

                if (left == null || right == null)
                    return false;

                if (left.GetType() != right.GetType())
                    continue;

                if (left.Equals(right))
                    return true;
            }

            return false;
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "Value",
                1 => "List",
                _ => ""
            };
        }
    }
}
