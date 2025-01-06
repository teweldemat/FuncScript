import { ParseContext, ParseResult } from "../FuncScriptParser.Main"; 
import { GetRootExpression } from "./FuncScriptParser.GetRootExpression";

export function Parse(context: ParseContext): ParseResult {
    const rootExpressionResult = GetRootExpression(context);
    return rootExpressionResult;
}