using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Logic
{
    public class IfConditionFunction : IFsFunction
    {
        public CallType CallType => CallType.Infix;

        public string Symbol => "If";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            if (pars.Length < 3)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, "IfConditionFunction requires three parameters: condition, trueResult, and falseResult.");

            var condition = pars[0];

            if (!(condition is bool))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: The first parameter must be a boolean value.");

            bool evalCondition = (bool)condition;
            int resultIndex = evalCondition ? 1 : 2;
            var result = pars[resultIndex];

            return result;
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "Condition",
                1 => "True Case",
                2 => "False Case",
                _ => ""
            };
        }
    }
}
