using FuncScript.Block;
using FuncScript.Funcs.Math;
using FuncScript.Model;
namespace FuncScript.Core
{
    public partial class FuncScriptParser
    {
        record GetIdentifierListResult(List<string> IdenList, ParseNode ParseNode, int NextIndex)
            :ParseResult(ParseNode, NextIndex);

        static GetIdentifierListResult GetIdentifierList(ParseContext context, int index)
        {
            int i = SkipSpace(context, index).NextIndex;

            if (i >= context.Expression.Length || context.Expression[i++] != '(')
                return new GetIdentifierListResult(null, null, index);

            var idenList = new List<string>();
            var parseNodes = new List<ParseNode>();

            i = SkipSpace(context, i).NextIndex;
            var (iden, idenLower, _, nodeIden, i2) = GetIdentifier(context, i, false);

            if (i2 > i)
            {
                parseNodes.Add(nodeIden);
                idenList.Add(iden);
                i = i2;

                i = SkipSpace(context, i).NextIndex;
                while (i < context.Expression.Length)
                {
                    if (context.Expression[i] != ',')
                        break;

                    i++;
                    i = SkipSpace(context, i).NextIndex;
                    (iden,  idenLower, _, nodeIden, i2) = GetIdentifier(context, i, false);

                    if (i2 == i)
                        return new GetIdentifierListResult(null, null, index);

                    parseNodes.Add(nodeIden);
                    idenList.Add(iden);
                    i = i2;
                    i = SkipSpace(context, i).NextIndex;
                }
            }

            if (i >= context.Expression.Length || context.Expression[i++] != ')')
                return new GetIdentifierListResult(null, null, index);

            var parseNode = new ParseNode(ParseNodeType.IdentifierList, index, i - index, parseNodes);
            return new GetIdentifierListResult(idenList, parseNode, i);
        }
    }
}
