using funcscript.block;
using funcscript.model;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        public record GetListExpressionResult(ListExpression ListExpr, ParseNode ParseNode, int NextIndex);

        static GetListExpressionResult GetListExpression(ParseContext context, int index)
        {
            var exp = context.Expression;
            var provider = context.Provider;
            var syntaxErrors = context.SyntaxErrors;

            var i = SkipSpace(context, index).NextIndex;
            var i2 = GetLiteralMatch(context, i, "[").NextIndex;
            if (i2 == i)
                return new GetListExpressionResult(null, null, index); // we didn't find '['
            i = i2;

            var listItems = new List<ExpressionBlock>();
            var nodeListItems = new List<ParseNode>();
            i = SkipSpace(context, i).NextIndex;
            var expressionResult = GetExpression(context, i);
            if (expressionResult.NextIndex > i)
            {
                listItems.Add(expressionResult.Expression);
                nodeListItems.Add(expressionResult.Node);
                i = expressionResult.NextIndex;
                do
                {
                    i = SkipSpace(context, i).NextIndex;
                    i2 = GetLiteralMatch(context, i, ",").NextIndex;
                    if (i2 == i)
                        break;
                    i = i2;

                    i = SkipSpace(context, i).NextIndex;
                    expressionResult = GetExpression(context, i);
                    if (expressionResult.NextIndex == i)
                        break;
                    listItems.Add(expressionResult.Expression);
                    nodeListItems.Add(expressionResult.Node);
                    i = expressionResult.NextIndex;
                } while (true);
            }

            i = SkipSpace(context, i).NextIndex;
            i2 = GetLiteralMatch(context, i, "]").NextIndex;
            if (i2 == i)
            {
                syntaxErrors.Add(new SyntaxErrorData(i, 0, "']' expected"));
                return new GetListExpressionResult(null, null, index);
            }

            i = i2;
            var listExpr = new ListExpression
            {
                ValueExpressions = listItems.ToArray()
            };
            listExpr.SetContext(provider);
            var parseNode = new ParseNode(ParseNodeType.List, index, i - index, nodeListItems);
            return new GetListExpressionResult(listExpr, parseNode, i);
        }
    }
}