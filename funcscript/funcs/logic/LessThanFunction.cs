using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Logic
{

    public class LessThanFunction : IFsFunction
    {
        public CallType CallType => CallType.Infix;

        public string Symbol => "<";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            const int MaxParameters = 2;
            if (pars.Length != MaxParameters)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"{this.Symbol}: expected {MaxParameters} got {pars.Length}");

            var par0 = pars[0];
            var par1 = pars[1];

            if (par0 == null || par1 == null)
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER,
                    $"{this.Symbol}: null value(s) not allowed");

            if (Helpers.IsNumeric(par0) && Helpers.IsNumeric(par1))
            {
                Helpers.ConvertToCommonNumericType(par0, par1, out par0, out par1);
            }

            if (par0.GetType() != par1.GetType())
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: Can't compare incompatible types");

            if (par0 is IComparable)
                return ((IComparable)par0).CompareTo(par1) < 0;

            return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol} function can't compare these data types: {par0.GetType()}");
        }

        public string ParName(int index)
        {
            switch(index)
            {
                case 0:
                    return "Left Value";
                case 1:
                    return "Right Value";
                default:
                    return "";
            }
        }
    }
}
