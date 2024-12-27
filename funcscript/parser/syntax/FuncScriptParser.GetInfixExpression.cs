using funcscript.block;
using funcscript.funcs.math;
using funcscript.model;
namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static int GetInfixExpression(KeyValueCollection provider, string exp, int index, out ExpressionBlock expBlock,
            out ParseNode parseNode, List<SyntaxErrorData> syntaxErrors)
        {
            var i = GetInfixExpressionSingleLevel(provider, s_operatorSymols.Length - 1, s_operatorSymols[^1], exp,
                index, out expBlock,
                out parseNode, syntaxErrors);
            
            if (expBlock != null)
            {
                expBlock.SetContext(provider);
            }

            return i;
        }
    }
}