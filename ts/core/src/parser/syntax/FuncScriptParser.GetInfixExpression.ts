import { ParseContext, s_operatorSymbols } from "../FuncScriptParser.Main";
import { GetInfixExpressionSingleLevel } from "./FuncScriptParser.GetInfixExpressionSingleLevel";

export function GetInfixExpression(context: ParseContext, index: number) {
    const result = GetInfixExpressionSingleLevel(context, s_operatorSymbols.length - 1, s_operatorSymbols[s_operatorSymbols.length - 1], index);

    return result;
}