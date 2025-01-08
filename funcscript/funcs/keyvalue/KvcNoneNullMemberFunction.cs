using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.KeyValue
{
    public class KvcNoneNullMemberFunction : IFsFunction
    {
        public CallType CallType => CallType.Infix;

        public string Symbol => "?.";

        private object EvaluateInternal(object target, object key)
        {
            if (!(key is string))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{Symbol} function: The second parameter should be a string (Member key).");

            if (target == null)
                return null;

            if (!(target is KeyValueCollection))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{Symbol} function: Cannot access member '{key}' on non-KeyValueCollection type '{Helpers.GetFsDataType(target)}'.");

            return ((KeyValueCollection)target).Get(((string)key).ToLower());
        }

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            if (pars.Length != 2)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{Symbol} function: Expected 2 parameters, received {pars.Length}.");

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
