import { ParseContext, ParseNode, ParseNodeType, ParseResult } from "../FuncScriptParser.Main";
import { GetLiteralMatch } from "./FuncScriptParser.GetLiteralMatch";

export function GetInt(context: ParseContext, allowNegative: boolean, index: number): ParseResult {
    let parseNode: ParseNode | null = null;
    let i = index;

    if (allowNegative) {
        i = GetLiteralMatch(context, i, "-").NextIndex;
    }

    let i2 = i;
    while (i2 < context.Expression.length && /\d/.test(context.Expression[i2])) {
        i2++;
    }

    if (i === i2) {
        return { ParseNode: null, NextIndex: i };
    }

    i = i2;

    const intVal = context.Expression.substring(index, i);
    parseNode = new ParseNode(ParseNodeType.LiteralInteger, index, i - index);
    return { ParseNode: parseNode, NextIndex: i };
}