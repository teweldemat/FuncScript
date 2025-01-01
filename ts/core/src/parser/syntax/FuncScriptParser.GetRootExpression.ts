import { ParseContext, ParseResult, SyntaxErrorData } from "../FuncScriptParser.Main";
import { GetExpression } from "./FuncScriptParser.GetExpression";

export function GetRootExpression(context: ParseContext, index: number): ParseResult {
    const thisErrors: SyntaxErrorData[] = [];
    const result = GetExpression(context, index);
    if (result.NextIndex > index) {
        context.SyntaxErrors.push(...thisErrors);
        return result;
    }
    return {ParseNode:null,NextIndex:index};
}