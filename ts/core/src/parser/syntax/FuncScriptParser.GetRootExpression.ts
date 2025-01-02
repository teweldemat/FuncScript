import { ParseContext, ParseResult } from "../FuncScriptParser.Main";
import { GetExpression } from "./FuncScriptParser.GetExpression";
import { GetNakedKvc } from "./FuncScriptParser.GetKvcExpression";

export function GetRootExpression(context: ParseContext, index: number): ParseResult {
    const res1 = GetNakedKvc(context, index);
    
    if (res1.NextIndex === context.Expression.trim().length) {
        return res1;
    }

    context.SyntaxErrors.length = 0;

    return GetExpression(context, 0);
}