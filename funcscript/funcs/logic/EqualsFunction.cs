using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Logic
{
    public class EqualsFunction : IFsFunction
    {
        public CallType CallType => CallType.Infix;

        public string Symbol => "=";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            if (pars.Length != 2)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"{this.Symbol}: expected 2 parameters but got {pars.Length}");

            var par0 = pars[0];
            var par1 = pars[1];

            return EvaluateInternal(par0, par1);
        }

        private object EvaluateInternal(object par0, object par1)
        {
            if (par0 == null && par1 == null)
                return true;

            if (par0 == null || par1 == null)
                return false;

            if (Helpers.IsNumeric(par0) && Helpers.IsNumeric(par1))
            {
                Helpers.ConvertToCommonNumericType(par0, par1, out par0, out par1);
            }

            if ((par0 is IEnumerable<object> e1) && (par1 is IEnumerable<object> e2))
            {
                return e1.SequenceEqual(e2);
            }
            return par0.Equals(par1);
        }
        
    }
}
