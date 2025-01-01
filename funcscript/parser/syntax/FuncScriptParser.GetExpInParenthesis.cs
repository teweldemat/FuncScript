using funcscript.block;
namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static ExpressionBlockResult GetExpInParenthesis(ParseContext context, int index)
        {
            ParseNode parseNode = null;
            ExpressionBlock expBlock = null;
            var i = index;
            i = SkipSpace(context, i).NextIndex;
            var i2 = GetLiteralMatch(context, i, "(").NextIndex;
            if (i == i2)
                return new ExpressionBlockResult(expBlock, parseNode, index);
            i = i2;

            i = SkipSpace(context, i).NextIndex;
            var expressionResult = GetExpression(context, i);
            expBlock = expressionResult.Block;
            var nodeExpression = expressionResult.ParseNode;
            if (expressionResult.NextIndex == i)
                expBlock = null;
            else
                i = expressionResult.NextIndex;
            i = SkipSpace(context, i).NextIndex;
            i2 = GetLiteralMatch(context, i, ")").NextIndex;
            if (i == i2)
            {
                context.SyntaxErrors.Add(new SyntaxErrorData(i, 0, ")' expected"));
                return new ExpressionBlockResult(expBlock, parseNode, index);
            }

            i = i2;
            if (expBlock == null)
                expBlock = new NullExpressionBlock();

            parseNode = new ParseNode(ParseNodeType.ExpressionInBrace, index, i - index, new[] { nodeExpression });
            return new ExpressionBlockResult(expBlock, parseNode, i);
        }
    }
}