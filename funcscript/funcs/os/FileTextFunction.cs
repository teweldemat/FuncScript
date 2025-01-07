using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.OS
{
    internal class FileTextFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;

        public string Symbol => "file";

        public object EvaluateList(FsList pars)
        {
            const int MaxParameters = 1; // Moved const declaration in method scope
            if (pars.Length != MaxParameters)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol}: {MaxParameters} parameter expected, got {pars.Length}");
                
            var par0 = pars[0];

            if (par0 == null)
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: parameter cannot be null");

            if (!(par0 is string))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: string expected");

            var fileName = (string)par0;
            if (!System.IO.File.Exists(fileName))
                return new FsError(FsError.ERROR_TYPE_EVALUATION, $"{this.Symbol}: file '{par0}' doesn't exist");
            if (new System.IO.FileInfo(fileName).Length > 1000000)
                return new FsError(FsError.ERROR_TYPE_EVALUATION, $"{this.Symbol}: file '{par0}' is too big");
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
