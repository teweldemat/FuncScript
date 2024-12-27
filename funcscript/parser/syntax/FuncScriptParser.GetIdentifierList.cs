using funcscript.block;
using funcscript.funcs.math;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static int GetIdentifierList(IFsDataProvider parseContext, string exp, int index, out List<string> idenList, out ParseNode parseNode)
        {
            parseNode = null;
            idenList = null;
            int i = SkipSpace(exp, index);
            //get open brace
            if (i >= exp.Length || exp[i++] != '(')
                return index;

            idenList = new List<string>();
            var parseNodes = new List<ParseNode>();
            //get first identifier
            i = SkipSpace(exp, i);
            int i2 = GetIdentifier(parseContext, exp, i, out var iden, out var idenLower, out var nodeIden);
            if (i2 > i)
            {
                parseNodes.Add(nodeIden);
                idenList.Add(iden);
                i = i2;

                //get additional identifiers separated by commas
                i = SkipSpace(exp, i);
                while (i < exp.Length)
                {
                    if (exp[i] != ',')
                        break;
                    i++;
                    i = SkipSpace(exp, i);
                    i2 = GetIdentifier(parseContext, exp, i, out iden, out idenLower, out nodeIden);
                    if (i2 == i)
                        return index;
                    parseNodes.Add(nodeIden);
                    idenList.Add(iden);
                    i = i2;
                    i = SkipSpace(exp, i);
                }
            }

            //get close brace
            if (i >= exp.Length || exp[i++] != ')')
                return index;
            parseNode = new ParseNode(ParseNodeType.IdentiferList, index, i - index, parseNodes);
            return i;
        }
    }
}