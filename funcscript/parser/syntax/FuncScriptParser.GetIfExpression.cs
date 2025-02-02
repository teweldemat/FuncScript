using FuncScript.Block;
using FuncScript.Model;

namespace FuncScript.Core
{
    public partial class FuncScriptParser
    {
        static ExpressionBlockResult GetIfExpression(ParseContext context, int index)
        {
            ExpressionBlock expBlock = null;
            ParseNode parseNode = null;
            var i = index;

            var literalMatchResult = GetLiteralMatch(context, i, KW_IF);
            if (literalMatchResult.NextIndex == i)
                return new ExpressionBlockResult(null, null, index);

            i = SkipSpace(context, literalMatchResult.NextIndex).NextIndex;

            var conditionResult = GetExpression(context, i);
            if (conditionResult.NextIndex == i)
            {
                context.SyntaxErrors.Add(new SyntaxErrorData(i, 1, "If condition expected"));
                return new ExpressionBlockResult(null, null, index);
            }

            var pars = new List<ExpressionBlock>();
            var childNodes = new List<ParseNode>();
            pars.Add(conditionResult.Block);
            childNodes.Add(conditionResult.ParseNode);
            i = SkipSpace(context, conditionResult.NextIndex).NextIndex;

            literalMatchResult = GetLiteralMatch(context, i, KW_THEN);
            if (literalMatchResult.NextIndex == i)
            {
                context.SyntaxErrors.Add(new SyntaxErrorData(i, 1, "Keyword 'then' expected"));
                return new ExpressionBlockResult(null, null, index);
            }

            i = SkipSpace(context, literalMatchResult.NextIndex).NextIndex;

            var trueExprResult = GetExpression(context, i);
            if (trueExprResult.NextIndex == i)
            {
                context.SyntaxErrors.Add(new SyntaxErrorData(i, 1, "Then expression expected"));
                return new ExpressionBlockResult(null, null, index);
            }

            pars.Add(trueExprResult.Block);
            childNodes.Add(trueExprResult.ParseNode);
            i = SkipSpace(context, trueExprResult.NextIndex).NextIndex;

            literalMatchResult = GetLiteralMatch(context, i, KW_ELSE);
            if (literalMatchResult.NextIndex != i)
            {
                i = SkipSpace(context, literalMatchResult.NextIndex).NextIndex;

                var elseExprResult = GetExpression(context, i);
                if (elseExprResult.NextIndex == i)
                {
                    context.SyntaxErrors.Add(new SyntaxErrorData(i, 1, "Else expression expected"));
                    return new ExpressionBlockResult(null, null, index);
                }

                pars.Add(elseExprResult.Block);
                childNodes.Add(elseExprResult.ParseNode);
                i = SkipSpace(context, elseExprResult.NextIndex).NextIndex;
            }

            expBlock = new FunctionCallExpression
            {
                Function = new LiteralBlock(context.ReferenceProvider.Get(KW_IF)),
                CodePos = index,
                CodeLength = i - index,
                Parameters = pars.ToArray()
            };

            parseNode = new ParseNode(ParseNodeType.IfExpression, index, i - index);
            parseNode.Children = childNodes;

            return new ExpressionBlockResult(expBlock, parseNode, i);
        }
    }
}