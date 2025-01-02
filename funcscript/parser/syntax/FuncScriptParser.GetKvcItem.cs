using funcscript.block;
using funcscript.model;

namespace funcscript.core
{
    public partial class FuncScriptParser
    {
        record GetKvcItemResult(KvcExpression.KeyValueExpression Item, ParseNode ParseNode, int NextIndex)
            : ParseResult(ParseNode, NextIndex);

        static GetKvcItemResult GetKvcItem(ParseContext context, int index, bool allowKeyOnly, bool allowReturn,
            bool allowImplicitReturn)
        {
            ParseNode parseNode = null;
            KvcExpression.KeyValueExpression item = null;

            // First, try to parse a "key" (could be a SimpleString or Identifier).
            // Merge logic from GetKeyValuePair:
            string nameLower;
            var tokenStart = index;
            var (name, nodeName, i) = GetSimpleString(context, index);
            if (i == index)
            {
                (name, nameLower, _, nodeName, i) = GetIdentifier(context, index, false);
                if (i == index)
                {
                    // No key parsed -> fallback to return block if allowed
                    if (!allowReturn)
                        return new GetKvcItemResult(null, null, index);

                    var returnDefResult = GetReturnDefinition(context, index, allowImplicitReturn);
                    if (returnDefResult.NextIndex > index)
                    {
                        item = new KvcExpression.KeyValueExpression
                        {
                            Key = null,
                            ValueExpression = returnDefResult.Block
                        };
                        return new GetKvcItemResult(item, returnDefResult.ParseNode, returnDefResult.NextIndex);
                    }

                    // If still nothing found, try key-only logic
                    if (!allowKeyOnly)
                        return new GetKvcItemResult(null, null, index);

                    // Try an identifier
                    var identifierResult = GetIdentifier(context, index, false);
                    if (identifierResult.NextIndex > index)
                    {
                        item = new KvcExpression.KeyValueExpression
                        {
                            Key = identifierResult.Iden,
                            KeyLower = identifierResult.IdenLower,
                            ValueExpression =
                                new ReferenceBlock(identifierResult.Iden, identifierResult.IdenLower, false)
                                {
                                    CodePos = index,
                                    CodeLength = identifierResult.NextIndex - index
                                }
                        };
                        return new GetKvcItemResult(item, identifierResult.ParseNode, identifierResult.NextIndex);
                    }

                    // Finally, try a simple string
                    var simpleStringResult = GetSimpleString(context, index);
                    if (simpleStringResult.NextIndex > index)
                    {
                        item = new KvcExpression.KeyValueExpression
                        {
                            Key = simpleStringResult.Str,
                            KeyLower = simpleStringResult.Str.ToLower(),
                            ValueExpression = new ReferenceBlock(
                                simpleStringResult.Str,
                                simpleStringResult.Str.ToLower(),
                                false)
                            {
                                CodePos = index,
                                CodeLength = simpleStringResult.NextIndex - index
                            }
                        };
                        return new GetKvcItemResult(item, simpleStringResult.ParseNode, simpleStringResult.NextIndex);
                    }

                    return new GetKvcItemResult(null, null, index);
                }
            }
            else
            {
                nameLower = name.ToLower();
            }

            // We have parsed a key. Now see if there's a colon or if key-only is allowed.
            i = SkipSpace(context, i).NextIndex;
            var i2 = GetLiteralMatch(context, i, ":").NextIndex;
            if (i2 == i)
            {
                // No colon -> key-only
                if (!allowKeyOnly)
                {
                    if (allowImplicitReturn)
                    {
                        var retExp = GetExpression(context, tokenStart);
                        if (retExp.NextIndex > i)
                        {
                            item = new KvcExpression.KeyValueExpression
                            {
                                Key = null,
                                ValueExpression = retExp.Block
                            };
                            return new GetKvcItemResult(item, retExp.ParseNode, retExp.NextIndex);
                        }
                    }
                    context.SyntaxErrors.Add(new SyntaxErrorData(i, 0, "':' expected"));
                    return new GetKvcItemResult(null, null, index);
                }

                item = new KvcExpression.KeyValueExpression
                {
                    Key = name,
                    KeyLower = nameLower,
                    ValueExpression = new ReferenceBlock(name, nameLower, true)
                };
                nodeName.NodeType = ParseNodeType.Key;
                parseNode = new ParseNode(ParseNodeType.Identifier, index, i - index, new[] { nodeName });
                return new GetKvcItemResult(item, parseNode, i);
            }

            // Consume the colon
            i = i2;

            // Parse the value expression
            i = SkipSpace(context, i).NextIndex;
            var (expBlock, nodeExpBlock, i3) = GetExpression(context, i);
            if (i3 == i)
            {
                context.SyntaxErrors.Add(new SyntaxErrorData(i, 0, "value expression expected"));
                return new GetKvcItemResult(null, null, index);
            }

            i = i3;
            i = SkipSpace(context, i).NextIndex;

            // Construct the key-value expression
            item = new KvcExpression.KeyValueExpression
            {
                Key = name,
                KeyLower = nameLower,
                ValueExpression = expBlock
            };
            nodeName.NodeType = ParseNodeType.Key;
            parseNode = new ParseNode(ParseNodeType.KeyValuePair, index, i - index, new[] { nodeName, nodeExpBlock });

            return new GetKvcItemResult(item, parseNode, i);
        }
    }
}