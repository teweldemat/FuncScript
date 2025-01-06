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

    let parseNode: ParseNode | null = null;
    let name: string | null = null;
    let nameLower: string | null = null;
    let nodeName: ParseNode | null = null;
    const tokenStart = index;

    const simpleStringResult = GetSimpleString(context, index);
    let i = simpleStringResult.NextIndex;
    if (i === index) {
        const idResult = GetIdentifier(context, index, false);
        i = idResult.NextIndex;
        if (i === index) {
            if (!allowReturn) {
                return {
                    ParseNode: null,
                    NextIndex: index
                };
            }
            const returnDefResult = GetReturnDefinition(context, index, allowImplicitReturn);
            if (returnDefResult.NextIndex > index) {
                return {
                    ParseNode: returnDefResult.ParseNode,
                    NextIndex: returnDefResult.NextIndex
                };
            }
            if (!allowKeyOnly) {
                return {
                    ParseNode: null,
                    NextIndex: index
                };
            }

            const identifierResult = GetIdentifier(context, index, false);
            if (identifierResult.NextIndex > index) {
                const singleNode = new ParseNode(
                    ParseNodeType.Identifier,
                    index,
                    identifierResult.NextIndex - index
                );
                return {
                    ParseNode: singleNode,
                    NextIndex: identifierResult.NextIndex
                };
            }

            const strRes2 = GetSimpleString(context, index);
            if (strRes2.NextIndex > index) {
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

            return {
                ParseNode: null,
                NextIndex: index
            };
        } else {
            name = idResult.Iden;
            nameLower = idResult.IdenLower;
            nodeName = idResult.ParseNode;
        }
    } else {
        name = simpleStringResult.Str;
        nameLower = name!.toLowerCase();
        nodeName = simpleStringResult.ParseNode;
    }

    i = SkipSpace(context, i).NextIndex;
    const colonMatch = GetLiteralMatch(context, i, ":");
    if (colonMatch.NextIndex === i) {
        if (!allowKeyOnly) {
            if (allowImplicitReturn) {
                const exprResult = GetExpression(context, tokenStart);
                if (exprResult.NextIndex > tokenStart) {
                    return {
                        ParseNode: exprResult.ParseNode,
                        NextIndex: exprResult.NextIndex
                    };
                }
            }
            context.SyntaxErrors.push(new SyntaxErrorData(i, 0, "':' expected"));
            return {
                ParseNode: null,
                NextIndex: index
            };
        }

        if (nodeName) {
            nodeName.NodeType = ParseNodeType.Key;
        }
        parseNode = new ParseNode(
            ParseNodeType.Identifier,
            index,
            i - index,
            nodeName ? [nodeName] : []
        );
        return {
            ParseNode: parseNode,
            NextIndex: i
        };
    }

    i = colonMatch.NextIndex;
    i = SkipSpace(context, i).NextIndex;
    const valueResult = GetExpression(context, i);
    if (valueResult.NextIndex === i) {
        context.SyntaxErrors.push(new SyntaxErrorData(i, 0, "value expression expected"));
        return {
            ParseNode: null,
            NextIndex: index
        };
    }
    i = valueResult.NextIndex;
    i = SkipSpace(context, i).NextIndex;

    if (nodeName) {
        nodeName.NodeType = ParseNodeType.Key;
    }
    parseNode = new ParseNode(
        ParseNodeType.KeyValuePair,
        tokenStart,
        i - tokenStart,
        [
            nodeName ? nodeName : new ParseNode(ParseNodeType.Key, tokenStart, 0),
            valueResult.ParseNode!
        ]
    );

    return {
        ParseNode: parseNode,
        NextIndex: i
    };
}