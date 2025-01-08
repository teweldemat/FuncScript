using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.KeyValue
{
    public class KvcMemberFunction : IFsFunction
    {
        private const int MaxParameterCount = 2;

        public CallType CallType => CallType.Infix;

        public string Symbol => ".";

        private object EvaluateInternal(object par0, object par1)
        {
            if (!(par1 is string))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{Symbol} function: The second parameter should be {ParName(1)}");

            if (par0 == null)
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{Symbol} function: Can't get member {par1} from null data");

            if (!(par0 is KeyValueCollection))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{Symbol} function: Can't get member {par1} from a {Helpers.GetFsDataType(par0)}");

            return ((KeyValueCollection)par0).Get(((string)par1).ToLower());
        }

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            if (pars.Length != 2)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{Symbol} function: Invalid parameter count. Expected 2, but got {pars.Length}");

            var par0 = pars[0];
            var par1 = pars[1];

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
