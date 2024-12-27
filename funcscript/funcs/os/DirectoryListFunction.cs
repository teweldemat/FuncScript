using funcscript.core;
using funcscript.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace funcscript.funcs.os
{
    internal class DirectoryListFunction : IFsFunction
    {
        private const int ExpectedParameterCount = 1;

        public CallType CallType => CallType.Prefix;

        public string Symbol => "dirlist";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != ExpectedParameterCount)
                throw new error.EvaluationTimeException($"{this.Symbol} function: invalid parameter count. {ExpectedParameterCount} expected, got {pars.Length}");

            var par0 = pars[0];
            if (par0 == null || !(par0 is string))
                throw new error.TypeMismatchError($"Function {this.Symbol}. Invalid parameter type, expected a string");

            var directoryPath = (string)par0;

            if (!Directory.Exists(directoryPath))
                throw new error.TypeMismatchError($"Function {this.Symbol}. Directory '{directoryPath}' does not exist");
            try
            {
                var files = Directory.GetDirectories(directoryPath).Concat(Directory.GetFiles(directoryPath)).ToArray();
                return new ArrayFsList(files);
            }
            catch (Exception ex)
            {
                throw new error.EvaluationTimeException($"Function {this.Symbol}. Error retrieving files from '{directoryPath}': {ex.Message}");
            }
        }

        public string ParName(int index)
        {
            return index == 0 ? "directory path" : null;
        }
    }
}