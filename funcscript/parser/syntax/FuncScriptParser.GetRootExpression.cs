using funcscript.block;
using funcscript.funcs.math;
using funcscript.model;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static int GetRootExpression(KeyValueCollection parseContext, string exp, int index, out ExpressionBlock prog,
            out ParseNode parseNode, List<SyntaxErrorData> serrors)
        {
            var thisErrors = new List<SyntaxErrorData>();
            var i =  GetExpression(parseContext, exp, index, out prog, out parseNode, serrors);
            if (i > index)
            {
                prog.SetContext(parseContext);
                serrors.AddRange(thisErrors);
                return i;
            }
            return index;
            /*var thisErrors = new List<SyntaxErrorData>();
            var i = GetKvcExpression(parseContext, false, exp, index, out var kvc, out parseNode, thisErrors);
            if (i > index)
            {
                prog = kvc;
                prog.SetContext(parseContext);
                serrors.AddRange(thisErrors);
                return i;
            }

            thisErrors = new List<SyntaxErrorData>();
            i = GetExpression(parseContext, exp, index, out prog, out parseNode, serrors);
            if (i > index)
            {
                prog.SetContext(parseContext);
                serrors.AddRange(thisErrors);
                return i;
            }
            return index;*/
        }
    }
}