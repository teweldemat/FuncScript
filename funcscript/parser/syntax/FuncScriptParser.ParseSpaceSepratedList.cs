using funcscript.block;
using funcscript.funcs.math;
using funcscript.model;

namespace funcscript.core
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