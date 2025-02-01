using FuncScript.Core;
using System;
using FuncScript.Model;

namespace FuncScript.Funcs.Logic
{
    public class NowFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;
        public string Symbol => "Now";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            if (pars.Length > 0)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol} function: invalid parameter count. Expected 0, got {pars.Length}");

            return DateTime.Now;
        }

        public string ParName(int index)
        {
            return "";
        }
    }
} 