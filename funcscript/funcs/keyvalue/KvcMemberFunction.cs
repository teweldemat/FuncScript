using funcscript.core;
using funcscript.model;

namespace funcscript.funcs.keyvalue
{
    public class KvcMemberFunction : IFsFunction
    {
        private const int MaxParameterCount = 2;

        public CallType CallType => CallType.Infix;

        public string Symbol => ".";

        private object EvaluateInternal(object par0, object par1)
        {
            if (!(par1 is string))
                throw new error.TypeMismatchError($"{Symbol} function: The second parameter should be {ParName(1)}");

            if (par0 == null)
                throw new error.TypeMismatchError($"{Symbol} function: Can't get member {par1} from null data");

            if (!(par0 is KeyValueCollection))
                throw new error.TypeMismatchError($"{Symbol} function: Can't get member {par1} from a {FuncScript.GetFsDataType(par0)}");

            return ((KeyValueCollection)par0).Get(((string)par1).ToLower());
        }

        public object Evaluate(IFsDataProvider parent, IParameterList pars)
        {
            if (pars.Count != MaxParameterCount)
                throw new error.TypeMismatchError($"{Symbol} function: Invalid parameter count. Expected {MaxParameterCount}, but got {pars.Count}");

            var par0 = pars.GetParameter(parent, 0);
            var par1 = pars.GetParameter(parent, 1);

            return EvaluateInternal(par0, par1);
        }

        public string ParName(int index)
        {
            switch (index)
            {
                case 0:
                    return "Key-value collection";
                case 1:
                    return "Member key";
                default:
                    return "";
            }
        }
    }
}