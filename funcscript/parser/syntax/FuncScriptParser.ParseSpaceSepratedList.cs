using funcscript.block;
using funcscript.funcs.math;
using funcscript.model;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        public static List<string> ParseSpaceSepratedList(KeyValueCollection provider, String exp,
            List<SyntaxErrorData> syntaxErrors)
        {
            var i = GetSpaceSepratedStringListExpression(provider, exp, 0, out var prog, out var parseNode, syntaxErrors);
            return prog;
        }
    }
}