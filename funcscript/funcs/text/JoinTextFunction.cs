using funcscript.core;
using funcscript.model;
using System.Text;

namespace funcscript.funcs.text
{
    public class JoinTextFunction : IFsFunction
    {
        public const string SYMBOL = "join";
        private const int MAX_PAR_COUNT = 2;

        public CallType CallType => CallType.Dual;

        public string Symbol => SYMBOL;

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != MAX_PAR_COUNT)
                throw new funcscript.error.TypeMismatchError($"{this.Symbol}: Two parameters expected");

            var par0 = pars[0];
            var par1 = pars[1];

            if (par0 == null || par1 == null)
                throw new funcscript.error.TypeMismatchError($"{this.Symbol}: List and separator expected as parameters");
            if (!(par0 is FsList list))
                throw new InvalidOperationException($"{this.Symbol}: first parameter should be list");
            if (!(par1 is string separator))
                throw new InvalidOperationException($"{this.Symbol}: second parameter should be string");

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Length; i++)
            {
                var item = list[i];

                if (item != null)
                {
                    if (i > 0)
                        sb.Append(separator);
                    sb.Append(item ?? "");
                }
            }
            return sb.ToString();
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "List",
                1 => "Separator",
                _ => ""
            };
        }
    }
}