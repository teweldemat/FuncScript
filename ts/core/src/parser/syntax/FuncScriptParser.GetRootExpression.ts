import { ParseContext, ParseResult } from "../FuncScriptParser.Main";
import { GetExpression } from "./FuncScriptParser.GetExpression";
import { GetNakedKvc } from "./FuncScriptParser.GetKvcExpression";
import { SkipSpace } from "./FuncScriptParser.SkipSpace";

export function GetRootExpression(context: ParseContext): ParseResult {
    const res1 = GetNakedKvc(context, 0);
    if (res1.NextIndex > 0) {
        const next = SkipSpace(context, res1.NextIndex);
        if (next.NextIndex === context.Expression.length) {
            return res1;
        }
    }
    
    context.SyntaxErrors.length = 0;
    return GetExpression(context, 0);
}