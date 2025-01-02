
import { ParseContext, ParseNode, ParseNodeType, ParseResult, SyntaxErrorData } from "../FuncScriptParser.Main";
import { SkipSpace } from "./FuncScriptParser.SkipSpace";
import { GetLiteralMatch, GetLiteralMatchMultiple } from "./FuncScriptParser.GetLiteralMatch";
import { GetKvcItem } from "./FuncScriptParser.GetKvcItem";

export function GetKvcExpression(context: ParseContext, index: number): ParseResult {
    const syntaxErrors = context.SyntaxErrors;
    let i = SkipSpace(context, index).NextIndex;
    const nodeStart = i;
    let i2 = GetLiteralMatch(context, i, "{").NextIndex;
    if (i2 === i) return { ParseNode: null, NextIndex: index };

    i = i2;
    const bodyResult = GetKvcBody(context, i, true);
    const { ParseNode: parseNode, NextIndex: nextIndex } = bodyResult;

    i = SkipSpace(context, nextIndex).NextIndex;
    i2 = GetLiteralMatch(context, i, "}").NextIndex;
    if (i2 === i) {
        syntaxErrors.push(new SyntaxErrorData(i, 0, "'}' expected"));
        return { ParseNode: null, NextIndex: index };
    }

    i = i2;
    if (parseNode) {
        parseNode.Pos = nodeStart;
        parseNode.Length = i - nodeStart;
    }

    return { ParseNode: parseNode, NextIndex: i };
}

export function GetNakedKvc(context: ParseContext, index: number): ParseResult {
    return GetKvcBody(context, index, false);
}

export function GetKvcBody(context: ParseContext, index: number, allowKeyOnly: boolean): ParseResult {
    const syntaxErrors = context.SyntaxErrors;
    let parseNode: ParseNode | null = null;
    let i = SkipSpace(context, index).NextIndex;
    const nodeItems: ParseNode[] = [];
    let retExp: ParseNode | null = null;

    do {
        let i2: number;
        if (nodeItems.length > 0 || retExp) {
            i2 = GetLiteralMatchMultiple(context, i, [",", ";"]).NextIndex;
            if (i2 === i) break;
            i = SkipSpace(context, i2).NextIndex;
        }

        const kvcItemResult = GetKvcItem(context, i, allowKeyOnly);
        i2 = kvcItemResult.NextIndex;
        const otherItem = kvcItemResult.ParseNode;

        if (!otherItem) break;

        if (otherItem.NodeType === ParseNodeType.ReturnExpression) {
            if (retExp) {
                syntaxErrors.push(new SyntaxErrorData(otherItem.Pos, otherItem.Length, "Duplicate return statement"));
                return { ParseNode: null, NextIndex: index };
            }
            retExp = otherItem;
        } 
        
        nodeItems.push(otherItem);        

        i = SkipSpace(context, i2).NextIndex;
    } while (true);

    parseNode = new ParseNode(ParseNodeType.KeyValueCollection, index, i - index, nodeItems);
    return { ParseNode: parseNode, NextIndex: i };
}
