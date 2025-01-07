using FuncScript.Block;
using FuncScript.Funcs.Math;
using FuncScript.Model;

namespace FuncScript.Core
{
    public partial class FuncScriptParser
    {
        public static List<string> ParseSpaceSepratedList(ParseContext context)
        {
            var (prog, parseNode, i) = GetSpaceSeparatedStringListExpression(context,  0);
            return prog;
        }
    }
}
