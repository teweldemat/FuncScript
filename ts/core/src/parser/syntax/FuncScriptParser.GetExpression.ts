import { ParseContext, ParseResult } from "../FuncScriptParser.Main";
import { GetInfixExpression } from "./FuncScriptParser.GetInfixExpression";

export function GetExpression(context: ParseContext, index: number): ParseResult {
    const result = GetInfixExpression(context, index);
    if (result.NextIndex > index) {
        return result;
    }

    return { ParseNode: null, NextIndex: index };
}