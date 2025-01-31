using funcscript.core;
using funcscript.model;

namespace funcscript.funcs.os
{
    internal class FileTextFunction : IFsFunction
    {
        private const int MaxParameters = 1;

        public CallType CallType => CallType.Prefix;

        public string Symbol => "file";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != MaxParameters)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol} function: invalid parameter count. {MaxParameters} expected got {pars.Length}");
            var par0 = pars[0];

            if (par0 == null)
                return null;

            if (!(par0 is string))
                return new FsError(FsError.ERROR_TYPE_MISMATCH, $"Function {this.Symbol}. Type mismatch");

            var fileName = (string)par0;
            if (!System.IO.File.Exists(fileName))
                return new FsError(FsError.ERROR_TYPE_EVALUATION, $"Function {this.Symbol}. File '{par0}' doesn't exist");
            if (new System.IO.FileInfo(fileName).Length > 1000000)
                return new FsError(FsError.ERROR_TYPE_EVALUATION, $"Function {this.Symbol}. File '{par0}' is too big");
            return System.IO.File.ReadAllText(fileName);
        }

        public string ParName(int index)
        {
            switch (index)
            {
                case 0: return "file name";
                default:
                    return null;
            }
        }
    }
}