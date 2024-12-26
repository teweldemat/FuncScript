using funcscript.core;
using System;
using System.IO;
using funcscript.model;

namespace funcscript.funcs.os
{
    internal class SaveFileFunction : IFsFunction
    {
        public int MaxParsCount => 2;

        public CallType CallType => CallType.Prefix;

        public string Symbol => "SaveFile";

        public int Precidence => 0;

        public object Evaluate(IFsDataProvider parent, IParameterList pars)
        {
            if (pars.Count != this.MaxParsCount)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol} function: invalid parameter count. {this.MaxParsCount} expected, got {pars.Count}");

            var par0 = pars.GetParameter(parent, 0);
            var par1 = pars.GetParameter(parent, 1);

            if (par0 == null || par1 == null)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"Function {this.Symbol}: parameters cannot be null");

            if (!(par0 is string) || !(par1 is string))
                return new FsError(FsError.ERROR_TYPE_MISMATCH, $"Function {this.Symbol}: Type mismatch. Both parameters must be strings");

            var fileName = (string)par0;
            var content = (string)par1;

            try
            {
                // Save content to the specified file
                File.WriteAllText(fileName, content);
                Console.WriteLine($"Saved {fileName}");
                return content; // Return the content if saved successfully
            }
            catch (Exception ex)
            {
                // Return a file system error if any exception occurs
                return new FsError(FsError.ERROR_TYPE_EVALUATION, $"Function {this.Symbol}: Failed to save file '{fileName}'. Error: {ex.Message}");
            }
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "file name",
                1 => "content",
                _ => null
            };
        }
    }
}