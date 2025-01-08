using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Logic
{
    public class NotEqualsFunction : IFsFunction
    {
        public CallType CallType => CallType.Infix;

        public string Symbol => "!=";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            const int MaxParameterCount = 2;

            if (pars.Length != MaxParameterCount)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"{this.Symbol}: expected {MaxParameterCount} got {pars.Length}");

            var par0 = pars[0];
            var par1 = pars[1];

            try
            {
                return EvaluateInternal(par0, par1);
            }
            catch (Exception ex)
            {
                return new FsError(ex);
            }
        }

        private object EvaluateInternal(object par0, object par1)
        {
            if (par0 == null && par1 == null)
                return false;

            if (par0 == null || par1 == null)
                return true;

            if (Helpers.IsNumeric(par0) && Helpers.IsNumeric(par1))
            {
                Helpers.ConvertToCommonNumericType(par0, par1, out par0, out par1);
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
