using FuncScript.Core;
using FuncScript.Model;

using System.Runtime.InteropServices.JavaScript;

namespace FsStudio.Server.FileSystem.Exec.Funcs
{
    internal class MarkDownFunction(RemoteLogger remoteLogger, string sessionId) : IFsFunction
    {
        private const int MaxParameters = 1;
        public CallType CallType => CallType.Prefix;
        public string Symbol => "MarkDown";

        public object EvaluateList(FsList pars)
        {
            if (pars.Length != MaxParameters)
                return new FsError(
                    FsError.ERROR_PARAMETER_COUNT_MISMATCH,
                    $"{Symbol} function: invalid parameter count. {MaxParameters} expected, got {pars.Length}"
                );

            var par0 = pars[0];
            if (par0 is null)
                remoteLogger.SendMarkdDown(sessionId, "");
            else if (par0 is string str)
            {
                string content = (string)par0;
                remoteLogger.SendMarkdDown(sessionId, str);
            }
            else
            {
                remoteLogger.SendMarkdDown(sessionId, $"Unsupported type {FuncScript.FuncScript.GetFsDataType(par0)} for markdown");
            }
            return par0;
        }

        public string ParName(int index)
        {
            return index switch
            {
                0 => "content",
                _ => ""
            };
        }
    }
}
