using funcscript.core;
using funcscript.model;

namespace funcscript.funcs.logic
{
    public class NotEqualsFunction : IFsFunction
    {
        private const int MaxParameterCount = 2;

        public CallType CallType => CallType.Infix;

        public string Symbol => "!=";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != MaxParameterCount)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"{this.Symbol}: expected {MaxParameterCount} got {pars.Length}");

            var par0 = pars[0];
            var par1 = pars[1];

            return EvaluateInternal(par0, par1);
        }

        private object EvaluateInternal(object par0, object par1)
        {
            if (par0 == null && par1 == null)
                return false;

            if (par0 == null || par1 == null)
                return true;

            if (FuncScript.IsNumeric(par0) && FuncScript.IsNumeric(par1))
            {
                FuncScript.ConvertToCommonNumericType(par0, par1, out par0, out par1);
            }

            if (par0.GetType() != par1.GetType())
                return true;

            return !par0.Equals(par1);
        }

        public string ParName(int index)
        {
            switch (index)
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