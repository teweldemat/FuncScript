using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Logic
{
    public class CaseFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;

        public string Symbol => "case";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            int count = pars.Length;

            if (count == 0)
            {
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, "Case: at least one parameter expected");
            }

            for (int i = 0; i < count / 2; i++)
            {
                var cond = pars[i * 2];

                if (cond is bool && (bool)cond)
                {
                    return pars[i * 2 + 1];
                }

                if (!(cond is bool))
                {
                    return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, "Case: boolean condition expected");
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
