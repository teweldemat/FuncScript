using funcscript.core;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static int GetIdentifier(IFsDataProvider parseContext, string exp, int index, bool supportParentRef, out String iden, out String idenLower, out bool parentRef, out ParseNode parseNode)
        {
            parseNode = null;
            iden = null;
            idenLower = null;
            parentRef = false;

            if (index >= exp.Length)
                return index;
            var i1 = index;
            var i = i1;
            if (supportParentRef)
            {
                var i2 = GetLiteralMatch(exp, i, "^");
                if (i2 > i)
                {
                    parentRef = true;
                    i1 = i2;
                    i = i2;
                }
            }

            if (!IsIdentfierFirstChar(exp[i]))
                return index;
            i++;
            while (i < exp.Length && IsIdentfierOtherChar(exp[i]))
            {
                i++;
            }

            iden = exp.Substring(i1, i - i1);
            idenLower = iden.ToLower();
            if (s_KeyWords.Contains(idenLower))
                return index;
            parseNode = new ParseNode(ParseNodeType.Identifier, index, i - index);
            return i;
        }
    }
}