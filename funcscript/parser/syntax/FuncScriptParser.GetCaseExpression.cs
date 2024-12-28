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

                    pars.Add(expressionResult.Block);
                    childNodes.Add(expressionResult.ParseNode);
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
                    pars.Add(expressionResult.Block);
                    childNodes.Add(expressionResult.ParseNode);
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

                pars.Add(valueExpressionResult.Block);
                childNodes.Add(valueExpressionResult.ParseNode);
                i = SkipSpace(context, valueExpressionResult.NextIndex).NextIndex;
            } while (true);

            expBlock = new FunctionCallExpression
            {
                Function = new LiteralBlock(context.ReferenceProvider.Get(KW_CASE)),
                CodePos = index,
                CodeLength = i - index,
                Parameters = pars.ToArray(),
            };
            expBlock.SetContext(context.ReferenceProvider);
            parseNode = new ParseNode(ParseNodeType.Case, index, i - index);
            parseNode.Children = childNodes;
            return new ExpressionBlockResult(expBlock, parseNode, i);
        }
    }
}