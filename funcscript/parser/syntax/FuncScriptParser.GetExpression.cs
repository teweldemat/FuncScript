using funcscript.model;
namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static int GetExpression(KeyValueCollection provider, String exp, int index, out ExpressionBlock expBlock,
            out ParseNode parseNode, List<SyntaxErrorData> syntaxErrors)
        {
            var i = GetInfixExpression(provider, exp, index, out expBlock, out parseNode, syntaxErrors);
            if (i > index)
                return i;

            return index;
        }
    }
}