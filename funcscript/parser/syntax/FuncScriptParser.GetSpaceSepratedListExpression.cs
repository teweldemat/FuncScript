using funcscript.block;
using funcscript.model;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {

        static ParseResult GetSpaceSepratedListExpression(ParseContext context, int index)
        {
            ParseNode parseNode = null;
            ListExpression listExpr = null;
            var i = SkipSpace(context, index).NextIndex;
            var tokenStart = i;
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
            listExpr.SetContext(context.ReferenceProvider);
            parseNode = new ParseNode(ParseNodeType.List, index, i - tokenStart, nodeListItems);
            return new ExpressionBlockResult(listExpr, parseNode, i);
        }
    }
}