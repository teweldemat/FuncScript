using funcscript.block;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static int GetKvcItem(IFsDataProvider context, bool nakedKvc, string exp, int index,
            out KvcExpression.KeyValueExpression item,
            out ParseNode parseNode)
        {
            item = null;
            var e1 = new List<SyntaxErrorData>();
            var i = GetKeyValuePair(context, exp, index, out item, out parseNode, e1);
            if (i > index)
                return i;

            var e2 = new List<SyntaxErrorData>();
            i = GetReturnDefinition(context, exp, index, out var retExp, out var nodeRetExp, e2);
            if (i > index)
            {
                item = new KvcExpression.KeyValueExpression
                {
                    Key = null,
                    ValueExpression = retExp
                };
                parseNode = nodeRetExp;
                return i;
            }

            if (!nakedKvc)
            {
                i = GetIdentifier(context, exp, index, out var iden, out var idenLower, out var nodeIden);

                if (i > index)
                {
                    item = new KvcExpression.KeyValueExpression
                    {
                        Key = iden,
                        KeyLower = idenLower,
                        ValueExpression = new ReferenceBlock(iden, idenLower, false)
                        {
                            CodePos = index,
                            CodeLength = i - index,
                            Provider = context
                        }
                    };
                    parseNode = nodeIden;
                    return i;
                }

                var e3 = new List<SyntaxErrorData>();
                i = GetSimpleString(context, exp, index, out iden, out nodeIden, e3);
                if (i > index)
                {
                    item = new KvcExpression.KeyValueExpression
                    {
                        Key = iden,
                        KeyLower = idenLower,
                        ValueExpression = new ReferenceBlock(iden, iden.ToLower(), false)
                        {
                            CodePos = index,
                            CodeLength = i - index,
                            Provider = context
                        }
                    };
                    parseNode = nodeIden;
                    return i;
                }
            }

            return index;
        }
    }
}