using funcscript.block;
using funcscript.model;
namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static ParseResult GetExpInParenthesis(ParseContext context, int index)
        {
            ParseNode parseNode = null;
            ExpressionBlock expBlock = null;
            var i = index;
            i = SkipSpace(context, i).NextIndex;
            var i2 = GetLiteralMatch(context, i, "(").NextIndex;
            if (i == i2)
                return new ParseResult(expBlock, parseNode, index);
            i = i2;

            i = SkipSpace(context, i).NextIndex;
            var expressionResult = GetExpression(context, i);
            expBlock = expressionResult.Expression;
            var nodeExpression = expressionResult.Node;
            if (expressionResult.NextIndex == i)
                expBlock = null;
            else
                i = expressionResult.NextIndex;
            i = SkipSpace(context, i).NextIndex;
            i2 = GetLiteralMatch(context, i, ")").NextIndex;
            if (i == i2)
            {
                context.Serrors.Add(new SyntaxErrorData(i, 0, "')' expected"));
                return new ParseResult(expBlock, parseNode, index);
            }

            i = i2;
            if (expBlock == null)
                expBlock = new NullExpressionBlock();
            expBlock.SetContext(context.Provider);

            parseNode = new ParseNode(ParseNodeType.ExpressionInBrace, index, i - index, new[] { nodeExpression });
            return new ParseResult(expBlock, parseNode, i);
        }
    }
}