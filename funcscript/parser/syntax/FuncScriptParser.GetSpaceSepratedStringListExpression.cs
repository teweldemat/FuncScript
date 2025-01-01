using funcscript.model;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        record GetSpaceSeparatedStringListExpressionResult(List<string> StringList, ParseNode ParseNode, int NextIndex)
            :ParseResult(ParseNode,NextIndex);
        static GetSpaceSeparatedStringListExpressionResult GetSpaceSeparatedStringListExpression(ParseContext context, int index)
        {
            var i = SkipSpace(context, index).NextIndex;
            var listItems = new List<string>();
            var nodeListItems = new List<ParseNode>();
            
            string otherItem;
            ParseNode otherNode;
            var (firstItem, firstNode, i2) = GetSimpleString(context,i);
            if (i2 == i)
                (firstItem,firstNode, i2) = GetSpaceLessString(context,i);
            if (i2 > i)
            {
                listItems.Add(firstItem);
                nodeListItems.Add(firstNode);
                i = i2;
                do
                {
                    i2 = GetLiteralMatch(context, i, " ").NextIndex;
                    if (i2 == i)
                        break;
                    i = i2;
                    i = SkipSpace(context, i).NextIndex;
                    (otherItem,otherNode, i2) = GetSimpleString(context, i);
                    if (i2 == i)
                        (otherItem,otherNode,i2) = GetSpaceLessString(context,i);

                    if (i2 == i)
                        break;
                    listItems.Add(otherItem);
                    nodeListItems.Add(otherNode);
                    i = i2;
                } while (true);
            }

            var parseNode = new ParseNode(ParseNodeType.List, index, i - index, nodeListItems);
            return new GetSpaceSeparatedStringListExpressionResult(listItems, parseNode, i);
        }
    }
}