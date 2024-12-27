using funcscript.block;
using funcscript.funcs.math;
using funcscript.model;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static int GetRootExpression(KeyValueCollection provider, string exp, int index, out ExpressionBlock prog,
            out ParseNode parseNode, List<SyntaxErrorData> syntaxErrors)
        {
            var thisErrors = new List<SyntaxErrorData>();
            var i =  GetExpression(provider, exp, index, out prog, out parseNode, syntaxErrors);
            if (i > index)
            {
                prog.SetContext(provider);
                syntaxErrors.AddRange(thisErrors);
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