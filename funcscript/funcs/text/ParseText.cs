using System.Globalization;
using System.Text;
using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.Text
{
    public class ParseText : IFsFunction
    {
        private KeyValueCollection _parentContext;

        public CallType CallType => CallType.Dual;

        public string Symbol => "parse";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            if (pars.Length == 0)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"{this.Symbol} requires at least one parameter");

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

            if (string.IsNullOrEmpty(format))
                return str;
            if (str == null)
                return null;
            try
            {
                switch (format.ToLower())
                {
                    case "hex":
                        var strToParse = str.StartsWith("0x") ? str.Substring(2) : str;
                        if (int.TryParse(strToParse, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var intVal))
                        {
                            return intVal;
                        }
                        else
                        {
                            return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER,
                                $"{this.Symbol}: invalid string: {str}");
                        }                    
                    case "l":
                        if (long.TryParse(str,  out var longVal))
                        {
                            return longVal;
                        }
                        else
                        {
                            return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER,
                                $"{this.Symbol}: invalid string: {str}");
                        }
                    case "fs":
                        return Helpers.Evaluate(context, str);
                    default:
                        return str;
                }
            }
            catch (Exception e)
            {
                return new FsError(e);
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

    }
}