using funcscript.core;
using funcscript.model;

namespace funcscript.funcs.logic
{
    public class CaseFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;

        public string Symbol => "Case";

        public object EvaluateList(FsList pars)
        {
            int count = pars.Length;

            for (int i = 0; i < count / 2; i++)
            {
                var cond = pars[i * 2];

                if (cond is bool && (bool)cond)
                {
                    return pars[i * 2 + 1];
                }
            }

            if (count % 2 == 1)
            {
                return pars[count - 1];
            }

            return null;
        }

        public string ParName(int index)
        {
            return "Parameter " + (index + 1);
        }
    }
}