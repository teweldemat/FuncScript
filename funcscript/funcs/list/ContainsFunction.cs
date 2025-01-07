using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.List
{
    public class ContainsFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;

        public string Symbol => "Contains";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != 2)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol} function: Invalid parameter count. Expected 2, but got {pars.Length}");

            var container = pars[0];
            var item = pars[1];

            var result = EvaluateInternal(container, item);
            if (result is FsError error)
                return error;
            
            return result;
        }

        private object EvaluateInternal(object container, object item)
        {
            if (container is FsList list)
            {
                return list.Contains(item);
            }

            if (container is string str && item is string substr)
            {
                return str.Contains(substr, StringComparison.OrdinalIgnoreCase);
            }

            return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol} function: Invalid types for parameters");
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "Container (List/String)",
                1 => "Item (Object/String)",
                _ => "",
            };
        }
    }
}
