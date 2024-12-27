using funcscript.block;
using funcscript.model;
namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static int GetExpInParenthesis(KeyValueCollection provider, String exp, int index,
            out ExpressionBlock expBlock, out ParseNode parseNode, List<SyntaxErrorData> syntaxErrors)
        {
            parseNode = null;
            expBlock = null;
            var i = index;
            i = SkipSpace(exp, i);
            var i2 = GetLiteralMatch(exp, i, "(");
            if (i == i2)
                return index;
            i = i2;

            i = SkipSpace(exp, i);
            i2 = GetExpression(provider, exp, i, out expBlock, out var nodeExpression, syntaxErrors);
            if (i2 == i)
                expBlock = null;
            else
                i = i2;
            i = SkipSpace(exp, i);
            i2 = GetLiteralMatch(exp, i, ")");
            if (i == i2)
            {
                syntaxErrors.Add(new SyntaxErrorData(i, 0, "')' expected"));
                return index;
            }

            i = i2;
            if (expBlock == null)
                expBlock = new NullExpressionBlock();
            expBlock.SetContext(provider);

            parseNode = new ParseNode(ParseNodeType.ExpressionInBrace, index, i - index, new[] { nodeExpression });
            return i;
        }
    }
}