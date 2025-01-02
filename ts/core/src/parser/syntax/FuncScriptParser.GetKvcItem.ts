import { ParseContext, ParseResult, ParseNode, ParseNodeType, SyntaxErrorData } from "../FuncScriptParser.Main";
import { GetExpression } from "./FuncScriptParser.GetExpression";
import { GetIdentifier } from "./FuncScriptParser.GetIdentifier";
import { GetLiteralMatch } from "./FuncScriptParser.GetLiteralMatch";
import { GetReturnDefinition } from "./FuncScriptParser.GetReturnDefinition";
import { GetSimpleString } from "./FuncScriptParser.GetSimpleString";
import { SkipSpace } from "./FuncScriptParser.SkipSpace";

export function GetKvcItem(
    context: ParseContext,
    index: number,
    allowKeyOnly: boolean,
    allowReturn: boolean,
    allowImplicitReturn: boolean
): ParseResult {

    const originalIndex = index;
    let parseNode: ParseNode | null = null;

    // Try to parse a simple string as key
    const simpleStringResult = GetSimpleString(context, index);
    let name: string | null = null;
    let nodeName: ParseNode | null = null;
    let i = simpleStringResult.NextIndex;

    if (i === index) {
        // No simple string => try identifier
        const idResult = GetIdentifier(context, index, false);
        i = idResult.NextIndex;
        if (i === index) {
            // No identifier => fallback to return definition if allowed
            if (allowReturn) {
                const returnDefResult = GetReturnDefinition(context, index);
                if (returnDefResult.NextIndex > index) {
                    // We found a return definition
                    return {
                        ParseNode: returnDefResult.ParseNode,
                        NextIndex: returnDefResult.NextIndex
                    };
                }
            }

            // No return or key => if key-only is allowed, re-check identifier or string
            if (allowKeyOnly) {
                // Re-check identifier
                const idRes2 = GetIdentifier(context, index, false);
                if (idRes2.NextIndex > index) {
                    // Key-only identifier
                    const singleNode = new ParseNode(
                        ParseNodeType.Identifier,
                        index,
                        idRes2.NextIndex - index
                    );
                    return {
                        ParseNode: singleNode,
                        NextIndex: idRes2.NextIndex
                    };
                }

                // Re-check simple string
                const strRes2 = GetSimpleString(context, index);
                if (strRes2.NextIndex > index) {
                    // Key-only simple string
                    const singleNode = new ParseNode(
                        ParseNodeType.LiteralString,
                        index,
                        strRes2.NextIndex - index
                    );
                    return {
                        ParseNode: singleNode,
                        NextIndex: strRes2.NextIndex
                    };
                }
            }

            // Could not parse anything
            return {
                ParseNode: null,
                NextIndex: index
            };
        } else {
            // We have an identifier as key
            name = idResult.Iden;
            nodeName = idResult.ParseNode;
        }
    } else {
        // We have a simple string as key
        name = simpleStringResult.Str;
        nodeName = simpleStringResult.ParseNode;
        // We may want to set NodeType to LiteralString or Key here.
    }

    // We have a key (string or identifier). Check for colon
    i = SkipSpace(context, i).NextIndex;
    const colonMatch = GetLiteralMatch(context, i, ":");
    if (colonMatch.NextIndex === i) {
        // No colon => check if key-only is allowed
        if (!allowKeyOnly) {
            // Possibly parse the entire expression as an implicit return?
            if (allowImplicitReturn) {
                const exprResult = GetExpression(context, originalIndex);
                if (exprResult.NextIndex > i) {
                    // We recognized an expression from originalIndex
                    return {
                        ParseNode: exprResult.ParseNode,
                        NextIndex: exprResult.NextIndex
                    };
                }
            }
            // Still no parse => error
            context.SyntaxErrors.push(
                new SyntaxErrorData(i, 0, "':' expected")
            );
            return {
                ParseNode: null,
                NextIndex: originalIndex
            };
        }

        // Key-only is allowed => just return the key parse node
        if (nodeName) {
            nodeName.NodeType = ParseNodeType.Key;
            nodeName.Pos = originalIndex;
            nodeName.Length = i - originalIndex;
        }
        return {
            ParseNode: nodeName,
            NextIndex: i
        };
    }

    // Consume the colon
    i = colonMatch.NextIndex;
    i = SkipSpace(context, i).NextIndex;

    // Parse the value expression
    const valueResult = GetExpression(context, i);
    if (valueResult.NextIndex === i) {
        // No value expression
        context.SyntaxErrors.push(
            new SyntaxErrorData(i, 0, "value expression expected")
        );
        return {
            ParseNode: null,
            NextIndex: originalIndex
        };
    }
    i = valueResult.NextIndex;
    i = SkipSpace(context, i).NextIndex;

    // Construct the key node
    if (nodeName) {
        nodeName.NodeType = ParseNodeType.Key;
    }

    // Construct the key-value pair node
    const pairNode = new ParseNode(
        ParseNodeType.KeyValuePair,
        originalIndex,
        i - originalIndex,
        [
            nodeName ? nodeName : new ParseNode(ParseNodeType.Key, originalIndex, 0),
            valueResult.ParseNode!
        ]
    );

    return {
        ParseNode: pairNode,
        NextIndex: i
    };
}
