using funcscript.core;
using funcscript.model;

namespace funcscript.funcs.keyvalue
{
    internal class KvSelectFunction : IFsFunction
    {
        public int MaxParsCount => 2;

        public CallType CallType => CallType.Prefix;

        public string Symbol => "Select";

        public int Precidence => 0;

        public object Evaluate(IFsDataProvider parent, IParameterList pars)
        {
            if (pars.Count != MaxParsCount)
                throw new error.TypeMismatchError($"{Symbol} function: Invalid parameter count. Expected {MaxParsCount}, but got {pars.Count}");

            var par0 = pars.GetParameter(parent, 0);
            var par1 = pars.GetParameter(parent, 1);

            if (!(par0 is KeyValueCollection))
                throw new error.TypeMismatchError($"{Symbol} function: The first parameter should be {ParName(0)}");

            if (!(par1 is KeyValueCollection))
                throw new error.TypeMismatchError($"{Symbol} function: The second parameter should be {ParName(1)}");

            var first = (KeyValueCollection)par0;
            var second = ((KeyValueCollection)par1).GetAll();

            for (int i = 0; i < second.Count; i++)
            {
                if (second[i].Value == null)
                {
                    var key = second[i].Key.ToLower();
                    var value = first.Get(key);
                    second[i] = new KeyValuePair<string, object>(second[i].Key, value);
                }
            }

            return new SimpleKeyValueCollection(parent, second.ToArray());
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "Source KVC",
                1 => "Target KVC",
                _ => null
            };
        }
    }
}