using funcscript.core;
using funcscript.model;

namespace funcscript.funcs.keyvalue
{
    public class KvcNoneNullMemberFunction : IFsFunction
    {
        private const int MaxParametersCount = 2;

        public CallType CallType => CallType.Infix;

        public string Symbol => "?.";

        private object EvaluateInternal(object target, object key)
        {
            if (!(key is string))
                throw new error.TypeMismatchError($"{Symbol} function: The second parameter should be a string (Member key).");

            if (target == null)
                return null;

            if (!(target is KeyValueCollection))
                throw new error.TypeMismatchError($"{Symbol} function: Cannot access member '{key}' on non-KeyValueCollection type '{FuncScript.GetFsDataType(target)}'.");

            return ((KeyValueCollection)target).Get(((string)key).ToLower());
        }

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != MaxParametersCount)
                throw new error.TypeMismatchError($"{Symbol} function: Expected {MaxParametersCount} parameters, received {pars.Length}.");

            var key = pars[1];
            var target = pars[0];

            return EvaluateInternal(target, key);
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "Key-value collection",
                1 => "Member key",
                _ => string.Empty,
            };
        }
    }
}