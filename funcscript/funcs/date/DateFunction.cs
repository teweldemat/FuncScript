using FuncScript.Core;
using System;
using System.Runtime.Serialization;
using FuncScript.Model;
using System.Globalization;

namespace FuncScript.Funcs.Logic
{
    public class DateFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;
        public string Symbol => "Date";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            if (pars.Length > 2)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol} function: invalid parameter count. Max of 2 expected, got {pars.Length}");

            var par0 = pars[0];
            if (par0 == null)
                return null;

            if (!(par0 is string))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"Function {this.Symbol}: Type mismatch, expected string");

            var str = (string)par0;
            DateTime date;
            var par1 = pars.Length > 1 ? pars[1] as string : null;

            if (par1 == null)
            {
                if (!DateTime.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                    return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"Function {this.Symbol}: String '{str}' can't be converted to date");
            }
            else
            {
                var f = new DateTimeFormat(par1);
                if (!DateTime.TryParse(str, f.FormatProvider, DateTimeStyles.None, out date))
                    return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"Function {this.Symbol}: String '{str}' can't be converted to date with format '{par1}'");
            }

            date = DateTime.SpecifyKind(date, DateTimeKind.Unspecified);
            return date;
        }

        public string ParName(int index)
        {
            switch (index)
            {
                case 0:
                    return "Date string";
                case 1:
                    return "Date format";
                default:
                    return "";
            }
        }
    }
}