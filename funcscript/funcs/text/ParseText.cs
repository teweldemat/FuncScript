﻿using funcscript.core;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using funcscript.model;

namespace funcscript.funcs.text
{
    public class ParseText : IFsFunction
    {
        public int MaxParsCount => 2;

        public CallType CallType => CallType.Prefix;

        public string Symbol => "parse";

        public int Precidence => 0;

        public object Evaluate(IFsDataProvider parent, IParameterList pars)
        {
            if (pars.Count == 0)
                throw new error.TypeMismatchError($"{this.Symbol} requires at least one parameter");
            var par0 = pars.GetParameter(parent, 0);
            if (par0 is ValueReferenceDelegate)
                return CallRef.Create(parent, this, pars);
            if (par0 == null)
                return null;
            var str = par0.ToString();
            object par1;
            string format = null;
            if (pars.Count > 1 && (par1 = pars.GetParameter(parent, 1)) != null)
            {
                if (par1 is ValueReferenceDelegate)
                    return CallRef.Create(parent, this, pars);
                format = par1.ToString();
            }
            if (format == null)
                return str;
            switch (format)
            {
                case "hex":
                    if (str.StartsWith("0x"))
                        return Convert.ToInt32(str, 16);
                    return Convert.ToInt32("0x" + str, 16);
                case "l":
                    return Convert.ToInt64(str);
                case "fs":
                    return FuncScript.Evaluate(parent, str);
            }
            return str;
        }
        public string ParName(int index)
        {
            switch (index)
            {
                case 0: return "text";
                case 1: return "format";
            }
            return null;
        }
    }
}
