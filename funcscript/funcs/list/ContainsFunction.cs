using funcscript.core;
using funcscript.model;

namespace funcscript.funcs.list
{
    public class ContainsFunction : IFsFunction
    {
        private const int MaxParsCountValue = 2;

        public CallType CallType => CallType.Prefix;

        public string Symbol => "Contains";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != MaxParsCountValue)
                throw new error.TypeMismatchError($"{this.Symbol} function: Invalid parameter count. Expected {MaxParsCountValue}, but got {pars.Length}");

            var container = pars[0];
            var item = pars[1];

            return EvaluateInternal(container, item);
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

            throw new error.TypeMismatchError($"{this.Symbol} function: Invalid types for parameters");
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