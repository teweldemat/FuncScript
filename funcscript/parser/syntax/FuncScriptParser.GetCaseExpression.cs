using funcscript.block;
using funcscript.model;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static ExpressionBlockResult GetCaseExpression(ParseContext context, int index)
        {
            ExpressionBlock expBlock = null;
            ParseNode parseNode = null;
            var i = index;
            var literalMatchResult = GetLiteralMatch(context, i, KW_CASE);
            if (literalMatchResult.NextIndex == i)
                return new ExpressionBlockResult(null, null, index);
            
            i = SkipSpace(context, literalMatchResult.NextIndex).NextIndex;
            var pars = new List<ExpressionBlock>();
            var childNodes = new List<ParseNode>();
            do
            {
                if (pars.Count == 0)
                {
                    var expressionResult = GetExpression(context, i);
                    if (expressionResult.NextIndex == i)
                    {
                        context.SyntaxErrors.Add(new SyntaxErrorData(i, 1, "Case condition expected"));
                        return new ExpressionBlockResult(null, null, index);
                    }

                    pars.Add(expressionResult.Expression);
                    childNodes.Add(expressionResult.Node);
                    i = SkipSpace(context, expressionResult.NextIndex).NextIndex;
                }
                else
                {
                    literalMatchResult = GetLiteralMatch(context, i, ",", ";");
                    if (literalMatchResult.NextIndex == i)
                        break;
                    i = SkipSpace(context, literalMatchResult.NextIndex).NextIndex;
                    var expressionResult = GetExpression(context, i);
                    if (expressionResult.NextIndex == i)
                        break;
                    pars.Add(expressionResult.Expression);
                    childNodes.Add(expressionResult.Node);
                    i = SkipSpace(context, expressionResult.NextIndex).NextIndex;
                }

                literalMatchResult = GetLiteralMatch(context, i, ":");
                if (literalMatchResult.NextIndex == i)
                {
                    break;
                }

                i = SkipSpace(context, literalMatchResult.NextIndex).NextIndex;
                var valueExpressionResult = GetExpression(context, i);
                if (valueExpressionResult.NextIndex == i)
                {
                    context.SyntaxErrors.Add(new SyntaxErrorData(i, 1, "Case value expected"));
                    return new ExpressionBlockResult(null, null, index);
                }

                pars.Add(valueExpressionResult.Expression);
                childNodes.Add(valueExpressionResult.Node);
                i = SkipSpace(context, valueExpressionResult.NextIndex).NextIndex;
            } while (true);

            expBlock = new FunctionCallExpression
            {
                Function = new LiteralBlock(context.Provider.Get(KW_CASE)),
                CodePos = index,
                CodeLength = i - index,
                Parameters = pars.ToArray(),
            };
            expBlock.SetContext(context.Provider);
            parseNode = new ParseNode(ParseNodeType.Case, index, i - index);
            parseNode.Childs = childNodes;
            return new ExpressionBlockResult(expBlock, parseNode, i);
        }
    }
}