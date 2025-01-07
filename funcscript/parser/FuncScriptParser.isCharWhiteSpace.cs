using FuncScript.Model;

namespace FuncScript.Core
{
    public partial class FuncScriptParser
    {
        static bool IsCharWhiteSpace(char ch)
            => ch == ' ' ||
               ch == '\r' ||
               ch == '\t' ||
               ch == '\n';

    }
}
