import { ParseContext, ParseNode, ParseNodeType, ParseResult } from "../FuncScriptParser.Main";
import { isCharWhiteSpace } from "../FuncScriptParser.Utils";

export function GetSpaceLessString(context: ParseContext, index: number): ParseResult {
    if (index >= context.Expression.length) {
        return { ParseNode: null, NextIndex: index };
    }

    let i = index;

    // Assuming a placeholder for isCharWhiteSpace as it's not to be implemented
    if (i >= context.Expression.length || isCharWhiteSpace(context.Expression[i])) {
        return { ParseNode: null, NextIndex: index };
    }

    i++;
    while (i < context.Expression.length && !isCharWhiteSpace(context.Expression[i])) {
        i++;
    }

    const text = context.Expression.substring(index, i);
    const parseNode = new ParseNode(ParseNodeType.Identifier, index, i - index);
    return { ParseNode: parseNode, NextIndex: i };
}