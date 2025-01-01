import { ParseContext, ParseNode, ParseNodeType, SyntaxErrorData } from "../FuncScriptParser.Main";
import { GetKvcItem } from "./FuncScriptParser.GetKvcItem";
import { GetLiteralMatch, GetLiteralMatchMultiple } from "./FuncScriptParser.GetLiteralMatch";
import { SkipSpace } from "./FuncScriptParser.SkipSpace";

export function GetKvcExpression(context: ParseContext, index: number) {
    const syntaxErrors = context.SyntaxErrors;
    let parseNode: ParseNode | null = null;
    let i = SkipSpace(context, index).NextIndex;
    let i2;

    // Always process the opening curly brace '{'
    i2 = GetLiteralMatch(context, i, "{").NextIndex;
    if (i2 === i) {
        return { ParseNode: null, NextIndex: index };
    }
    i = SkipSpace(context, i2).NextIndex;

    const nodeItems: ParseNode[] = [];
    do {
        if (nodeItems.length > 0) {
            i2 = GetLiteralMatchMultiple(context, i, [",", ";"]).NextIndex;
            if (i2 === i) {
                break;
            }
            i = SkipSpace(context, i2).NextIndex;
        }

        const kvcItemResult = GetKvcItem(context, i);
        i2 = kvcItemResult.NextIndex;
        const nodeOtherItem = kvcItemResult.ParseNode;
        
        if (i2 === i) {
            break;
        }

        nodeItems.push(nodeOtherItem!);
        i = SkipSpace(context, i2).NextIndex;
    } while (true);

    // Always process the closing curly brace '}'
    i2 = GetLiteralMatch(context, i, "}").NextIndex;
    if (i2 === i) {
        syntaxErrors.push(new SyntaxErrorData(i, 0, "'}' expected"));
        return { ParseNode: null, NextIndex: index };
    }
    i = SkipSpace(context, i2).NextIndex;

    parseNode = new ParseNode(ParseNodeType.KeyValueCollection, index, i - index, nodeItems);
    return { ParseNode: parseNode, NextIndex: i };
}