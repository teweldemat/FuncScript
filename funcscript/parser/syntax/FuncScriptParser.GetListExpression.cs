using FuncScript.Block;
using FuncScript.Model;

namespace FuncScript.Core
{
    public partial class FuncScriptParser
    {
        static ExpressionBlockResult GetListExpression(ParseContext context, int index)
        {
            var exp = context.Expression;
            var provider = context.ReferenceProvider;
            var syntaxErrors = context.SyntaxErrors;

            var i = SkipSpace(context, index).NextIndex;
            
            var i2 = GetLiteralMatch(context, i, "[").NextIndex;
            if (i2 == i)
                return new ExpressionBlockResult(null, null, index); // we didn't find '['
            var tokenStart = i2;
            i = i2;

            var listItems = new List<ExpressionBlock>();
            var nodeListItems = new List<ParseNode>();
            i = SkipSpace(context, i).NextIndex;
            var expressionResult = GetExpression(context, i);
            if (expressionResult.NextIndex > i)
            {
                listItems.Add(expressionResult.Block);
                nodeListItems.Add(expressionResult.ParseNode);
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
                    listItems.Add(expressionResult.Block);
                    nodeListItems.Add(expressionResult.ParseNode);
                    i = expressionResult.NextIndex;
                } while (true);
            }

            i = SkipSpace(context, i).NextIndex;
            i2 = GetLiteralMatch(context, i, "]").NextIndex;
            if (i2 == i)
            {
                syntaxErrors.Add(new SyntaxErrorData(i, 0, "]' expected"));
                return new ExpressionBlockResult(null, null, index);
            }

            i = i2;
            var listExpr = new ListExpression
            {
                ValueExpressions = listItems.ToArray()
            };
            var parseNode = new ParseNode(ParseNodeType.List, index, i - tokenStart, nodeListItems);
            return new ExpressionBlockResult(listExpr, parseNode, i);
        }
    }
}
