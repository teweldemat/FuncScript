using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Logic
{
    public class LessThanOrEqualFunction : IFsFunction
    {
        public CallType CallType => CallType.Infix;

        public string Symbol => "<=";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != 2)
                throw new Error.EvaluationTimeException(
                    $"{this.Symbol} function: Invalid parameter count. Expected 2, but got {pars.Length}");

            var par0 = pars[0];
            var par1 = pars[1];

            if (par0 == null || par1 == null)
                return null;

            if (FuncScript.IsNumeric(par0) && FuncScript.IsNumeric(par1))
            {
                FuncScript.ConvertToCommonNumericType(par0, par1, out par0, out par1);
            }

            if (par0.GetType() != par1.GetType())
                return new FsError(FsError.ERROR_TYPE_MISMATCH, $"{this.Symbol}: Can't compare incompatible types");

            if (par0 is IComparable)
                return ((IComparable)par0).CompareTo(par1) <= 0;

            return new FsError(FsError.ERROR_TYPE_MISMATCH, $"{this.Symbol} function can't compare these data types: {par0.GetType()}");
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "Left Value",
                1 => "Right Value",
                _ => ""
            };
        }
    }
}
