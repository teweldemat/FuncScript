using funcscript.core;
using funcscript.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace funcscript.funcs.keyvalue
{

    public class KvcMemberFunction : IFsFunction
    {
        public int MaxParsCount => 2;

        public CallType CallType => CallType.Infix;

        public string Symbol => ".";

        public int Precidence => 200;

        public object Evaluate(IFsDataProvider parent, IParameterList pars)
        {
            if (pars.Count != MaxParsCount)
                throw new error.TypeMismatchError($"{Symbol} function: invalid parameter count. {MaxParsCount} expected got {pars.Count}");
            var par0 = pars[0];
            var par1 = pars[1];
            if (!(par1 is string))
                throw new error.TypeMismatchError($"{Symbol} function: second paramter should be {ParName(1)}");
            if (par0 == null)
                throw new error.TypeMismatchError($"{Symbol} function: can't get member {par1} from a null data");
            if (!(par0 is KeyValueCollection))
                throw new error.TypeMismatchError($"{Symbol} function: can't get member {par1} from a {FuncScript.GetFsDataType(par0)}");

            return ((KeyValueCollection)par0).Get(((string)par1).ToLower());

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
