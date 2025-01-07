using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Logic
{
    public class SwitchFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;

        public string Symbol => "switch";

        public object EvaluateList(FsList pars)
        {

            var selector = pars[0];

            for (var i = 1; i < pars.Length - 1; i += 2)
            {
                var val = pars[i];

                if ((val == null && selector == null) ||
                    (val != null && selector != null && selector.Equals(val)))
                {
                    return pars[i + 1];
                }
            }

            if (pars.Length % 2 == 0)
            {
                return pars[pars.Length - 1];
            }

            return null;
        }

        public string ParName(int index)
        {
            return "Parameter " + (index + 1);
        }
    }
}
