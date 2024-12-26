using funcscript.core;

namespace funcscript.funcs.logic
{
    public class CaseFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;

        public string Symbol => "Case";

        public object Evaluate(IFsDataProvider parent, IParameterList pars)
        {
            int count = pars.Count;

            for (int i = 0; i < count / 2; i++)
            {
                var cond = pars.GetParameter(parent, 2 * i);

                if (cond is bool && (bool)cond)
                {
                    return pars.GetParameter(parent, 2 * i + 1);
                }
            }

            if (count % 2 == 1)
            {
                return pars.GetParameter(parent, count - 1);
            }

            return null;
        }

        public string ParName(int index)
        {
            return "Parameter " + (index + 1);
        }
    }
}