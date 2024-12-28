using funcscript.core;
using funcscript.model;

namespace funcscript.funcs.text
{
    public class ParseText : IFsFunction, KeyValueCollection
    {
        private KeyValueCollection _parentContext;
        private const int MaxParameters = 2;

        public CallType CallType => CallType.Prefix;

        public string Symbol => "parse";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length == 0)
                throw new error.TypeMismatchError($"{this.Symbol} requires at least one parameter");

            var par0 = pars[0];
            
            if (par0 == null)
                return null;
            
            var str = par0.ToString();
            object par1;
            string format = null;
            if (pars.Length > 1)
            {
                par1 = pars[1];
                format = par1?.ToString();
            }

            return ParseAccordingToFormat(str, format);
        }

        private object ParseAccordingToFormat(string str, string format)
        {
            if (string.IsNullOrEmpty(format))
                return str;
            if (str == null)
                return null;
            switch (format.ToLower())
            {
                case "hex":
                    if (str.StartsWith("0x"))
                        return Convert.ToInt32(str, 16);
                    return Convert.ToInt32("0x" + str, 16);
                case "l":
                    return Convert.ToInt64(str);
                case "fs":
                    return FuncScript.Evaluate(this, str); // Assuming parent context is not needed or it is correctly handled within FuncScript.Evaluate
                default:
                    return str;
            }
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "text",
                1 => "format",
                _ => null
            };
        }

        public object Get(string name)
        {
            return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"The parsed function script should have no variables");
        }


        public KeyValueCollection ParentContext { get; }
        public bool IsDefined(string key)
        {
            throw new NotImplementedException();
        }

        public IList<string> GetAllKeys()
        {
            throw new NotImplementedException();
        }
    }
}