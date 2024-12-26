using funcscript.core;
using funcscript.model;

namespace funcscript.funcs.logic
{
    public class SwitchFunction : IFsFunction
    {
        public int MaxParsCount => -1;

        public CallType CallType => CallType.Prefix;

        public string Symbol => "switch";

        public object Evaluate(IFsDataProvider parent, IParameterList pars)
        {
            var selector = pars.GetParameter(parent, 0);

            for (var i = 1; i < pars.Count - 1; i += 2)
            {
                var val = pars.GetParameter(parent, i);

                if ((val == null && selector == null) ||
                    (val != null && selector != null && selector.Equals(val)))
                {
                    return pars.GetParameter(parent, i + 1);
                }
            }

            if (pars.Count % 2 == 0)
            {
                return pars.GetParameter(parent, pars.Count - 1);
            }

            return null;
        }

        public string ParName(int index)
        {
            return "Parameter " + (index + 1);
        }
    }
}