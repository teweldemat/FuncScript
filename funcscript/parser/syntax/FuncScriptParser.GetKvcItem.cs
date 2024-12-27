using funcscript.block;
using funcscript.model;
namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        static int GetKvcItem(KeyValueCollection provider, bool nakedKvc, string exp, int index,
            out KvcExpression.KeyValueExpression item,
            out ParseNode parseNode)
        {
            item = null;
            var syntaxErrors = new List<SyntaxErrorData>();
            var i = GetKeyValuePair(provider, exp, index, out item, out parseNode, syntaxErrors);
            if (i > index)
                return i;

            syntaxErrors = new List<SyntaxErrorData>();
            i = GetReturnDefinition(provider, exp, index, out var retExp, out var nodeRetExp, syntaxErrors);
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
                i = GetIdentifier(provider, exp, index, false, out var iden, out var idenLower, out _, out var nodeIden);

                if (i > index)
                {
                    item = new KvcExpression.KeyValueExpression
                    {
                        Key = iden,
                        KeyLower = idenLower,
                        ValueExpression = new ReferenceBlock(iden, idenLower, false)
                        {
                            CodePos = index,
                            CodeLength = i - index
                        }
                    };
                    item.ValueExpression.SetContext(provider);
                    parseNode = nodeIden;
                    return i;
                }

                syntaxErrors = new List<SyntaxErrorData>();
                i = GetSimpleString(provider, exp, index, out iden, out nodeIden, syntaxErrors);
                if (i > index)
                {
                    item = new KvcExpression.KeyValueExpression
                    {
                        Key = iden,
                        KeyLower = idenLower,
                        ValueExpression = new ReferenceBlock(iden, iden.ToLower(), false)
                        {
                            CodePos = index,
                            CodeLength = i - index
                        }
                    };
                    item.ValueExpression.SetContext(provider);
                    parseNode = nodeIden;
                    return i;
                }
            }

            return index;
        }
    }
}