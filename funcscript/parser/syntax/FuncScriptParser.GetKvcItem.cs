using funcscript.block;
using funcscript.model;
namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        public record GetKvcItemResult(KvcExpression.KeyValueExpression Item, ParseNode ParseNode, int NextIndex);

        static GetKvcItemResult GetKvcItem(ParseContext context, bool nakedKvc, int index)
        {
            KvcExpression.KeyValueExpression item = null;
            var result = GetKeyValuePair(context, index);

            if (result.NextIndex > index)
                return new GetKvcItemResult(result.KeyValue, result.ParseNode, result.NextIndex);

            var returnDefResult = GetReturnDefinition(context, index);

            if (returnDefResult.NextIndex > index)
            {
                item = new KvcExpression.KeyValueExpression
                {
                    Key = null,
                    ValueExpression = returnDefResult.Expression
                };
                return new GetKvcItemResult(item, returnDefResult.Node, returnDefResult.NextIndex);
            }

            if (!nakedKvc)
            {
                var identifierResult = GetIdentifier(context, index, false);

                if (identifierResult.NextIndex > index)
                {
                    item = new KvcExpression.KeyValueExpression
                    {
                        Key = identifierResult.Iden,
                        KeyLower = identifierResult.IdenLower,
                        ValueExpression = new ReferenceBlock(identifierResult.Iden, identifierResult.IdenLower, false)
                        {
                            CodePos = index,
                            CodeLength = identifierResult.NextIndex - index
                        }
                    };
                    item.ValueExpression.SetContext(context.Provider);
                    return new GetKvcItemResult(item, identifierResult.ParseNode, identifierResult.NextIndex);
                }

                var simpleStringResult = GetSimpleString(context, index);

                if (simpleStringResult.NextIndex > index)
                {
                    item = new KvcExpression.KeyValueExpression
                    {
                        Key = simpleStringResult.Str,
                        KeyLower = simpleStringResult.Str.ToLower(),
                        ValueExpression = new ReferenceBlock(simpleStringResult.Str, simpleStringResult.Str.ToLower(), false)
                        {
                            CodePos = index,
                            CodeLength = simpleStringResult.NextIndex - index
                        }
                    };
                    item.ValueExpression.SetContext(context.Provider);
                    return new GetKvcItemResult(item, simpleStringResult.Node, simpleStringResult.NextIndex);
                }
            }

            return new GetKvcItemResult(item, null, index);
        }
    }
}