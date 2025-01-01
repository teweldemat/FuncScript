using funcscript.core;
using funcscript.model;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        record GetIdentifierResult(string Iden, string IdenLower, bool ParentRef, ParseNode ParseNode, int NextIndex)
            : ParseResult(ParseNode, NextIndex);

        static GetIdentifierResult GetIdentifier(ParseContext context, int index, bool supportParentRef)
        {
            string iden = null;
            string idenLower = null;
            bool parentRef = false;
            ParseNode parseNode = null;

            var exp = context.Expression;

            if (index >= exp.Length)
                return new GetIdentifierResult(iden, idenLower, parentRef, parseNode, index);
            var i1 = index;
            var i = i1;
            if (supportParentRef)
            {
                var i2 = GetLiteralMatch(context, i, "^").NextIndex;
                if (i2 > i)
                {
                    parentRef = true;
                    i1 = i2;
                    i = i2;
                }
            }

            if (!IsIdentifierFirstChar(exp[i]))
                return new GetIdentifierResult(iden, idenLower, parentRef, parseNode, index);
            i++;
            while (i < exp.Length && IsIdentfierOtherChar(exp[i]))
            {
                i++;
            }

            iden = exp.Substring(i1, i - i1);
            idenLower = iden.ToLower();
            if (s_KeyWords.Contains(idenLower))
                return new GetIdentifierResult(iden, idenLower, parentRef, parseNode, index);

            parseNode = new ParseNode(ParseNodeType.Identifier, index, i - index);
            return new GetIdentifierResult(iden, idenLower, parentRef, parseNode, i);
        }
    }
}