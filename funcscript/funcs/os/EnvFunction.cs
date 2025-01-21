using FuncScript.Core;
using FuncScript.Model;

namespace FuncScript.Funcs.OS
{
    internal class EnvFunction : IFsFunction
    {
        public CallType CallType => CallType.Prefix;

        public string Symbol => "osenv";

        public object EvaluateList(KeyValueCollection context, FsList pars)
        {
            const int MaxParameters = 1;
            if (pars.Length != MaxParameters)
                return new FsError(FsError.ERROR_PARAMETER_COUNT_MISMATCH, $"{this.Symbol}: {MaxParameters} parameter expected, got {pars.Length}");
                
            var par0 = pars[0];

            if (par0 == null)
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: parameter cannot be null");

            if (!(par0 is string))
                return new FsError(FsError.ERROR_TYPE_INVALID_PARAMETER, $"{this.Symbol}: string expected");

            var variableName = (string)par0;
            var value = System.Environment.GetEnvironmentVariable(variableName);
            
            if (value == null)
                return new FsError(FsError.ERROR_TYPE_EVALUATION, $"{this.Symbol}: environment variable '{variableName}' not found");
                
            return value;
        }

        public string ParName(int index)
        {
            switch (index)
            {
                case 0: return "variable name";
                default:
                    return null;
            }
        }
    }
} 