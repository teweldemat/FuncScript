using funcscript.block;
using funcscript.model;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        public record GetSpaceSepratedListExpressionResult(ListExpression ListExpression, ParseNode ParseNode, int NextIndex);

        static GetSpaceSepratedListExpressionResult GetSpaceSepratedListExpression(ParseContext context, int index)
        {
            ParseNode parseNode = null;
            ListExpression listExpr = null;
            var i = SkipSpace(context, index).NextIndex;

            var listItems = new List<ExpressionBlock>();
            var nodeListItems = new List<ParseNode>();
            (var firstItem,var nodeFirstItem, var i2) = GetExpression(context, i);
            if (i2 > i)
            {
                listItems.Add(firstItem);
                nodeListItems.Add(nodeFirstItem);
                i = i2;
                do
                {
                    i2 = GetLiteralMatch(context, i, new[] { " " }).NextIndex;
                    if (i2 == i)
                        break;
                    i = i2;
                    i = SkipSpace(context, i).NextIndex;
                    (var otherItem,var nodeOtherItem, i2) = GetExpression(context, i);
                    if (i2 == i)
                        break;
                    listItems.Add(otherItem);
                    nodeListItems.Add(nodeOtherItem);
                    i = i2;
                } while (true);
            }

            listExpr = new ListExpression { ValueExpressions = listItems.ToArray() };
            listExpr.SetContext(context.Provider);
            parseNode = new ParseNode(ParseNodeType.List, index, i - index, nodeListItems);
            return new GetSpaceSepratedListExpressionResult(listExpr, parseNode, i);
        }
    }
}