using FuncScript.Core;
using System;
using System.IO;
using FuncScript.Model;

namespace FuncScript.Funcs.OS
{
    internal class MkdirFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;

        public string Symbol => "mkdir";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            if (pars.Length != 1)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"{Symbol} function: invalid parameter count. 1 expected, got {pars.Length}");

            var par0 = pars[0];
            if (par0 == null)
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER,
                    $"Function {Symbol}: parameter cannot be null");

            if (par0 is not string directory)
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER,
                    $"Function {Symbol}: Type mismatch. Parameter must be a string");

            try
            {
                Directory.CreateDirectory(directory);
                return directory;
            }
            catch (Exception ex)
            {
                return new FsError(ex);
            }
        }

        public string ParName(int index)
        {
            return index == 0 ? "path" : null;
        }
    }
}