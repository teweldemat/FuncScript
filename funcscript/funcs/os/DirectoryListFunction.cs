using FuncScript.Core;
using FuncScript.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FuncScript.Funcs.OS
{
    internal class DirectoryListFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;

        public string Symbol => "dirlist";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != 1)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol} function: invalid parameter count. 1 expected, got {pars.Length}");

            var par0 = pars[0];
            if (par0 == null || !(par0 is string))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"Function {this.Symbol}. Invalid parameter type, expected a string");

            var directoryPath = (string)par0;

            if (!Directory.Exists(directoryPath))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"Function {this.Symbol}. Directory '{directoryPath}' does not exist");
            try
            {
                var files = Directory.GetDirectories(directoryPath).Concat(Directory.GetFiles(directoryPath)).ToArray();
                return new ArrayFsList(files);
            }
            catch (Exception ex)
            {
                return new FsError(ex);
            }
        }

        public string ParName(int index)
        {
            return index == 0 ? "directory path" : null;
        }
    }
}
