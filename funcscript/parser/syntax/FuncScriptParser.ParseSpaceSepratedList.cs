using funcscript.block;
using funcscript.funcs.math;
using funcscript.model;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        public static List<string> ParseSpaceSepratedList(KeyValueCollection context, String exp,
            List<SyntaxErrorData> serrors)
        {
            var i = GetSpaceSepratedStringListExpression(context, exp, 0, out var prog, out var parseNode, serrors);
            return prog;
        }
    }
}