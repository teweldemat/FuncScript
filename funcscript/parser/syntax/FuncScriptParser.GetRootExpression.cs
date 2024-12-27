using funcscript.block;
using funcscript.funcs.math;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static int GetRootExpression(IFsDataProvider parseContext, string exp, int index, out ExpressionBlock prog,
            out ParseNode parseNode, List<SyntaxErrorData> serrors)
        {
            var thisErrors = new List<SyntaxErrorData>();
            var i = GetKvcExpression(parseContext, true, exp, index, out var kvc, out parseNode, thisErrors);
            if (i > index)
            {
                prog = kvc;
                prog.Provider = parseContext;
                serrors.AddRange(thisErrors);
                return i;
            }

            thisErrors = new List<SyntaxErrorData>();
            i = GetExpression(parseContext, exp, index, out prog, out parseNode, serrors);
            if (i > index)
            {
                prog.Provider = parseContext;
                serrors.AddRange(thisErrors);
                return i;
            }
            return index;
        }
    }
}